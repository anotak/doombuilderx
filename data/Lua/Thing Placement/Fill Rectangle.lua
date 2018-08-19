-- flood_fill_things.lua by anotak
-- if you select two things, it fills the resulting rectangle
-- full of clones of the first thing
-- the only way it refers to the second thing is the angle,
-- as it smoothly averages the angle of the floodfilled things

-- amount of padding space between things
padding = 2

-- let's get the table of selected things
selected_things = Map.GetSelectedThings()

-- we need 2 things selected to make a box out of them
-- other numbers don't make sense
if #selected_things == 2 then
	-- let's keep track of how many things we made / unmade
	created_things = 0
	
	-- let's make a box out of the positions of our selected things
	-- for more info about math.min / math.max, see http://lua-users.org/wiki/MathLibraryTutorial
	west = math.min(selected_things[1].position.x, selected_things[2].position.x)
	east = math.max(selected_things[1].position.x, selected_things[2].position.x)
	
	-- the bigger y is farther north
	south = math.min(selected_things[1].position.y, selected_things[2].position.y)
	north = math.max(selected_things[1].position.y, selected_things[2].position.y)
	
	-- simple geometry
	width = east - west
	height = north - south
	
	-- how big is the thing, then we add the padding
	step_size = selected_things[1].GetRadius() * 2 + padding
	
	-- we prevent infinite loops by doing this
	if step_size < 1 then
		step_size = 1
	end
	
	--[[
		math.ceil rounds up, see http://lua-users.org/wiki/MathLibraryTutorial
		we see how many steps we can fit into the rectangle, then round up
		(round up because of cases like our rectangle being 0-wide because the
		2 things are axis-aligned)
	--]]
	x_thing_count = math.ceil(width / step_size)
	y_thing_count = math.ceil(height / step_size)
	
	-- get the angles of our things so we can have the
	-- neat smoothing of angles between them
	first_angle = selected_things[1].GetAngleDoom()
	second_angle = selected_things[2].GetAngleDoom()
	
	-- make sure our things dont just turn all the way the
	-- long way around, instead they will take the shorter
	-- direction on the circle
	if math.abs(first_angle - second_angle) > 180 then
		if first_angle > second_angle then
			first_angle = first_angle - 360
		else
			second_angle = second_angle - 360
		end
	end
	
	-- in order to do that properly, we need to associate the angles
	-- with the cardinal directions (you'll see how this is used later)
	if selected_things[1].position.x < selected_things[2].position.x then
		if selected_things[1].position.y < selected_things[2].position.y then
			-- 1st is southwest
			-- 2nd is northeast
			north_angle = second_angle
			south_angle = first_angle
			east_angle = second_angle
			west_angle = first_angle
		else
			-- 1st is northwest
			-- 2nd is southeast
			north_angle = first_angle
			south_angle = second_angle
			east_angle = second_angle
			west_angle = first_angle
		end
	elseif selected_things[1].position.y < selected_things[2].position.y then
		-- 1st is southeast
		-- 2nd is northwest
		north_angle = second_angle
		south_angle = first_angle
		east_angle = first_angle
		west_angle = second_angle
	else
		-- 1st is northeast
		-- 1st is southwest
		north_angle = first_angle
		south_angle = second_angle
		east_angle = first_angle
		west_angle = second_angle
	end
	
	-- we go southwest -> northeast, placing things, per column
	-- go for each column
	for x=0, x_thing_count do
		-- go for each location within the column
		for y=0, y_thing_count do
			-- copy the first selected thing, and
			-- set move it to the correct position
			new_thing = selected_things[1].Clone(
				x * step_size + west,
				y * step_size + south)
			
			
			-- handle the case where we have only 1 row or column
			-- otherwise the math works out wrong
			if x_thing_count == 0 then
				-- vertical line case
				
				y_amount = y / y_thing_count
				
				-- we will get the angle by lerping between
				-- the angles based on our current position
				-- see https://en.wikipedia.org/wiki/Lerp_(computing)
				angle = south_angle + y_amount * (north_angle - south_angle)
				
				-- set our angle
				new_thing.SetAngleDoom(angle)
			elseif y_thing_count == 0 then
				-- horizontal line case
				
				x_amount = x / x_thing_count
				
				-- we will get the angle by lerping between
				-- the angles based on our current position
				-- see https://en.wikipedia.org/wiki/Lerp_(computing)
				angle = west_angle + x_amount * (east_angle - west_angle)
				
				-- set our angle
				new_thing.SetAngleDoom(angle)
			else
				-- true rectangular case
				
				-- get how far along we are in the rectangle in the range
				-- of 0.0 to 1.0
				x_amount = x / x_thing_count
				y_amount = y / y_thing_count
				
				-- we will get the components of the angles by lerping between
				-- the angles at each side based on our current position
				-- see https://en.wikipedia.org/wiki/Lerp_(computing)
				ew_angle = west_angle + x_amount * (east_angle - west_angle)
				ns_angle = south_angle + y_amount * (north_angle - south_angle)
				
				
				-- then we will average the two and set the angle to that average
				new_thing.SetAngleDoom((ew_angle + ns_angle) / 2)
			end
			-- done with this thing
		end
		-- done with this column
	end
	-- done with each column

	-- delete the original things, because we just put replacements on the map
	selected_things[1].Dispose()
	selected_things[2].Dispose()
else
	-- let's let the user know what went wrong
	UI.LogLine("You need to select exactly 2 things.")
	UI.LogLine("")
	UI.LogLine("The first thing will determine the resulting things' properties.")
	UI.LogLine("The angle of both will be used to determine the angles of the new things.")
	UI.LogLine("")
	UI.LogLine("If you have any sectors selected, it will fill only those sectors.")
end