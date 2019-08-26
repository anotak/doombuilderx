-- polar_scale.lua by anotak
-- this works but im not sure what the point is

function move_vert_relative(v, delta_vector)
	if delta_vector.GetLengthSq() < 2 then
		return
	end
	if not v.TryToMove(v.position + delta_vector) then
		move_vert_relative(v, delta_vector * 0.75)
	end
end -- function

vertices = Map.GetSelectedVertices()
if #vertices == 0 then
	UI.LogLine("No geometry selected!")
else
	-- to scale vertices toward mouse position,
	-- well, we need the mouse position in map coordinates
	-- FIXME figure out the snaptogrid
	local cursor = UI.GetMouseMapPosition(false,false)

	
	
	-- create a bounding box
	local old_xmin = MapFormat.GetRightBoundary()
	local old_xmax = MapFormat.GetLeftBoundary()
	local old_ymin = MapFormat.GetTopBoundary()
	local old_ymax = MapFormat.GetBottomBoundary()
	
	for _,v in ipairs(vertices) do
		old_xmin = math.min(old_xmin, v.position.x)
		old_xmax = math.max(old_xmax, v.position.x)
		old_ymin = math.min(old_ymin, v.position.y)
		old_ymax = math.max(old_ymax, v.position.y)
	end
	
	-- we need to make 0-width or length things slightly bigger in order to prevent
	-- division by 0
	if old_xmin == old_xmax then
		old_xmin = old_xmin - 0.1
		old_xmax = old_xmax + 0.1
	end
	
	if old_ymin == old_ymax then
		old_ymin = old_ymin - 0.1
		old_ymax = old_ymax + 0.1
	end
	
	-- find the center by averaging, then determine which sides of the center
	-- the cursor is on
	local xcenter = (old_xmax + old_xmin) / 2
	local ycenter = (old_ymax + old_ymin) / 2
	
	local center = Vector2D.From(xcenter,ycenter)
	
	local rmax = 0
	for _,v in ipairs(vertices) do
		rmax = math.max(rmax, Vector2D.DistanceSq(center,v.position))
	end
	
	rmax = math.sqrt(rmax)
	local rcursor = Vector2D.Distance(cursor, center)
	
	--[[
	we will sort the list of vertices, so that vertices closer
	to the moving corner will be earlier in the list, and therefore
	they will be moved earlier

	see these links for more information on table.sort
	https://www.lua.org/pil/19.3.html
	http://lua-users.org/wiki/TableLibraryTutorial
	--]]
	function vertex_comparison(a,b)
		local which = Vector2D.DistanceSq(cursor, a.position) < Vector2D.DistanceSq(cursor, b.position)
		return which
	end
	
	table.sort(vertices, vertex_comparison)
	
	UI.LogLine("original")
	UI.LogLine(center)
	UI.LogLine(rmax)
	UI.LogLine("cursor")
	UI.LogLine(cursor)
	UI.LogLine(rcursor)
	
	UI.LogLine("-----------")
	
	local ratio = rcursor / rmax
	UI.LogLine("ratio: " .. ratio)
	for _,v in ipairs(vertices) do
		local old_rel = (v.position - center)
		local angle = old_rel.GetAngle()
		
		local r = Vector2D.Distance(v.position, center) * ratio
		--[[UI.LogLine(Vector2D.Distance(v.position, center))
		UI.LogLine(r)
		
		UI.LogLine("\n")--]]
		local dest = Vector2D.FromAngle(angle, r)
		--[[UI.LogLine(old_rel)
		UI.LogLine(dest)
		
		UI.LogLine("-\n\n")--]]
		
		--local relative_dest = dest - v.position
		
		move_vert_relative(v, dest - old_rel)
	end
	
end














