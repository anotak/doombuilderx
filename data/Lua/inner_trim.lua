-- inner_trim.lua by anotak
-- this is incomplete

function inner_trim(sector)
	local sector_lines = sector.GetLinedefs()
	
	local vector_sum = Vector2D.From(0,0)
	
	-- let's only get the lines that are outer lines
	for _,line in ipairs(sector_lines) do
		local is_line_included = false
		
		if line.IsFlagSet("twosided") then
			local front_index = line.GetFront().GetSector().GetIndex()
			local back_index = line.GetBack().GetSector().GetIndex()
			
			if front_index ~= back_index then
				is_line_included = true
			end
		else
			is_line_included = true
		end
		
		if is_line_included then
			local start_pos = line.start_vertex.position
			local end_pos = line.start_vertex.position
			vector_sum = vector_sum + end_pos.x
		end
	end
end

sectors = Map.GetSelectedSectors()

for _, sector in ipairs(sectors) do
	inner_trim(sector)
end