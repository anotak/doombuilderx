-- regular_polygon.lua by anotak

local cursor = UI.GetMouseMapPosition(false,false)

UI.AddParameter("size", "size", 32, "how big is the polygon")
UI.AddParameter("angle", "angle", 0, "what angle to start the polygon at")
UI.AddParameter("sides", "# of sides", 6, "how many sides do you want your polygon to have")

parameters = UI.AskForParameters()

r = parameters.size
sides = parameters.sides
theta = parameters.angle
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
