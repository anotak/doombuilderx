-- split twist.lua by anotak


function split_twist(angle, iterations_left)
	if iterations_left <= 0 then
		return
	end
	
	-- if our angle is big enough
	if math.abs(angle) < math.pi/360 then
		return
	end
	
	-- get the selected linedefs on the map
	-- note: we need to call this again every time we start this function
	-- over specifically because we created new selected linedefs by
	-- splitting.
	local linedefs = Map.GetSelectedLinedefs()
	
	-- iterate over the list of linedefs
	for i=1, #linedefs do
		-- don't bother splitting linedefs shorter than 6 map units
		if linedefs[i].GetLength() > 6 then
			-- split returns splitting vertex, and the two resulting lines
			local splitvertex, lineA, lineB = linedefs[i].Split()
			
			local sp = lineA.end_vertex.position
			local ep = lineB.start_vertex.position
			local delta = ep - sp
			local length = delta.GetLength()
			local d_angle = delta.GetAngle()
			
			local new_ep = sp + Vector2D.FromAngle(d_angle + angle, length)
			
			-- TryToMove is a nice function that tries to move our vertex
			-- but only succeeds if we aren't going to run into another vertex
			-- or create overlapping linedefs or other bad geometry
			-- it returns a boolean
			-- lineA.GetNormal() gives us a unit vector perpendicular to lineA
			-- so if we move the vertex in that direction multiplied by the amount
			-- we get neat wiggles
			if not lineB.start_vertex.TryToMove( new_ep ) then
				-- if we couldn't move the vertex, just delete it
				splitvertex.Dispose()
			end
		end
	end
	
	-- then let's split twist by half in the other direction!
	split_twist(angle / -2, iterations_left - 1)
end

-- it doesn't make sense to run with no linedefs selected, so let's let the user know
num_linedefs = #(Map.GetSelectedLinedefs())
if num_linedefs == 0 then
	UI.LogLine("No lines selected. You need to select some lines.")
else
	UI.AddParameter("angle", "angle", -3)
	UI.AddParameter("max_iterations", "Maximum iterations (negative or 0 is effectively no limit)", 1)

	parameters = UI.AskForParameters()

	if parameters.max_iterations <= 0 then
		parameters.max_iterations = 65536
	end

	split_twist(parameters.angle * math.pi / 180, parameters.max_iterations)
end