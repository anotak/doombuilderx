-- skew.lua by anotak

function move_vert_relative(v, delta_vector)
	if delta_vector.GetLengthSq() < 2 then
		return false
	end
	-- fixme logic is awful here
	if not v.TryToMove(v.position + delta_vector) then
		return false
		--return move_vert_relative(v, delta_vector * 0.75)
	end
	return true
end -- function

function move_many_relative(vertices, move_horizontally, slope, farx, fary)
	local failed_to_move = {}
	local destination
	local relative_destination
	
	for _,v in ipairs(vertices) do
		if move_horizontally then
			destination = Vector2D.From(
							v.position.x + ((v.position.y - fary)*slope),
							v.position.y)
		else
			destination = Vector2D.From(
							v.position.x,
							v.position.y + ((v.position.x - farx)*slope))
		end
		
		relative_destination = destination - v.position
		if not move_vert_relative(v, relative_destination) then
			failed_to_move[#failed_to_move + 1] = v
		end
	end
	
	if #vertices == #failed_to_move then
		return
	end
	
	move_many_relative(failed_to_move,move_horizontally,slope,farx,fary)
end

local vertices = Map.GetSelectedVertices()
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
	
	old_w = old_xmax - old_xmin
	old_h = old_ymax - old_ymin
	
	-- find the center by averaging, then determine which sides of the center
	-- the cursor is on
	xcenter = (old_xmax + old_xmin) / 2
	ycenter = (old_ymax + old_ymin) / 2
	
	local nearestx
	local nearesty
	local farx
	local fary
	
	-- i'm a bit unsure as to how to handle == case
	if cursor.x > xcenter then
		nearestx = old_xmax
		farx = old_xmin
	else
		nearestx = old_xmin
		farx = old_xmax
	end
	
	if cursor.y > ycenter then
		nearesty = old_ymax
		fary = old_ymin
	else
		nearesty = old_ymin
		fary = old_ymax
	end
	
	local newx
	local newy
	local move_horizontally
	
	-- gotta figure out which direction we are going
	if math.abs(cursor.x - nearestx) > math.abs(cursor.y - nearesty) then
		newx = cursor.x
		newy = nearesty
		move_horizontally = true
	else
		newx = nearestx
		newy = cursor.y
		move_horizontally = false
	end
	
	-- we need to calculate a linear equation based on this info
	if move_horizontally then
		-- don't have to check div by 0 bc we added tiny number earlier
		slope = (newx - nearestx) / (nearesty - fary)
	else
		-- don't have to check div by 0 bc we added tiny number earlier
		slope = (newy - nearesty) / (nearestx - farx)
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
	
	move_many_relative(vertices, move_horizontally, slope, farx, fary)
end














