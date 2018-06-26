-- split_and_wiggle.lua by anotak
-- splits all the selected lines and then moves the vertex away from the line
-- in order to create a "wiggle"

function split_and_wiggle(amount)
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
			
			-- TryToMove is a nice function that tries to move our vertex
			-- but only succeeds if we aren't going to run into another vertex
			-- or create overlapping linedefs or other bad geometry
			-- it returns a boolean
			-- lineA.GetNormal() gives us a unit vector perpendicular to lineA
			-- so if we move the vertex in that direction multiplied by the amount
			-- we get neat wiggles
			if not splitvertex.TryToMove( splitvertex.position + (lineA.GetNormal() * amount * 2) ) then
				-- if we couldn't move the vertex, just delete it
				splitvertex.Dispose()
			end
		end
	end
	
	-- if our amount is still big
	if(math.abs(amount / 2) > 4) then
		-- then let's wiggle by half in the other direction!
		split_and_wiggle(amount / -2)
	end
end

-- it doesn't make sense to run with no linedefs selected, so let's let the user know
num_linedefs = #(Map.GetSelectedLinedefs())
if num_linedefs == 0 then
	UI.LogLine("No lines selected!")
end

split_and_wiggle(-16)