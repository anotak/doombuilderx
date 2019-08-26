sectors = Map.GetSelectedSectors()

if #sectors == 0 then
	UI.LogLine("You need to select some sectors.")
	return
end

UI.AddParameter("height", "height = ", "math.floor(math.random()*16)*8", "formula for sector height")

parameters = UI.AskForParameters()
local heightf = dostring("function f_height(t,s)\n return " .. parameters.height .. "\nend\n return f_height")

if #sectors == 0 then
	UI.LogLine("No sectors selected!")
else
	for i,s in ipairs(sectors) do
		s.floorheight = math.floor(heightf(i, s))
	end
end
