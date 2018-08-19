function flank_linedefs(amount)
	local linedefs = Map.GetSelectedLinedefs()
	
	if amount == 1 then
		return
	end
	local our_stitchrange = math.min(amount - 2, 10)
	our_stitchrange = math.max(1, our_stitchrange)
	for i=1, #linedefs do
		if linedefs[i].GetLength() > 2 * amount then

			local myLen = linedefs[i].GetLength()
			local p = Pen.From(linedefs[i].start_vertex.position)
			p.snaptogrid = false
			p.stitchrange = our_stitchrange
			p.DrawVertex()
			p.angle = linedefs[i].GetAngle()
			Map.InsertThing(p.position)
			--UI.LogLine(tostring(linedefs[i].GetAngleDegrees()) .. " " .. tostring(myLen))
			p.MoveForward(amount)
			p.DrawVertex()
			--Map.InsertThing(p.position)
			p.MoveForward(myLen - 2 * amount)
			p.DrawVertex()
			--Map.InsertThing(p.position)
			p.MoveForward(amount)
			p.DrawVertex()
			--Map.InsertThing(p.position)
			p.FinishPlacingVertices()

		end
	end
end

-- it doesn't make sense to run with no linedefs selected, so let's let the user know
num_linedefs = #(Map.GetSelectedLinedefs())
if num_linedefs == 0 then
	UI.LogLine("No lines selected!")
end

flank_linedefs(32)