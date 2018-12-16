-- vortex.lua by anotak
-- spins vertices around point where you click
-- based on Black Hole.lua

vortex = {}

-- cursor is a vector, where to put center of vortex

-- delta_angle is how much to move vertices, in radians
-- reasonable default is math.pi / 8

-- max_distance -- vertices farther than this won't be moved
-- reasonable default is 64
function vortex.do_vortex(cursor, max_distance, delta_angle)

	-- we'll be using squared distances to do our comparisons
	-- because the square root is an expensive operation
	local max_distance_sq = max_distance * max_distance

	-- don't move the vertex if the angle is smaller than this
	local min_angle = math.pi / 256

	-- so by default, we use the selected vertices only
	local vertices = Map.GetSelectedVertices()
	-- if nothing selected, just do all the vertices
	if #vertices == 0 then
		vertices = Map.GetVertices()
	end

	-- make a new empty table of vertices
	local moving_vertices = {}

	-- lets make a list of the vertices we will actually move
	for _, vert in ipairs(vertices) do
		--[[ to do so, we compare the squared distances vs our previously
			calculated maximum
			note, we are using squared distances because it's a lot faster
			to not do the square root to calculate distance.
			you should only do the square root or call
			Vector2D.Distance if you need to.
		--]]
		
		local distance_squared = Vector2D.DistanceSq(cursor, vert.position)
		
		if distance_squared < max_distance_sq then
			moving_vertices[#moving_vertices+1] = vert
		end
	end

	--[[
	we will sort the list of vertices, so that vertices closer
	to the mouse will be earlier in the list, and therefore
	they will be moved earlier

	see these links for more information on table.sort
	https://www.lua.org/pil/19.3.html
	http://lua-users.org/wiki/TableLibraryTutorial
	--]]
	function vertex_comparison(a,b)
		local which = Vector2D.DistanceSq(cursor, a.position) < Vector2D.DistanceSq(cursor, b.position)
		return which
	end
	table.sort(moving_vertices, vertex_comparison)


	-- we set up a function to suck in 1 vertex, we will do this
	-- recursively if we can't move it
	function twist_vertex(vert, cur_delta_angle)
		-- once the "gravity" is low enough, don't move at all
		if math.abs(cur_delta_angle) < min_angle then
			return
		end
		
		-- we'll be using polar coordinates for this
		-- see https://en.wikipedia.org/wiki/Polar_coordinates
		
		-- get our distance again vs the mouse map position
		local distance = Vector2D.Distance(cursor, vert.position)
		
		-- we get the angle of the vector from the click location to the vertex
		local angle = (vert.position - cursor).GetAngle()
		
		-- then we can move our vertex in polar space
		angle = angle + (cur_delta_angle * ((max_distance_sq - (distance * distance)) / max_distance_sq))
		
		-- then convert back
		local destination = Vector2D.FromAngle(angle, distance)
		
		-- and move it back to relative to the mouse cursor
		destination = destination + cursor
		
		-- snap to our map format's accuracy
		-- (positions internally to doom builder are floating point decimals, but
		-- the map file formats are snapped to integers or fixed point numbers
		-- and therefore bad things can happen if we don't snap the positions 
		-- to these numbers)
		destination = Vector2D.SnappedToAccuracy(destination)
		
		-- okay, so let's try to move the vertex
		-- TryToMove is a nice function that tries to move our vertex
		-- but only succeeds if we aren't going to run into another vertex
		-- or create overlapping linedefs or other bad geometry
		-- it returns a boolean
		if not vert.TryToMove(destination) then
			-- so if the TryToMove failed, we will try all of this again
			-- with a smaller gravity
			twist_vertex(vert, cur_delta_angle * 0.75)
		end
	end

	-- now that we declared that function 'suck_in_vertex'
	-- we will go through all our vertices and call
	-- that function on them
	for _, vert in ipairs(moving_vertices) do
		twist_vertex(vert, delta_angle)
	end
end