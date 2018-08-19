-- radiate_from_thing.lua by anotak
-- very experimental - draw lines based on a field-of-view algorithm from a central point

radius = 51200 -- TODO / FIXME think about removing thins

Map.DeselectGeometry() -- TODO REMOVE

cursor = UI.GetMouseMapPosition()

linedefs = Map.GetLinedefs()

-- create an axis-aligned bounding box
xmin = cursor.x - radius
xmax = cursor.x + radius
ymin = cursor.y - radius
ymax = cursor.y + radius


-- we're going to be adding start and end vertices to our data structure
points  = {}
--[[
	.segment_index
		is the index of the line segment in the other list
	.is_start
		boolean: is it the first vertex?
	.angle
		of the line from the centerpoint
	.distsq
		distance squared from the centerpoint
	.position
		the actual position, Vector2D in cartesian coordinates
--]]
function add_vertex(v, is_start, segment_index)
	--v.selected = true -- TODO REMOVE THIS DEBUGGING LINE
	
	p = {}
	
	p.position = v.position
	
	p.segment_index = segment_index;
	
	-- we will need to keep track of startness
	p.is_start = is_start
	
	-- let's get a vector that's equivalent to the line from the center
	-- to the vertex
	-- see also https://en.wikipedia.org/wiki/Polar_coordinates
	local delta_vector = v.position - cursor;
	p.angle = delta_vector.GetAngle() -- TODO should we filter lines that are colinear?
	
	-- we don't need actual distance/length because we will be doing comparisons,
	-- so we use distance squared.
	-- the square root required for a real distance calculation is slightly slow, this can
	-- add up when you have many vertices
	p.distsq = delta_vector.GetLengthSq();
	
	points[#points + 1] = p
	
	return p.distsq
end

-- as well as lines
segments = {}
--[[
.linedef is the actual linedef -- FIXME UNUSED?
.is_active is set to true at first, and then false when we're done with a line -- FIXME / TODO remove?
.far_distsq is the distance squared of the farther point
.close_distsq is the distance squared of the closer point
.sp_index is start point's index in points
.ep_index is end point's index in points.
	these two get set much later.
	i would choose a wordier name but it bugs me aesthetically that
	"end" and "start" have a different number of letters in them
--]]
function add_line(line)
	line.selected = true -- TODO REMOVE THIS DEBUGGING LINE
	
	segments[#segments + 1] = {}
	
	--segments[#segments].linedef = line
	segments[#segments].is_active = true
	
	--[[
		so a nice property here is that
		because we filtered lines by sidedness,
		we know that the second vertex will
		always be clockwise from the first vertex
		see https://doomwiki.org/wiki/Linedef
	--]]
	local sd = add_vertex(line.start_vertex, true, #segments)
	local ed = add_vertex(line.end_vertex , false, #segments)
	
	if ed > sd then
		segments[#segments].far_dist = ed
		segments[#segments].close_dist = sd
	else
		segments[#segments].far_dist = sd
		segments[#segments].close_dist = ed
	end
end

-- filter out lines outside of our box
for i, line in ipairs(linedefs) do
	local add_linedef = true
	
	-- also filter out 2 sided lines
	if line.GetBack() ~= nil then 
		add_linedef = false
	elseif line.start_vertex.position.x < xmin and line.end_vertex.position.x < xmin then
		add_linedef = false
	elseif line.start_vertex.position.y < ymin and line.end_vertex.position.y < ymin then
		add_linedef = false
	elseif line.start_vertex.position.x > xmax and line.end_vertex.position.x > xmax then
		add_linedef = false
	elseif line.start_vertex.position.y > ymax and line.end_vertex.position.y > ymax then
		add_linedef = false
	end
	
	if add_linedef then
		-- check here for sidedness of our point, only add lines that are facing toward it
		if line.SideOfLine(cursor) < 0 then
			add_line(line)
		end
	end
end



-- let's sort our points by angle
function point_comparison(a,b)
	if a.angle > b.angle then
		return true
	elseif a.angle < b.angle then
		return false
	elseif a.angle == b.angle then
		if a.distsq > b.distsq then
			return false
		elseif a.distsq < b.distsq then
			return true
		else
			-- TODO perhaps we should remove this case from the list somehow ??
			-- either they are same vertex or at least in the same exact spot
		end
	end
	
	return false
end

-- let's sort our segments by close_dist, close to far
-- FIXME / TODO remove?
function segment_comparison(a,b)
	if a.close_dist < b.close_dist  then
		return true
	elseif a.close_dist > b.close_dist  then
		return false
	elseif a.close_dist  == b.close_dist  then
		if a.far_dist > b.far_dist then
			return false
		elseif a.far_dist < b.far_dist then
			return true
		else
			if points[a.ep].angle < points[b.ep].angle then
				return true
			end
			-- TODO perhaps we should remove this case from the list somehow ??
			-- either they are same vertex or at least in the same exact spot
		end
	end
	
	return false
end

function set_up_points()
	table.sort(points, point_comparison)

	local p_index = 1

	-- let's set up the segments' .ep & .sp
	for p_index, cur_p in ipairs(points) do
		if cur_p.is_start then
			segments[cur_p.segment_index].ep = p_index
		else
			segments[cur_p.segment_index].sp = p_index
		end
	end
	
	table.sort(segments, segment_comparison);
	
	local segment_index = 1
	
	while segment_index <= #segments do
		s_points[segments[segment_index].sp].segment_index = segment_index
		e_points[segments[segment_index].ep].segment_index = segment_index
		segment_index = segment_index + 1
	end
end

set_up_points()

cur_walls = {}
for sp_index, cur_sp in ipairs(s_points) do
	cur_walls[#cur_walls + 1] = cur_sp.segment_index
	
	-- DEBUG
	debugt = Map.InsertThing(cur_sp.position)
	debugt.type = 1
	debugt.tag = sp_index
	debugt.SetAngleDoom((sp_index - 1) / #s_points * 360)
	-- END DEBUG
end

for ep_index, cur_ep in ipairs(e_points) do
	
	-- DEBUG
	debugt = Map.InsertThing(cur_ep.position)
	debugt.type = 9300
	debugt.tag = ep_index
	debugt.SetAngleDoom((ep_index - 1) / #e_points * 360)
	-- END DEBUG
end

--[[
	p = Pen.From(cursor)
	p.stitchrange = 2
	p.snaptogrid = false
	p.DrawVertex()
	p.position = cur_ep.position
	p.DrawVertex()
	p.FinishPlacingVertices()
	--]]











