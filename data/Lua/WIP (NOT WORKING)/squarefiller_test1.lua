
cursor = UI.GetMouseMapPosition(true,true)
-- let's draw a little simple 32x32 square

for x=-32, 32 do
	for y=-32, 32 do
		location = Vector2D.From(x * 32, y *32)
		
		-- first we need to get some pen
		p = Pen.From(cursor + location)
		-- let's turn off snap to grid after the fact, even though our initial first position in the FromClick was snapped
		p.snaptogrid = false
		p.stitchrange = 1
		p.MoveForward(0.01)
		--p.position = Vector2D.From(0,0)
		-- draw our first vertex
		p.DrawVertex()

		p.MoveForward(32)
		-- finish up the first side
		p.DrawVertex()

		-- now to do the 2nd side
		p.TurnRightDegrees(90)
		p.MoveForward(32)
		p.DrawVertex()

		-- 3rd
		p.TurnRight()
		p.MoveForward(32)
		p.DrawVertex()

		-- and 4th!
		-- we can also turn with an angle in radians
		p.TurnRight(math.pi/2)
		p.MoveForward(32)
		p.DrawVertex()

		-- most importantly, we have to do this, to draw anything
		p.FinishPlacingVertices()
	end
end