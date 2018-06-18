-- sector-wand-select.lua by anotak
-- selects all contiguous sectors nearest to where you clicked

-- this script is an implementation of a depth-first search algorithm
-- for more information, see https://en.wikipedia.org/wiki/Depth-first_search

-- deselect sectors/lines/vertices
Map.DeselectGeometry()

-- well, we need to know where we clicked
cursor = UI.GetMouseMapPosition()

-- keep track of the searched sectors and linedefs
-- so that we don't recheck stuff (for speed reasons)
searched_sectors = {}
searched_lines = {}

-- we will set up this recursive function and then call it after
function find_sectors(current_sector)
	-- check if the current sector hasn't been searched already
	if searched_sectors[current_sector.GetIndex()] == nil then
		-- mark it as searched
		searched_sectors[current_sector.GetIndex()] = true
		
		-- select it
		current_sector.selected = true
		
		-- let's go find our neighbors through our sidedefs
		sidedefs = current_sector.GetSidedefs()
		
		-- for each sidedef
		for _, current_side in ipairs(sidedefs) do
			-- let's make sure we haven't checked this line already
			line = current_side.GetLinedef()
			if searched_lines[line.GetIndex()] == nil then
				-- mark as checked
				searched_lines[line.GetIndex()] = true
				
				-- select the line, because it's technically possible to select
				-- a sector without selecting its lines in doom builder, as
				-- weird as that might seem
				line.selected = true
				
				-- then find the sector on the other side,
				-- and find_sectors on that if there is another side
				other = current_side.GetOther()
				
				-- make sure that other exists
				if other ~= nil then
					-- note that othersector can't be nil if other exists
					-- by definition, otherwise we have a larger bug
					-- in doom builder itself and maybe we should just
					-- crash in that case
					othersector = other.GetSector()
					find_sectors(othersector)
				end
			end
		end
	end
end

-- start at the mouse click point, and run our function
find_sectors(Map.NearestSector(cursor))