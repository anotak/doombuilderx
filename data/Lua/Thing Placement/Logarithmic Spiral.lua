-- log_spiral_things.lua by anotak

-- places things in logarithmic spirals, see
-- https://en.wikipedia.org/wiki/Logarithmic_spiral

-- let's get the table of selected things
selected_things = Map.GetSelectedThings()

-- check if the number of things is less than or equal to 0
if #selected_things <= 0 then
	UI.LogLine("No things selected! This script won't run without anything selected.")
elseif #selected_things > 600 then
	-- if we have a lot of things selected it might be better idea to not run this.
	-- 600 things * 97 will result in 58200 things, which is, well, plenty
	UI.LogLine(#selected_things .. " things are selected. 600 or less only please.")
	-- if you want to go down a rabbit hole about this kind of thing,
	-- here's a link to get you started:
	-- https://en.wikipedia.org/wiki/Time_complexity
else
	-- otherwise let's go on ahead
	
	-- how big a step to take before we place another thing
	delta_distance = 32

	-- we will loop over all the selected things
	-- for more information about ipairs see https://www.lua.org/pil/7.3.html
	for _, source in ipairs(selected_things) do
		--[[ with that 1 thing, we will repeat this for different angles
			so with our default values of
			angle_offset=(2*math.pi)/6, 2*math.pi, (2*math.pi) / 3
			these angles are in radians.
			that means that our starting point will be at 1/6th of a circle,
			our maximum will be the full circle,
			and we will take steps in 1/3rd of a circle every time
			
			so our 3 spirals will be at the starting angles of
			1/6 of the circle
			1/2 of the circle
			5/6 of the circle
		--]]
		for angle_offset=(2*math.pi)/6, 2*math.pi, (2*math.pi) / 3 do
			-- we'll place 32 things for each spiral
			for i=1,32 do
				-- we're using polar coordinates from the source thing
				-- see https://en.wikipedia.org/wiki/Polar_coordinates
				-- so we have distance & angle based on the log spiral formulas
				distance = delta_distance * i
				angle = math.log(distance / .5) / .5 + angle_offset
				
				-- clone our source thing
				new_thing = source.Clone()
				-- the thing's original position will be the source's, so we
				-- convert our polar coordinates into a cartesian 2d vector using
				-- .FromAngle, and then add the two.
				new_thing.position = new_thing.position + Vector2D.FromAngle(angle, distance)
			end
			-- we placed 32 things
		end
		-- and we did our two spirals
	end
	-- and we did it for all of our selected things
end
-- so we're done!