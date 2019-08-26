-- copy geometry.lua by anotak

linedefs = Map.GetSelectedLinedefs()

if #linedefs <= 0 then
	UI.LogLine("please select some vertices")
	return
end

UI.AddParameter("angle", "angle", 30, "angle to rotate by for each iteration")
UI.AddParameter("count", "count", 3, "angle to rotate by for each iteration")

parameters = UI.AskForParameters()
parameters.angle = parameters.angle * (math.pi/180)

dset = {}
l_paths = {}

function add_neighbors(old_lindex, neighbors, src_vert)
	if dset[old_lindex].nextl ~= nil then
		return
	end
	
	for _,l in ipairs(neighbors) do
		if l.selected then
			local new_lindex = l.GetIndex()
			
			if dset[new_lindex] == nil then
				dset[new_lindex] = {}
				dset[new_lindex].linedef = l
				
				if dset[old_lindex].nextl == nil then
					dset[old_lindex].nextl = new_lindex
				end
				
				-- make sure you don't leave from the same vertex
				if l.start_vertex.GetIndex() ~= src_vert.GetIndex() then
					add_neighbors(new_lindex, l.start_vertex.GetLines(), l.start_vertex)
					dset[new_lindex].go_from_start = true
				else
					add_neighbors(new_lindex, l.end_vertex.GetLines(), l.end_vertex)
					dset[new_lindex].go_from_start = false
				end
				
				return
				
			end
		end
		
	end -- end loop
end -- function

for _,l in ipairs(linedefs) do
	local lindex = l.GetIndex()
	if dset[lindex] == nil then
		dset[lindex] = {}
		dset[lindex].linedef = l
		
		add_neighbors(lindex, l.start_vertex.GetLines(), l.start_vertex)
		dset[lindex].go_from_start = true
		
		if dset[lindex].nextl == nil then
			add_neighbors(lindex, l.end_vertex.GetLines(), l.end_vertex)
			dset[lindex].go_from_start = false
		end
		
		l_paths[#l_paths + 1] = lindex
	end
end


pt_paths = {}

function travel_path_recursive(lindex, depth, path_index)
	--UI.LogLine(lindex)
	if depth == 1 then
		if dset[lindex].go_from_start then
			pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.end_vertex.position
		else
			pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.start_vertex.position
		end
	end
	
	if dset[lindex].nextl == nil then
		if dset[lindex].go_from_start then
			pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.start_vertex.position
		else
			pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.end_vertex.position
		end
		
		return
	end
	
	if dset[lindex].go_from_start then
		pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.start_vertex.position
	else
		pt_paths[path_index][#(pt_paths[path_index]) + 1] = dset[lindex].linedef.end_vertex.position
	end
	
	--return
	travel_path_recursive(dset[lindex].nextl, depth + 1,path_index)
end

for path_index,lindex in ipairs(l_paths) do
	pt_paths[path_index] = {}
	travel_path_recursive(lindex,1,path_index)
end


vertices = Map.GetSelectedVertices()


local xmin = vertices[1].position.x
local ymin = vertices[1].position.y
local xmax = vertices[1].position.x
local ymax = vertices[1].position.y

-- lets find the center of it all
for _,v in ipairs(vertices) do
	xmin = math.min(xmin, v.position.x)
	xmax = math.max(xmax, v.position.x)
	
	ymin = math.min(ymin, v.position.y)
	ymax = math.max(ymax, v.position.y)
end

centerpoint = Vector2D.SnappedToAccuracy(Vector2D.From((xmin + xmax) / 2, (ymin + ymax) / 2))

function transform_point(vect, iteration)
	local offset = Vector2D.SnappedToAccuracy(vect - centerpoint)
	local v_angle = offset.GetAngle()
	local v_dist = offset.GetLength()
	
	v_angle = v_angle + (iteration * parameters.angle)
	
	local output = Vector2D.FromAngle(v_angle, v_dist) + centerpoint
	output = Vector2D.SnappedToAccuracy(output)
	
	return output
end


function draw_paths(iteration)
	for path_index,_ in ipairs(pt_paths) do
		
		p = Pen.From(transform_point(pt_paths[path_index][1], iteration))
		p.stitchrange = 2
		p.snaptogrid = false
		
		p.DrawVertex()
		for i = 2,#(pt_paths[path_index]) do
			p.position = transform_point(pt_paths[path_index][i], iteration)
			--Map.InsertThing(p.position)
			p.DrawVertex()
		end
		
		p.FinishPlacingVertices()
	end
end

-- do it twice bc of wackiness with doom builder merging algorithm
for i=1,parameters.count do
	draw_paths(i)
	draw_paths(i)
end








