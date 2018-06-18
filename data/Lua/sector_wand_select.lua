-- sector_wand_select.lua by anotak
-- selects all sectors that are connected somehow to the sector where you clicked

-- deselect sectors/lines/vertices
Map.DeselectGeometry()

-- we're going to be using the sector_dfs.select_sectors function, so we need to include sector_dfs.lua
-- note that the dofile in DBXLua is limited to only certain relative paths
-- (see the wiki for more information)
dofile("include/sector_dfs.lua")

-- start at the mouse click point, and select all sectors
sector_dfs.select_sectors_from_mouse()