sectors = Map.GetSelectedSectors()

if #sectors == 0 then
	-- FIXME TODO REMOVE
	sectors = Map.GetSectors()
end

if #sectors == 0 then
	UI.LogLine("No sectors selected!")
else
	for i,s in ipairs(sectors) do
		s.floorheight = math.min(s.ceilheight, s.floorheight + math.floor((math.random()-1.0)*4)*8)
		s.ceilheight = math.max(s.floorheight, s.ceilheight + math.floor((math.random()-1.0)*4)*8)
	end
end
