-- dragon_curve.lua by anotak
-- see https://en.wikipedia.org/wiki/Dragon_curve
-- reasonable default delta_angle is 45 degrees
function dragon_curve(pen, r, i, delta_angle, direction)
	if(i <= 0) then
		pen.DrawVertex()
		pen.MoveForward(r)
	else
		pen.TurnRightDegrees(delta_angle * direction)
		dragon_curve(pen, r/1.4142136, i-1, delta_angle, 1)
		pen.TurnLeftDegrees(delta_angle * 2 * direction)
		dragon_curve(pen, r/1.4142136, i-1, delta_angle, -1)
		pen.TurnRightDegrees(delta_angle * direction)
	end
end

local length = 2048

dragon_pen = Pen.FromClick()
dragon_pen.stitchrange = 30
dragon_pen.snaptogrid = false
dragon_pen.TurnRightDegrees(2)
dragon_curve(dragon_pen, length, 10, -45, 1)
dragon_pen.DrawVertex()
dragon_pen.FinishPlacingVertices()

dragon_pen = Pen.FromClick()
dragon_pen.stitchrange = 30
dragon_pen.snaptogrid = false
dragon_pen.TurnRightDegrees(94)
dragon_curve(dragon_pen, length, 10, 45, -1)
dragon_pen.DrawVertex()
dragon_pen.FinishPlacingVertices()

dragon_pen = Pen.FromClick()
dragon_pen.stitchrange = 30
dragon_pen.snaptogrid = false
dragon_pen.TurnRightDegrees(186)
dragon_curve(dragon_pen, length, 10, -45, 1)
dragon_pen.DrawVertex()
dragon_pen.FinishPlacingVertices()

dragon_pen = Pen.FromClick()
dragon_pen.stitchrange = 30
dragon_pen.snaptogrid = false
dragon_pen.TurnRightDegrees(278)
dragon_curve(dragon_pen, length, 10, 45, -1)
dragon_pen.DrawVertex()
dragon_pen.FinishPlacingVertices()
