-- see https://en.wikipedia.org/wiki/Pinwheel_tiling

s5 = math.sqrt(5)

-- p_r is the one with the right angle
-- l_a == 1, l_b == 2, l_c == sqrt 5
function pinwheel_tile(l_a, l_b, l_c, iterations)
	iterations = iterations - 1
	
	if iterations < 0 then
		--[[
		
		
		t1 = Map.InsertThing(l_c.GetV1())
		t2 = Map.InsertThing(l_c.GetV2())
		
		t1.type = 66
		t2.type = 66
		
		t1 = Map.InsertThing(l_b.GetV1())
		t2 = Map.InsertThing(l_b.GetV2())
		
		t1.type = 77
		t2.type = 77
		
		t1 = Map.InsertThing(l_a.GetV1())
		t2 = Map.InsertThing(l_a.GetV2())
		
		t1.type = 99
		t2.type = 99
		--]]
		
		--[[
		t1 = Map.InsertThing(l_c.GetV1())
		t2 = Map.InsertThing(l_b.GetV1())
		t3 = Map.InsertThing(l_a.GetV1())
		
		t1.type = 1
		t2.type = 66
		t3.type = 99
		--]]
	
		p = Pen.From(l_c.GetV2())
		p.snaptogrid = false
		p.stitchrange = 1
		p.DrawVertex()
		p.position = l_c.GetV1()
		p.DrawVertex()
		p.position = l_b.GetV1()
		p.DrawVertex()
		p.position = l_a.GetV1()
		p.DrawVertex()
		p.FinishPlacingVertices()
		return
	end
	local a1 = l_b.GetCoordinatesAt(.5)
	local a2 = l_c.GetCoordinatesAt(2/5)
	local a3 = l_c.GetCoordinatesAt(4/5)
	local a4 = Line2D.From(a3, l_a.GetV2()).GetCoordinatesAt(.5)
	
	--[[
	Map.InsertThing(l_a.GetV1())
	Map.InsertThing(l_b.GetV1())
	
	--]]
	
	
	pinwheel_tile(Line2D.From(a1,a2) , Line2D.From(a2,a3), Line2D.From(a3,a1), iterations)
	
	pinwheel_tile(Line2D.From(a2,a1), Line2D.From(a1,l_c.GetV1()), Line2D.From(l_c.GetV1(),a2), iterations)
	
	pinwheel_tile(Line2D.From(a3,a4) , Line2D.From(a4,a1), Line2D.From(a1,a3), iterations)
	
	pinwheel_tile(Line2D.From(a4,l_b.GetV1()) , Line2D.From(l_b.GetV1(),a1), Line2D.From(a1,a4), iterations)
	
	pinwheel_tile(Line2D.From(a3,l_c.GetV2()) , Line2D.From(l_c.GetV2(),l_b.GetV1()), Line2D.From(l_b.GetV1(),a3), iterations)
end

v1 = Vector2D.From(0, 512)
v2 = Vector2D.From(1024,0)
v3 = Vector2D.From(0,0)
pinwheel_tile(Line2D.From(v1,v3), Line2D.From(v3, v2), Line2D.From(v2, v1), 3)