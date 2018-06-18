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

function full_koch_snowflake(size, angle, x, y, detail)
	local koch_pen = Pen.FromClick()
	
	koch_pen.MoveDiagonal(x,y)
	
	koch_pen.snaptogrid = false
	
	koch(koch_pen, size, 0 + angle, 60, detail)
	koch(koch_pen, size, -120 + angle, 60, detail)
	koch(koch_pen, size, 120 + angle, 60, detail)
	
	koch_pen.FinishPlacingVertices()
end

full_koch_snowflake(1024, 0, 0, 0, 4)
full_koch_snowflake(512, 0, -144, -256, 3)
full_koch_snowflake(256, 0, -216, -384, 3)
full_koch_snowflake(128, 0, -253, -448, 3)
sectors = Map.GetSectors()

for i=1, #sectors do
	if i % 2 == 0 then
		sectors[i].floortex = "FWATER1"
	end
	if i % 3 == 1 then
		sectors[i].ceiltex = "F_SKY1"
	end
	if i % 3 == 2 then
		sectors[i].ceiltex = "FLAT1"
	end
end