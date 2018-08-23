-- koch_snowflake.lua by anotak
-- a little example

-- generally i'd recommend you draw your shapes clockwise
-- sometimes with counterclockwise shapes in the void doom builder just gives up

-- see https://www.eecs.yorku.ca/course_archive/2013-14/W/1030/labs/04/koch.html
-- default delta would be around 60
function koch (pen, w, angle, delta, i)
	if(i <= 0) then
		pen.DrawVertex()
		pen.SetAngleDegrees(angle)
		pen.MoveForward(w)
		pen.DrawVertex()
	else
		koch(pen, w / 3, angle,         delta, i-1)
		koch(pen, w / 3, angle + delta, delta, i-1)
		koch(pen, w / 3, angle - delta, delta, i-1)
		koch(pen, w / 3, angle,         delta, i-1)
	end
end

function full_koch_snowflake(size, angle, x, y, detail, delta_angle)
	local koch_pen = Pen.FromClick()
	
	koch_pen.MoveDiagonal(x,y)
	
	koch_pen.snaptogrid = false
	
	koch(koch_pen, size, 0 + angle, delta_angle, detail)
	koch(koch_pen, size, -120 + angle, delta_angle, detail)
	koch(koch_pen, size, 120 + angle, delta_angle, detail)
	
	koch_pen.FinishPlacingVertices()
end

UI.AddParameter("size", "Size", 1024)

UI.AddParameter("detail", "Detail", 4)

UI.AddParameter("delta_angle", "Delta Angle", 60)

parameters = UI.AskForParameters()

if parameters.detail > parameters.size / 2 then
	UI.LogLine("detail must be less than size / 2")
elseif parameters.detail <= 0 then
	UI.LogLine("detail must be > 0")
else
	full_koch_snowflake(parameters.size, 0, 0, 0, parameters.detail, parameters.delta_angle)
end





