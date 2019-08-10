linedefs = Map.GetSelectedLinedefs()

if #linedefs == 2 then
	linedefs[1].SwapProperties(linedefs[2])
end