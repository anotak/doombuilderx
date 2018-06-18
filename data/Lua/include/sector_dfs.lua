-- sector_dfs.lua by anotak
-- this is a small lua library for dbx for selecting sectors via depth-first search

-- for more information on dfs, see https://en.wikipedia.org/wiki/Depth-first_search
-- methods for use to the public are:

-- sector_dfs.select_sectors(starting_sector, condition)
--[[ starting at starting_sector, we search through all sectors
	connected by sidedefs.
	if condition is not nil, then we check the condition first
	note that you might need to deselect geometry first
--]]

-- sector_dfs.select_sectors_from_mouse(condition)
--[[
	calls sector_dfs.select_sectors at the mouse position
	condition is optional, as sector_dfs.select_sectors
--]]




-- keep track of the searched sectors and linedefs
-- so that we don't recheck stuff (for speed reasons)
sector_dfs = {}
sector_dfs.searched_sectors = {}
sector_dfs.searched_lines = {}

function sector_dfs.select_sectors(starting_sector, condition)
	if starting_sector == nil then
		return
	end
	
	sector_dfs.searched_sectors = {}
	sector_dfs.searched_lines = {}
	
	if condition == nil then
		
		return sector_dfs.select_sectors_recursive(starting_sector)
	else
		return sector_dfs.select_sectors_recursive_condition(starting_sector, condition)
	end
end

function sector_dfs.select_sectors_from_mouse(condition)
	-- well, we need to know where we clicked
	-- the first false means no snap to grid
	-- the second means no snapping to geometry
	local cursor = UI.GetMouseMapPosition(false, false)

	-- find sector exactly at mouse
	local start_sector = Map.DetermineSector(cursor)

	-- if nothing at current exact mouse position, let's try to just find what's nearby
	if start_sector == nil then
		start_sector = Map.NearestSector(cursor)
	end
	
	-- start at the mouse click point, and run our function
	sector_dfs.select_sectors(start_sector, condition)
end

function sector_dfs.select_sectors_recursive(current_sector)
	-- check if the current sector hasn't been searched already
	if sector_dfs.searched_sectors[current_sector.GetIndex()] == nil then
		-- mark it as searched
		sector_dfs.searched_sectors[current_sector.GetIndex()] = true
		
		-- select it
		current_sector.selected = true
		
		-- let's go find our neighbors through our sidedefs
		local sidedefs = current_sector.GetSidedefs()
		
		-- for each sidedef
		for _, current_side in ipairs(sidedefs) do
			-- let's make sure we haven't checked this line already
			local line = current_side.GetLinedef()
			if sector_dfs.searched_lines[line.GetIndex()] == nil then
				-- mark as checked
				sector_dfs.searched_lines[line.GetIndex()] = true
				
				-- select the line, because it's technically possible to select
				-- a sector without selecting its lines in doom builder, as
				-- weird as that might seem
				line.selected = true
				
				-- then find the sector on the other side,
				-- and find_sectors on that if there is another side
				local other = current_side.GetOther()
				
				-- make sure that other exists
				if other ~= nil then
					-- note that othersector can't be nil if other exists
					-- by definition, otherwise we have a larger bug
					-- in doom builder itself and maybe we should just
					-- crash in that case
					local othersector = other.GetSector()
					sector_dfs.select_sectors_recursive(othersector)
				end
			end
		end
	end
end

-- use a condition to filter sectors. the condition is a function
function sector_dfs.select_sectors_recursive_condition(current_sector, condition)
	-- check if the current sector hasn't been searched already
	if sector_dfs.searched_sectors[current_sector.GetIndex()] == nil then
		-- mark it as searched
		sector_dfs.searched_sectors[current_sector.GetIndex()] = true
		
		-- select it
		current_sector.selected = true
		
		-- let's go find our neighbors through our sidedefs
		local sidedefs = current_sector.GetSidedefs()
		
		-- for each sidedef
		for _, current_side in ipairs(sidedefs) do
			-- let's make sure we haven't checked this line already
			line = current_side.GetLinedef()
			if sector_dfs.searched_lines[line.GetIndex()] == nil then
				-- mark as checked
				sector_dfs.searched_lines[line.GetIndex()] = true
				
				-- select the line, because it's technically possible to select
				-- a sector without selecting its lines in doom builder, as
				-- weird as that might seem
				line.selected = true
				
				-- then find the sector on the other side,
				-- and find_sectors on that if there is another side
				local other = current_side.GetOther()
				
				-- make sure that other exists
				if other ~= nil then
					-- note that othersector can't be nil if other exists
					-- by definition, otherwise we have a larger bug
					-- in doom builder itself and maybe we should just
					-- crash in that case
					local othersector = other.GetSector()
					-- check our condition
					if condition(othersector, current_sector) then
						sector_dfs.select_sectors_recursive_condition(othersector, condition)
					end
				end
			end
		end
	end
end