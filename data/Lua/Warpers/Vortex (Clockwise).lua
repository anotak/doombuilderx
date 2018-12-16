-- we're going to be using the vortex.vortex() function, so we need to include sector_dfs.lua
-- note that the dofile in DBXLua is limited to only certain relative paths
-- (see the wiki for more information)
dofile("include/Vortex.lua")

-- to warp vertices toward mouse position,
-- well, we need the mouse position in map coordinates
cursor = UI.GetMouseMapPosition(false,false)

-- 64 is max distance
-- 22.5 degrees, clockwise
-- at mouse cursor
vortex.do_vortex(cursor, 64, (-math.pi) / 8)

