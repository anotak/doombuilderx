-- sector_wand_select_height.lua by anotak
-- selects all sectors that are connected somehow to the sector where you clicked
-- and also have the same floor height

-- deselect sectors/lines/vertices
Map.DeselectGeometry()

-- we're going to be using the sector_dfs.select_sectors function, so we need to include sector_dfs.lua
-- note that the dofile in DBXLua is limited to only certain relative paths
-- (see the wiki for more information)
dofile("include/sector_dfs.lua")

-- our little comparator function to only select sectors with the same floor height
function sector_condition_function(next_sector, prev_sector)
	return next_sector.floorheight == prev_sector.floorheight
end

-- start at the mouse click point, and select all sectors matching our condition
sector_dfs.select_sectors_from_mouse(sector_condition_function)