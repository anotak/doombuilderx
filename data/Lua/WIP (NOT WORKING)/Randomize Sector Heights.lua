sectors = Map.GetSelectedSectors()

if #sectors == 0 then
	-- FIXME TODO REMOVE
	sectors = Map.GetSectors()
end

if #sectors == 0 then
	UI.LogLine("No sectors selected!")
else
	for i,s in ipairs(sectors) do
		s.floorheight = math.floor(math.random()*16)*8
		s.ceilheight = s.floorheight+math.floor(math.random()*16)*8
	end
end
