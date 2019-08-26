
-- TODO probably provide for this in the API somehow?
epsilon = MapFormat.GetMinLineLength() / 2
function floating_point_equality(a, b)
	-- epsilon number to compare vertices by for floating point imprecision
	-- see https://en.wikipedia.org/wiki/Floating_point#Accuracy_problems
	return math.abs(a - b) <= epsilon
end

vertices = Map.GetSelectedVertices()
if #vertices < 3 then
	UI.LogLine("You must select 3 or more vertices.")
else
	-- create the bounding box
	local old_xmin = MapFormat.GetRightBoundary()
	local old_xmax = MapFormat.GetLeftBoundary()
	local old_ymin = MapFormat.GetTopBoundary()
	local old_ymax = MapFormat.GetBottomBoundary()
	
	for _,v in ipairs(vertices) do
		old_xmin = math.min(old_xmin, v.position.x)
		old_xmax = math.max(old_xmax, v.position.x)
		old_ymin = math.min(old_ymin, v.position.y)
		old_ymax = math.max(old_ymax, v.position.y)
	end
	
	-- start and end points
	local sp
	local ep
	
	-- FIXME annoying edge case of when line has small axis aligned hook at the end perpendicular to detected axis can make it find the wrong endpoint
	
	-- see which axis-aligned direction is longer to pick our endpoints
	if old_xmax - old_xmin > old_ymax - old_ymin then
		-- more horizontal
		for _,v in ipairs(vertices) do
			if floating_point_equality(v.position.x,old_xmin) then
				sp = v
			elseif floating_point_equality(v.position.x, old_xmax) then
				ep = v
			end
		end
	else
		-- more vertical
		for _,v in ipairs(vertices) do
			if floating_point_equality(v.position.y,old_ymin) then
				sp = v
			elseif floating_point_equality(v.position.y, old_ymax) then
				ep = v
			end
		end
	end
	
	local l2d = Line2D.From(sp.position,ep.position)
	
	-- FIXME  this method tends to create lines that overlap backwards with itself
	
	for _,v in ipairs(vertices) do
		if v.GetIndex() != ep.GetIndex() and v.GetIndex() != sp.GetIndex() then
			-- TODO API FUNCTION NEEDS TO BE BETTER, HANDLE THIS THERE
			local u = l2d.GetNearestOnLine(v.position)
			if u < 0 then
				u = 0
			elseif u > 1 then
				u = 1
			end
			--v.position = l2d.GetCoordinatesAt(u)
			v.TryToMove(l2d.GetCoordinatesAt(u))
		end
	end
	
end