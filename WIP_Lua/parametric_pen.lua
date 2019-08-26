local cursor = UI.GetMouseMapPosition(false,false)

local p = Pen.From(0,0)
p.stitchrange = 1
p.snaptogrid = false

UI.AddParameter("xf", "x = ", "math.sin(t) * 128", "formula for x by t")
UI.AddParameter("yf", "y = ", "math.cos(t) * 128", "formula for x by t")
UI.AddParameter("stepsize", "t step size", .1, "how often to sample the formula")
UI.AddParameter("count", "# points", 64, "how many points to sample at")

parameters = UI.AskForParameters()
local xf
local yf
xf = dostring("function f_ffx(t)\n return " .. parameters.xf .. "\nend\n return f_ffx")
yf = dostring("function f_ffy(t)\n return " .. parameters.yf .. "\nend\n return f_ffy")


local t = 0
for i=1,parameters.count do
	local x = xf(t)
	local y = yf(t)
	p.position = Vector2D.From(x,y)
	p.position = p.position + cursor
	--Map.InsertThing(x,y)
	p.DrawVertex()
	t = t + parameters.stepsize
end

p.FinishPlacingVertices()