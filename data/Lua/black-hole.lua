-- black-hole.lua by anotak
-- sucks in vertices toward where you right click

-- try different numbers. default was 73325 in case you forget
gravity = 73325

-- gravity less than 256 doesn't make sense with how
-- this script works
if gravity < 256 then
	UI.LogLine("gravity less than 256 unsupported")
end

--[[
so that we dont just move all the vertices
on the map, we should find our maximum
effective distance

the amount we move vertices by is going to
be == to gravity divided by distance squared
so we want to find the distance squared
at which the amount <= 1, plus some to account
for rounding, so let's say, <= 1.5

so lets algebra this out
	amount = gravity / distance * distance
	distance * distance * amount = gravity
	distance * distance = gravity / amount
	
	since we know amount is 1.5
	
	distance * distance = gravity / 1.5
	
	so our max effective distance squared is
	gravity / 1.5
--]]

max_distance_sq = gravity / 1.5

-- so by default, we use all the vertices on the map
vertices = Map.GetSelectedVertices()
-- if nothing selected, just do all the vertices
if #vertices == 0 then
	vertices = Map.GetVertices()
end

-- to warp vertices toward mouse position,
-- well, we need the mouse position in map coordinates
cursor = UI.GetMouseMapPosition(false,false)

-- make a new empty table of vertices
moving_vertices = {}

-- lets make a list of the vertices we will actually move
for _, vert in ipairs(vertices) do
	--[[ to do so, we compare the squared distances vs our previously
		calculated maximum
		note, we are using squared distances because it's a lot faster
		to not do the square root to calculate distance.
		you should only do the square root or call
		Vector2D.Distance if you need to.
	--]]
	
	distance_squared = Vector2D.DistanceSq(cursor, vert.position)
	
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
	which = Vector2D.DistanceSq(cursor, a.position) < Vector2D.DistanceSq(cursor, b.position)
	return which
end
table.sort(moving_vertices, vertex_comparison)


-- we set up a function to suck in 1 vertex, we will do this
-- recursively if we can't move it
function suck_in_vertex(vert, current_gravity)
	-- once the gravity is low enough, don't move at all
	if current_gravity < 256 then
		return
	end
	
	-- get our distance squared again vs the mouse map position
	distance_squared = Vector2D.DistanceSq(cursor, vert.position)
	
	-- we're close enough! and therefore done with this vertex
	if distance_squared < 6 then
		return
	end
	
	if distance_squared < max_distance_sq then
		
		-- we calculate a force via the inverse square law
		-- see https://en.wikipedia.org/wiki/Inverse_square_law
		force = current_gravity / distance_squared
		
		-- don't let vertices move too far
		if force > 16 then
			force = 16
		end
		
		-- we get a vector toward the mouse cursor
		direction = (cursor - vert.position)
		
		-- then we normalize it (get the unit vector)
		-- see https://en.wikipedia.org/wiki/Unit_vector
		direction = direction.GetNormal()
		
		-- we have 2d unit vector to give us direction, and a 1d force
		-- so we multiply the two to get our 2d movement
		movement_vector = direction * force
		
		-- snap to our map format's accuracy
		-- (positions internally to doom builder are floating point decimals, but
		-- the map file formats are snapped to integers or fixed point numbers
		-- and therefore bad things can happen if we don't snap the positions 
		-- to these numbers)
		destination = Vector2D.SnappedToAccuracy(vert.position + movement_vector)
		
		-- we check that we aren't going to move either farther past the destination
		-- than we started, and also that we aren't moving farther from our 
		-- origin than our actual destination was (to prevent overshooting)
		if Vector2D.DistanceSq(cursor, destination) > distance_squared 
			or Vector2D.DistanceSq(vert.position, destination) > distance_squared then
			-- if that's the case, then we just move toward the cursor
			destination = cursor
		end
		
		-- okay, so let's try to move the vertex
		-- TryToMove is a nice function that tries to move our vertex
		-- but only succeeds if we aren't going to run into another vertex
		-- or create overlapping linedefs or other bad geometry
		-- it returns a boolean
		if not vert.TryToMove(destination) then
			-- so if the TryToMove failed, we will try all of this again
			-- with a smaller gravity
			suck_in_vertex(vert,current_gravity * 0.9)
		end
	end
end

-- now that we declared that function 'suck_in_vertex'
-- we will go through all our vertices and call
-- that function on them
for _, vert in ipairs(moving_vertices) do
	suck_in_vertex(vert, gravity)
end