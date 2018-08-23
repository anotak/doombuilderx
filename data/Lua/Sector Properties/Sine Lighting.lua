sectors = Map.GetSelectedSectors()
cursor = UI.GetMouseMapPosition(false,false)

-- see http://lua-users.org/wiki/StringRecipes
local function starts_with(str, start)
   return str:sub(1, #start) == start
end

if #sectors == 0 then
	-- FIXME TODO REMOVE
	sectors = Map.GetSectors()
end

if #sectors == 0 then
	UI.LogLine("No sectors selected!")
else
	UI.AddParameter("xfreq", "xFreq", 1)
	UI.AddParameter("yfreq", "yFreq", 1)
	UI.AddParameter("xybalance", "xybalance (0 to 1)", 0.5)
	UI.AddParameter("angle", "angle (degrees)", 0)
	UI.AddParameter("anglemul", "curvature (1 = none)", 1)
	UI.AddParameter("linear", "linear (positive = brighter farther from click)", 0)
	UI.AddParameter("dist_slope", "QUADRATIC MAGNETISM", 0)
	UI.AddParameter("minlight", "min", 96)
	UI.AddParameter("maxlight", "max", 255)
	UI.AddParameter("blendmode", "blend mode (add, mult, sub, max, min or replace)", "replace")
	-- TODO warn negative or > 255 light
	--
	
	p = UI.AskForParameters()
	p.xfreq = p.xfreq * 0.01
	p.yfreq = p.yfreq * 0.01
	p.dist_slope = p.dist_slope * 0.001
	p.xybalance = math.max(p.xybalance, 0)
	p.xybalance = math.min(p.xybalance, 1)
	p.angle = p.angle * (math.pi / 180) -- convert to radians
	
	range = p.maxlight - p.minlight
	
	p.blendmode = string.lower(p.blendmode)
	
	blendmode = 0
	if starts_with(p.blendmode, "add") then
		blendmode = 1
	elseif starts_with(p.blendmode, "sub")  then
		blendmode = 2
	elseif starts_with(p.blendmode, "mul")  then
		blendmode = 3
	elseif starts_with(p.blendmode, "min")  then
		blendmode = 4
	elseif starts_with(p.blendmode, "max")  then
		blendmode = 5
	end
	
	for i,s in ipairs(sectors) do
		local center = s.GetCenter() - cursor
		local length = center.GetLength()
		
		center = Vector2D.FromAngle(
						(center.GetAngle() * p.anglemul) + p.angle,
						(length + (length * length * p.dist_slope))
						)
		
		
		local wx = (math.cos(center.x * p.xfreq) + 1) * (1 - p.xybalance)
		local wy = (math.sin(center.y * p.yfreq) + 1) * p.xybalance
		
		local wave = (wx + wy) * .5
		
		wave = wave + (length * (p.linear * 0.001))
		
		wave = math.max(0, wave)
		wave = math.min(1, wave)
		--[[
		local new_floor = (wave * range) + p.minlight
		
		local old_floor = s.floorheight
		
		if blendmode == 3 then
			s.floorheight = (old_floor / 256) * (new_floor / 256) * 256
		elseif blendmode == 2 then
			s.floorheight = old_floor - new_floor
		elseif blendmode == 1 then
			s.floorheight = old_floor +  new_floor
		else
			s.floorheight =  new_floor
		end
		--]]
		
		
		local new_bright = (wave * range) + p.minlight
		
		new_bright = math.min(256, new_bright)
		new_bright = math.max(0, new_bright)
		
		local old_bright = math.min(256, s.brightness)
		old_bright = math.max(0, old_bright)
		
		
		
		if blendmode == 5 then
			s.brightness = math.max(old_bright,new_bright)
		elseif blendmode == 4 then
			s.brightness = math.min(old_bright,new_bright)
		elseif blendmode == 3 then
			s.brightness = (old_bright / 256) * (new_bright / 256) * 256
		elseif blendmode == 2 then
			s.brightness = old_bright - new_bright
		elseif blendmode == 1 then
			s.brightness = old_bright +  new_bright
		else
			s.brightness =  new_bright
		end
		
		-- TODO properly quantize lighting to 16s like doom???
		
		--UI.LogLine(s.floorheight)
	end
end
