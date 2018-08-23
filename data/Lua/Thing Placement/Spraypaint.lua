-- Thing Placement/Spraypaint.Lua by anotak
-- places things around where you click but also doesn't place too close to existing things of this kind

-- TODO add some way to properly change these parameters in the editor
-- how many things to attempt to place
brush_count = 10
-- how big is the spray "brush" radius in map units
brush_size = 100

-- initialize our random seed
math.randomseed((os.time() - math.floor(os.time())) * 100000)

-- well, we need the mouse position in map coordinates
cursor = UI.GetMouseMapPosition(false,false)

--[[
let's add a thing to the map and get it's type to figure out
what thing the user would get if they inserted a new thing in Things Mode
--]]
current_thing = Map.InsertThing(0,0)
current_thing_type = current_thing.type
-- the 1 is to adjust for snapping, and to put some padding
current_thing_diameter = current_thing.GetRadius() * 2 + 1

-- we also add the radius squared to the brush size
brush_size = brush_size + current_thing_diameter

-- and then delete it because we don't need it
current_thing.Dispose()

--[[
we don't want to place things on top of each other,
so we need to figure out what things on the map are
near where the user clicked
--]]

all_things = Map.GetThings()
-- we need a table to store the nearby things in
relevant_things = {}

for _,thing in ipairs(all_things) do
	-- we only want things of the same type
	if thing.type == current_thing_type then
		-- we'll see what's in range.
		-- things are 'squareish' in practice so we'll be using manhattan distance
		-- see https://en.wikipedia.org/wiki/Manhattan_distance
		distance = Vector2D.ManhattanDistance(thing.position, cursor)
		
		-- add it if it's within our brush + some extra space
		if distance <= brush_size + (current_thing_diameter * 2) then
			relevant_things[#relevant_things + 1] = thing
		end
	end
end

for i=1, brush_count do
	-- place our thing randomly within a circle around the cursor
	local destination = cursor +
			Vector2D.FromAngle(math.random() * math.pi * 6, math.random() * brush_size)
	
	-- snap to our map format's accuracy
	-- (positions internally to doom builder are floating point decimals, but
	-- the map file formats are snapped to integers or fixed point numbers
	-- and therefore bad things can happen if we don't snap the positions 
	-- to these numbers)
	destination = Vector2D.SnappedToAccuracy(destination)
	
	-- we need to only add our thing if we have space to add it
	-- so we need to check against all the relevant things
	local have_space = true
	
	for j, other in ipairs(relevant_things) do
		-- check if our potential new thing is within the same square as the other one
		x_dist = math.abs(other.position.x - destination.x)
		y_dist = math.abs(other.position.y - destination.y)
		
		if x_dist <= current_thing_diameter and y_dist <= current_thing_diameter then
			-- well, we didn't have space, so lets break out of the loop early
			have_space = false
			break
		end
	end
	
	if have_space then
		-- finally get to add our new thing
		relevant_things[#relevant_things + 1] = Map.InsertThing(destination)
	end
end
