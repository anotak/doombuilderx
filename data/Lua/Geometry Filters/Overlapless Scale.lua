-- overlapless_scale.lua by anotak

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
	cursor = UI.GetMouseMapPosition(false,false)

	
	
	-- create a bounding box
	old_xmin = MapFormat.GetRightBoundary()
	old_xmax = MapFormat.GetLeftBoundary()
	old_ymin = MapFormat.GetTopBoundary()
	old_ymax = MapFormat.GetBottomBoundary()
	
	for _,v in ipairs(vertices) do
		old_xmin = math.min(old_xmin, v.position.x)
		old_xmax = math.max(old_xmax, v.position.x)
		old_ymin = math.min(old_ymin, v.position.y)
		old_ymax = math.max(old_ymax, v.position.y)
	end
	
	old_w = old_xmax - old_xmin
	old_h = old_ymax - old_ymin
	
	-- find the center by averaging, then determine which sides of the center
	-- the cursor is on
	xcenter = (old_xmax + old_xmin) / 2
	ycenter = (old_ymax + old_ymin) / 2
	
	-- i'm a bit unsure as to how to handle == case
	if cursor.x > xcenter then
		new_xmin = old_xmin
		new_xmax = cursor.x
	else
		new_xmin = cursor.x
		new_xmax = old_xmax
	end
	
	if cursor.y > ycenter then
		new_ymin = old_ymin
		new_ymax = cursor.y
	else
		new_ymin = cursor.y
		new_ymax = old_ymax
	end
	
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
	
	new_w = new_xmax - new_xmin
	new_h = new_ymax - new_ymin
	
	new_origin_v = Vector2D.From(new_xmin, new_ymin)
	new_bounds_v = Vector2D.From(new_w, new_h)
	old_origin_v = Vector2D.From(old_xmin, old_ymin)
	old_bounds_v = Vector2D.From(old_w, old_h)
	
	
	
	for _,v in ipairs(vertices) do
		destination = ((v.position - old_origin_v) / old_bounds_v) * new_bounds_v + new_origin_v
		relative_destination = destination - v.position
		move_vert_relative(v, relative_destination)
	end
	
end














