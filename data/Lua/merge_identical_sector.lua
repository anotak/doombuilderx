-- doesnt work atm

-- let's get the table of selected sectors
sectors = Map.GetSelectedSectors()

-- if no sectors are selected, let's use where the user clicked
if #sectors <= 0 then
	-- well, we need to know where we clicked
	-- the first false means no snap to grid
	-- the second means no snapping to geometry
	local cursor = UI.GetMouseMapPosition(false, false)

	-- find sector exactly at mouse
	sectors[1] = Map.DetermineSector(cursor)

	-- if nothing at current exact mouse position, let's try to just find what's nearby
	if sectors[1] == nil then
		sectors[1] = Map.NearestSector(cursor)
	end
	
	-- now lets get the neighbors of the sector
	
	local sector_sidedefs = sectors[1].GetSidedefs()
	
	
	for _,side in ipairs(sector_sidedefs) do
		local other = side.GetOther()
		
		-- make sure that other exists
		if other ~= nil then
			-- note that othersector can't be nil if other exists
			-- by definition, otherwise we have a larger bug
			-- in doom builder itself and maybe we should just
			-- crash in that case
			local othersector = other.GetSector()
			
			if othersector.GetIndex() ~= sectors[1].GetIndex() then
				sectors[#sectors + 1] = othersector
			end
		end
	end
end

-- we're going to be keeping track of extra info per sector
-- this will make sense more in a second
info_table = {}

for _,sector in ipairs(sectors) do
	local index = sector.GetIndex()
	
	info_table[index] = {}
	
	info_table[index].sector = sector
	-- we need to remember our index because indices will get moved around later during merging
	info_table[index].index = index
	info_table[index].neighbors = {}
	
	local sector_sidedefs = sector.GetSidedefs()
	
	for _,side in ipairs(sector_sidedefs) do
		local other = side.GetOther()
		
		-- make sure that other exists
		if other ~= nil then
			-- note that othersector can't be nil if other exists
			-- by definition, otherwise we have a larger bug
			-- in doom builder itself and maybe we should just
			-- crash in that case
			local othersector = other.GetSector()
			local otherindex = othersector.GetIndex()
			
			if otherindex ~= index then
				-- this removes duplicates for the later loop, handily
				-- we also need to make sure that we aren't deleting a
				-- cage wall or something
				-- and also that we aren't deleting a walkover line
				local line = side.GetLinedef()
				if
					side.midtex == "-"
					and other.midtex == "-"
					and line.action == 0
					and not line.IsFlagSet("blocking")
					and not line.IsFlagSet("blockmonsters")
					and not line.IsFlagSet("blocksound")
				then
					if info_table[index].neighbors[otherindex] == nil then
						info_table[index].neighbors[otherindex] = otherindex
					end
				else
					info_table[index].neighbors[otherindex] = -1
				end
			end
		end
		-- we're done with this sidedef
	end
	
	-- we're done with this sector
end

-- set up the comparison so we know whether to merge or not
function check_merge(a,b)

	UI.LogLine(a.ToString() .. " vs " .. b.ToString())
	if a.floorheight ~= b.floorheight then
		return false
	end
	if a.ceilheight ~= b.ceilheight then
		return false
	end
	if a.floortex ~= b.floortex then
		return false
	end
	if a.ceiltex ~= b.ceiltex then
		return false
	end
	if a.tag ~= b.tag then
		return false
	end
	if a.brightness ~= b.brightness then
		return false
	end
	if a.effect ~= b.effect then
		return false
	end
	-- if we need to worry about UDMF, then worry about it
	if MapFormat.HasCustomFields() then
		local a_table = a.GetUDMFTable()
		local b_table = b.GetUDMFTable()
		
		if #a_table ~= #b_table then
			return false
		end
		
		for key,value in pairs(a_table) do
			if b_table[key] ~= value then
				return false
			end
		end
	end
	return true
end

join_sectors = {}
searched_sectors = {}

function dfs_recursive(current_sector)
	-- check if the current sector hasn't been searched already
	if searched_sectors[current_sector.index] == nil then
		-- mark it as searched
		searched_sectors[current_sector.index] = true
		
		join_sectors[#join_sectors + 1] = current_sector.sector
		
		-- let's look at our neighbors
		for n_index, neighbor in pairs(current_sector.neighbors) do
			-- let's make sure we haven't checked this neighbor already
			
			if
				searched_sectors[neighbor] == nil
				and neighbors ~= -1
				and check_merge(current_sector.sector, info_table[neighbor].sector)
			then
				select_sectors
			end
			-- done with this neighbor
		end
		-- done with this sector
	end
end


--[[
-- trash this whole approach, instead DFS from each current_info
for key,current_info in pairs(info_table) do
	if not current_info.sector.IsDisposed() then
		local join_group_sectors = { current_info.sector }
		local join_group_indices = { current_info.index }
		
		-- add all our valid neighbors to the list to be merged
		for other_key,other_index in pairs(current_info.neighbors) do
			if
				other_index > -1
				and info_table[other_index] ~= nil
				--and not info_table[other_index].sector.IsDisposed()
				and check_merge(current_info.sector, info_table[other_index].sector)
			then
				join_group_sectors[#join_group_sectors + 1] = info_table[other_index].sector
				join_group_indices[#join_group_indices + 1] = other_index
			end
		end
		
		info_table[key].sector = Map.MergeSectors(join_group_sectors)
		info_table[key].sector.selected = false
		
		for i = 1, #join_group_sectors do
			info_table[join_group_indices[i] ].index = info_table[key].index
			info_table[join_group_indices[i] ].sector = info_table[key].sector
			
			-- inform our merge's neighbors that we exist
			for _,join_neighbor in pairs(info_table[join_group_indices[i] ].neighbors) do
				-- if we rejected previously with -1, then keep rejecting
				if
					join_neighbor ~= -1
					and info_table[join_neighbor] ~= nil
					and (info_table[join_neighbor].neighbors[key] == nil
						or info_table[join_neighbor].neighbors[key] ~= -1)
				then
					info_table[join_neighbor].neighbors[key] = info_table[key].index
					info_table[key] = 
				end
			end
				
		end
		
		
	end
end
	--]]