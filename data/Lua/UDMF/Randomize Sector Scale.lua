-- randomize_sector_scale.lua by anotak
-- randomize the 

math.randomseed(os.time());
sectors = Map.GetSelectedSectors()

if #sectors == 0 then
	sectors = Map.GetSectors()
end

for _, sector in ipairs(sectors) do
	xscalef = math.random() * 3 + 0.01
	yscalef = math.random() * 3 + 0.01
	sector.SetUDMFField("xscalefloor",xscalef)
	sector.SetUDMFField("yscalefloor",yscalef)
	xscalec = math.random() * 3 + 0.01
	yscalec = math.random() * 3 + 0.01
	sector.SetUDMFField("xscaleceiling",xscalec)
	sector.SetUDMFField("yscaleceiling",yscalec)
end