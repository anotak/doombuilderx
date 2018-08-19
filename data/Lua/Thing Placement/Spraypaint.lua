-- Thing Placement/Spraypaint.Lua by anotak
-- places things around where you click but also doesn't place too close to existing things of this kind

brush_count = 10
brush_size = 100

math.randomseed((os.time() - math.floor(os.time())) * 100000)

-- well, we need the mouse position in map coordinates
cursor = UI.GetMouseMapPosition(false,false)

-- let's add a thing to the map and get it's type to figure out
-- what kind of thing inserting things is making
current_thing = Map.InsertThing(0,0)
current_thing_type = current_thing.type
-- the 1 is to adjust for snapping, and to put some padding
current_thing_diameter = current_thing.GetRadius() * 2 + 1

-- we also add the radius squared to the brush size
brush_size = brush_size + current_thing_diameter

-- and then delete it because we don't need it
current_thing.Dispose()

all_things = Map.GetThings()
relevant_things = {}
for _,thing in ipairs(all_things) do
	if thing.type == current_thing_type then
		distance = Vector2D.ManhattanDistance(thing.position, cursor)
		
		if distance <= brush_size + (current_thing_diameter * 2) then
			relevant_things[#relevant_things + 1] = thing
		end
	end
end

for i=1, brush_count do
	local destination = cursor +
			Vector2D.FromAngle(math.random() * math.pi * 6, math.random() * brush_size)
	
	-- snap to our map format's accuracy
	-- (positions internally to doom builder are floating point decimals, but
	-- the map file formats are snapped to integers or fixed point numbers
	-- and therefore bad things can happen if we don't snap the positions 
	-- to these numbers)
	destination = Vector2D.SnappedToAccuracy(destination)
	
	local have_space = true
	for j, other in ipairs(relevant_things) do
		x_dist = math.abs(other.position.x - destination.x)
		y_dist = math.abs(other.position.y - destination.y)
		
		if x_dist <= current_thing_diameter and y_dist <= current_thing_diameter then
			have_space = false
			break
		end
	end
	
	if have_space then
		relevant_things[#relevant_things + 1] = Map.InsertThing(destination)
	end
end
