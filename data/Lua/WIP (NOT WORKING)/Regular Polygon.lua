-- regular_polygon.lua by anotak

local cursor = UI.GetMouseMapPosition(false,false)


r = 32
sides = 6
theta = 0
delta_angle = (2*math.pi) / sides

local start = Vector2D.From(cursor.x, cursor.y)
local p = Pen.From(start + Vector2D.FromAngle(theta, r))
p.snaptogrid = false
p.stitchrange = 1

while sides >= 0 do
	p.DrawVertex()
	theta = theta + delta_angle
	p.position = start + Vector2D.FromAngle(theta, r)
	sides = sides - 1
end

p.FinishPlacingVertices()
