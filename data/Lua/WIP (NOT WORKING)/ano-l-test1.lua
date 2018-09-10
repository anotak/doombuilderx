function hexagon(l,angle,start)
	angle = angle * (math.pi / 180)
	
	--local offsetx = Vector2D.FromAngle(angle, l/2)
	--local offsetx = Vector2D.From(0,0)
	--local perp_angle = angle + (math.pi/3)
	--local offsety = -Vector2D.FromAngle(angle - (math.pi / 2), l * math.sin(perp_angle))
	--[[
	t = Map.InsertThing(start)
	t.type = 5
	
	t = Map.InsertThing(start + offsetx)
	t.type = 23
	
	t = Map.InsertThing(start + offsety)
	t.type = 66
	--]]
	start = start --+ offsetx + offsety
	
	local p = Pen.From(start)
	p.snaptogrid = false
	p.stitchrange = 1
	p.DrawVertex()
	
	p.TurnLeft(angle)
	
	
	
	for i = 1,6 do
		p.TurnRight(math.pi / 3)
		p.MoveForward(l)
		p.DrawVertex()
	end
	
	p.FinishPlacingVertices()
	
	return out
end


for i = 0,9 do
	hexagon(128,30 * i, Vector2D.From(0,0))
end

local sectors = Map.GetSectors()

for _,s in ipairs(sectors) do
	s.brightness = 0
end

for _,s in ipairs(sectors) do
	c = s.GetCenter()
	
	s.brightness = 96 + c.x % 64 + c.y % 64
end

--hexagon(128,15,Vector2D.From(0,0))
--hexagon(128,30,Vector2D.From(0,0))
--hexagon(128,45,Vector2D.From(0,0))

--hexagon(256,0,Vector2D.From(0,0))
--hexagon(128,math.pi / 2,Vector2D.From(128,128))
--hexagon(128,math.pi / 4,Vector2D.From(128,128))