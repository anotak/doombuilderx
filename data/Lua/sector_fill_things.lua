-- sector_fill_things.lua by anotak
-- fills selected sectors with things

-- if you select two things, it fills the resulting rectangle
-- full of clones of the first thing
-- the only way it refers to the second thing is the angle,
-- as it smoothly averages the angle of the floodfilled things

-- amount of padding space between things
padding = 2

-- let's get the table of selected things
selected_things = Map.GetSelectedThings()

-- let's get the table of selected sectors
selected_sectors = Map.GetSelectedSectors()

-- this function gets the bounding box of the lines that make up a sector
-- see https://en.wikipedia.org/wiki/Bounding_box
function get_sector_bounding_box(sector, east, west, north, south)
	local lines = sector.GetLinedefs()
	
	-- compare every line against our current box and make it bigger if needed
	-- see https://www.lua.org/pil/7.3.html for info on ipairs
	for _,line in ipairs(lines) do
		-- for more info about math.min / math.max, see http://lua-users.org/wiki/MathLibraryTutorial
		west = math.min(west, line.start_vertex.position.x)
		east = math.max(east, line.start_vertex.position.x)
		
		-- the bigger y is farther north
		south = math.min(south, line.start_vertex.position.y)
		north = math.max(north, line.start_vertex.position.y)
		
		-- now we need to handle the other vertex of the line
		west = math.min(west, line.end_vertex.position.x)
		east = math.max(east, line.end_vertex.position.x)
		
		south = math.min(south, line.end_vertex.position.y)
		north = math.max(north, line.end_vertex.position.y)
	end
	
	-- lua, unlike many languages, lets you return many values
	return east, west, north, south
end

function sector_fill_things()
	-- let's keep track of how many things we made / unmade
	local created_things = 0
	
	-- our things are locally sourced
	-- jokes aside, we need to say that it's local here because of scoping stuff
	-- see http://lua-users.org/wiki/ScopeTutorial for more information
	local source_thing
	if #selected_things > 0 then
		source_thing = selected_things[1]
	else
		source_thing = Map.InsertThing(0,0)
	end
	
	local west
	local east
	local south
	local north
	
	-- figure out what type of bounding box to use
	if #selected_things == 2 then
		-- in this case we will make a box out of the
		-- positions of our 2 selected things
		west = source_thing.position.x
		east = source_thing.position.x
		south = source_thing.position.y
		north = source_thing.position.y
		
		-- for more info about math.min / math.max, see http://lua-users.org/wiki/MathLibraryTutorial
		west = math.min(west, selected_things[2].position.x)
		east = math.max(east, selected_things[2].position.x)
		
		-- the bigger y is farther north
		south = math.min(south, selected_things[2].position.y)
		north = math.max(north, selected_things[2].position.y)
	else
		-- otherwise lets make a box out of our selected sectors
		local first_sector_lines = selected_sectors[1].GetLinedefs()
		local box_starting_point = first_sector_lines[1].start_vertex.position
		
		east = box_starting_point.x
		west = box_starting_point.x
		
		north = box_starting_point.y
		south = box_starting_point.y
		
		for _,sector in ipairs(selected_sectors) do
			east, west, north, south = get_sector_bounding_box(sector, east, west, north, south)
		end
	end
	
	-- simple geometry
	width = east - west
	height = north - south
	
	-- how big is the thing, then we add the padding
	step_size = source_thing.GetRadius() * 2 + padding
	
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
	local first_angle = source_thing.GetAngleDoom()
	local second_angle = first_angle
	
	local north_angle
	local south_angle
	local east_angle
	local west_angle
	
	-- we can only do this if we have 2 things though
	if #selected_things == 2 then
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
		if source_thing.position.x < selected_things[2].position.x then
			if source_thing.position.y < selected_things[2].position.y then
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
		elseif source_thing.position.y < selected_things[2].position.y then
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
	else
		-- if we have less than 2 things we still need to set our angles
		north_angle = first_angle
		south_angle = first_angle
		east_angle = first_angle
		west_angle = first_angle
	end
	-- done figuring out the angles for when we have 2 things
	
	-- we go southwest -> northeast, placing things
	for x=0, x_thing_count do
		for y=0, y_thing_count do
			-- copy the first selected thing, and
			-- set move it to the correct position
			local new_thing = source_thing.Clone(
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
			
			created_things = created_things + 1
			
			if #selected_sectors > 0 then
				thing_sector = Map.DetermineSector(new_thing.position)
				
				if thing_sector == nil or thing_sector.selected == false then
					new_thing.Dispose()
					created_things = created_things - 1
				end
			end
		end
	end
	
	-- let the user know why no new things were created
	if #selected_sectors > 0 and created_things == 0 and selected_things == 2 then
		UI.LogLine("You had 2 things selected but your thing-box didn't overlay with the chosen sectors, so nothing got made.")
	else
		-- delete the original things, because we just put replacements on the map
		source_thing.Dispose()
		if #selected_things == 2 then
			selected_things[2].Dispose()
		end
	end
end

-- if no sectors are selected, let's use where the user clicked
if #selected_sectors <= 0 then
	-- well, we need to know where we clicked
	-- the first false means no snap to grid
	-- the second means no snapping to geometry
	local cursor = UI.GetMouseMapPosition(false, false)

	-- find sector exactly at mouse
	selected_sectors[1] = Map.DetermineSector(cursor)

	-- if nothing at current exact mouse position, let's try to just find what's nearby
	if selected_sectors[1] == nil then
		selected_sectors[1] = Map.NearestSector(cursor)
	end
	
	selected_sectors[1].selected = true
end 

-- we need 2 things selected to make a box out of them
-- 1 thing to copy
-- or 0 things to just use what would be placed when you place
-- a new thing in Things Mode when you right click
-- other numbers don't make sense 
if #selected_things <= 2 then
	sector_fill_things()
else
	-- let's let the user know what went wrong
	UI.LogLine("You need to select 2 or less things.")
	UI.LogLine("")
	UI.LogLine("The first thing will determine the resulting things' properties.")
	UI.LogLine("The angle of both will be used to determine the angles of the new things.")
	UI.LogLine("")
	UI.LogLine("Then those things will fill the selected sectors.")
end