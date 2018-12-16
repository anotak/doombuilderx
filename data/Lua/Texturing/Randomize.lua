-- randomize_textures.lua by anotak
-- randomizes textures and flats
-- does it to selection or to whole map if nothing selected

-- let's initialize our random seed
-- see http://lua-users.org/wiki/MathLibraryTutorial
math.randomseed(os.time())

-- we need to know the textures we have loaded
textures = Data.GetTextureNames()

-- lets get all the selected sidedefs
sidedefs = Map.GetSelectedSidedefs()

-- if we don't have any selected sidedefs, let's just do the whole map
-- FIXME we probably oughta change this behavior
if #sidedefs <= 0 then
	sidedefs = Map.GetSidedefs()
end

-- iterate over all our sidedefs
for i=1, #sidedefs do
	-- on the upper, lower, and mid first we check if there is a texture there already
	-- because we don't want to just set untextured things to textured
	if sidedefs[i].uppertex ~= "-" then
		-- then we set the texture to a random one from the list
		sidedefs[i].uppertex = textures[math.random(#textures)]
	end
	if sidedefs[i].lowertex ~= "-" then
		sidedefs[i].lowertex = textures[math.random(#textures)]
	end
	if sidedefs[i].midtex ~= "-" then
		sidedefs[i].midtex = textures[math.random(#textures)]
	end
end

-- we need to know the flats we have loaded
flats = Data.GetFlatNames()

-- same deal as sides, get the selected list
sectors = Map.GetSelectedSectors()

-- and if it's empty, let's get just all the sectors
if #sectors <= 0 then
	sectors = Map.GetSectors()
end

-- and randomize everything
for i=1, #sectors do
	-- we set the flats to random one from the list
	sectors[i].ceiltex = flats[math.random(#flats)]
	sectors[i].floortex = flats[math.random(#flats)]
end