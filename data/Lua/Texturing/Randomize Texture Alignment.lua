
-- lets get all the selected sidedefs
sidedefs = Map.GetSelectedSidedefs()

-- if we don't have any selected sidedefs, let's just do the whole map
if #sidedefs <= 0 then
	return
end

-- iterate over all our sidedefs
for i=1, #sidedefs do
	-- on the upper, lower, and mid first we check if there is a texture there already
	if sidedefs[i].uppertex ~= "-" or sidedefs[i].lowertex ~= "-" or sidedefs[i].midtex ~= "-" then
		sidedefs[i].offsetx = math.random() * 512
		sidedefs[i].offsety = math.random() * 512
	end
end