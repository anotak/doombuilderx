-- UDMF/Search.lua by anotak

Map.DeselectGeometry()

UI.AddParameter("key", "UDMF Key", "", "The key to search for")
UI.AddParameter("value", "UDMF Value", "", "The value to search for")

p = UI.AskForParameters()

function check_map_elements(etable)
	for _, e in ipairs(etable) do
		if e.GetUDMFField(p.key) == p.value or e.GetUDMFField(p.key) == tonumber(p.value) then
			e.selected = true
		end
	end
end

sectors = Map.GetSectors()
check_map_elements(sectors)

linedefs = Map.GetLinedefs()
check_map_elements(linedefs)

vertices = Map.GetVertices()
check_map_elements(vertices)

things = Map.GetThings()
check_map_elements(things)