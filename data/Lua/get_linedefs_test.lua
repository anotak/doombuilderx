-- get_linedefs_test.lua
-- um this doesnt entirely work right right now but try clicking on some sectors with it anyway
-- by anotak
Map.DeselectGeometry()

function finish_at_line(new_line)
	if new_line.GetX1() ~= new_line.GetX2 then
		local p = Pen.From(new_line.GetV1())
		p.stitchrange = 2
		p.snaptogrid = false
		p.DrawVertex()
		
		p.position = new_line.GetV2()
		p.DrawVertex()
		
		--UI.LogLine(new_line.ToString())
		p.FinishPlacingVertices()
	end
end

function move_past(xmin, y, index, lines)
	index = index + 1
	
	while index <= #lines do		
		local y1 = lines[index].GetY1()
		local y2 = lines[index].GetY2()
		
		if
			y > math.max(y1,y2)
			or y < math.min(y1,y2)
			or lines[index].GetX2() < xmin
		then
			index = index + 1
		else
			return index
		end
	end
	
	return -1
end

function do_row(y, xmin, xmax, start_lines, end_lines)
	-- we find the leftmost start line within our range
	
	-- not 1 because move_past increments it, then move it right past ones outside our range vertically
	local start_index = 0
	local end_index = 0
	
	--[[
	-- DEBUG
	debugt = Map.InsertThing(xmin, y)
	debugt.type = 301
	-- END DEBUG
	--]]
	
	while true do
		-- let's find our starting line
		start_index = move_past(xmin, y, start_index, start_lines)
		if start_index < 1 then
			return
		end
		local startl = start_lines[start_index]
		
		-- found our starting line, let's find where the starting line intersects our horizontal cut
		local new_line = Line2D.From(xmin, y, xmax, y)
		
		local intersection
		
		if startl.GetY1() == startl.GetY2() then
			intersection = startl.GetV2()
		else
			intersection = Line2D.GetIntersectionPoint(startl, new_line)
		end
		
		if not intersection.IsFinite() then
			if y == startl.GetY2() then
				intersection = startl.GetV2()
			elseif y == startl.GetY1() then
				intersection = startl.GetV1()
			end
		end
		
		-- DEBUG
		-- this commented out section is an example of something you can
		-- do to help debug geometry code. in this case, it places Things
		-- along all the start line intersections
		if intersection.IsFinite() then
			debugt = Map.InsertThing(intersection)
			debugt.type = 9033
		else
			UI.LogLine("")
			UI.LogLine(start_lines[start_index])
			UI.LogLine(Line2D.From(xmin, y, xmax, y))
			UI.LogLine(y)
		end
		-- END DEBUG
		
		new_line = Line2D.From(intersection, xmax, y)
		-- found our intersection, applied it to new_line
		
		local endx = xmin
		while endx <= new_line.GetX1() do
			-- let's find our ending line
			end_index = move_past(intersection.x, y, end_index, end_lines)
			
			if end_index < 1 then
				return
			end
			
			local endl = end_lines[end_index]
			
			-- let's find where the horizontal cut intersects the ending line
			if endl.GetY1() == endl.GetY2() then
				intersection = endl.GetV2()
			else
				intersection = Line2D.GetIntersectionPoint(endl, new_line)
			end
			
			endx = intersection.x
		end
		
		
		-- DEBUG
		-- this commented out section is an example of something you can
		-- do to help debug geometry code. in this case, it places Things
		-- along all the end line intersections and selects them
		if intersection.IsFinite() then
			debugt = Map.InsertThing(intersection)
			debugt.type = 9034
			debugt.selected = true
		else
			UI.LogLine("")
			UI.LogLine(start_lines[start_index])
			UI.LogLine(Line2D.From(xmin, y, xmax, y))
			UI.LogLine(y)
		end
		-- END DEBUG
		
		new_line = new_line.WithV2(intersection)
		
		if new_line.GetX1() < new_line.GetX2() and new_line.GetLengthSq() > 0.9 then
			finish_at_line(new_line)
		end
		-- set up next cut
		xmin = new_line.GetX2()
	end
end

function gridify_sector(sector, grid_size)
	if grid_size < 2 then
		return
	end
	
	local sector_lines = sector.GetLinedefs()
	
	-- categorize lines by whether their front sidedef is left or right
	local end_lines = {}
	local start_lines = {}
	
	
	local xmin = sector_lines[1].start_vertex.position.x
	local xmax = sector_lines[1].start_vertex.position.x
	local ymin = sector_lines[1].start_vertex.position.y
	local ymax = sector_lines[1].start_vertex.position.y
	
	for _,line in ipairs(sector_lines) do
		local is_line_included = false
		
		if line.IsFlagSet("twosided") then
			local front_index = line.GetFront().GetSector().GetIndex()
			local back_index =line.GetBack().GetSector().GetIndex()
			
			if front_index ~= back_index then
				is_line_included = true
			end
		else
			is_line_included = true
		end
		
		if is_line_included then
			local normal = line.GetNormal()
			local l2d = Line2D.From(line)
			
			local x1 = l2d.GetX1()
			local x2 = l2d.GetX2()
			local y1 = l2d.GetY1()
			local y2 = l2d.GetY2()
			
			-- lets flip all the lines around so that the first vertex is to the left
			if x2 < x1 then
				l2d = l2d.GetFlipped()
			end
			
			-- adjust if the back side is ours
			local back = line.GetBack()
			
			if back ~= nil and back.GetSector().GetIndex() == sector.GetIndex() then
				normal = normal * -1
			end
		
			if normal.x == 0 then
				-- horizontal case, we need to figure out what lines are to the left/right of it
				-- look at leftmost vertex first to determine if it's an end
				local end_neighbors
				local start_neighbors
				
				-- figure out our leftmost/rightmost vertices
				if x2 < x1 then
					end_neighbors = line.end_vertex.GetLines()
					start_neighbors = line.start_vertex.GetLines()
				else
					start_neighbors = line.end_vertex.GetLines()
					end_neighbors = line.start_vertex.GetLines()
				end
				
				local is_end = true
				for _, neighbor in ipairs(end_neighbors) do
					-- is our left neighbor facing into the sector
					if
						(
						  neighbor.GetFront() ~= nil
						  and neighbor.GetFront().GetSector().GetIndex() == sector.GetIndex()
						)
						and -- and make sure both sides are not our sectors
						(
						  neighbor.GetBack() == nil
						  or neighbor.GetBack().GetSector().GetIndex() ~= sector.GetIndex()
						)
					then
						-- then we can't be an end
						is_end = false
					end
				end
				
				if is_end then
					end_lines[#end_lines+1] = l2d
				end
				
				
				local is_start = true
				-- handle right neighbor / can we be a start?
				for _, neighbor in ipairs(start_neighbors) do
					-- is our right neighbor facing into the sector
					if
						(
						  neighbor.GetFront() ~= nil
						  and neighbor.GetFront().GetSector().GetIndex() == sector.GetIndex()
						)
						and -- and make sure both sides are not our sectors
						(
						  neighbor.GetBack() == nil
						  or neighbor.GetBack().GetSector().GetIndex() ~= sector.GetIndex()
						)
					then
						-- then we can't be an end
						is_start = false
					end
				end
				
				if is_start then
					start_lines[#start_lines+1] = l2d
				end
			elseif normal.x < 0 then
				start_lines[#start_lines+1] = l2d
			else
				end_lines[#end_lines+1] = l2d
			end
			
			xmin = math.min(xmin, x1)
			xmin = math.min(xmin, x2)
			xmax = math.max(xmax, x1)
			xmax = math.max(xmax, x2)
			
			ymin = math.min(ymin, y1)
			ymin = math.min(ymin, y2)
			ymax = math.max(ymax, y1)
			ymax = math.max(ymax, y2)
		end
	end
	
		--[[
	we will sort the list of lines so that they will be
	sorted left -> right (based on the first, leftmost vertex)
	if the leftmost vertex is the same, then the rightmost is used

	see these links for more information on table.sort
	https://www.lua.org/pil/19.3.html
	http://lua-users.org/wiki/TableLibraryTutorial
	--]]
	function line_comparison(a,b)
		local ax1 = a.GetX1()
		local ax2 = a.GetX2()
		local bx1 = b.GetX1()
		local bx2 = b.GetX2()
		
		if ax1 < bx1 then
			return true
		elseif ax1 == bx1 and ax2 < bx2 then
			return true
		else
			return false
		end
	end
	table.sort(start_lines, line_comparison)
	table.sort(end_lines, line_comparison)
	
	
	-- lets round off our minimums to our grid_size
	ymin = (math.floor(ymin / grid_size) + 1) * grid_size
	ymax = (math.floor(ymax / grid_size) - 1) * grid_size
	
	
	for y = ymin, ymax, grid_size do
		
		do_row(y, xmin, xmax, start_lines, end_lines)
	end
	
end

-- well, we need to know where we clicked
-- the first false means no snap to grid
-- the second means no snapping to geometry
local cursor = UI.GetMouseMapPosition(false, false)

-- find sector exactly at mouse
local start_sector = Map.DetermineSector(cursor)

-- if nothing at current exact mouse position, let's try to just find what's nearby
if start_sector == nil then
	start_sector = Map.NearestSector(cursor)
end

gridify_sector(start_sector, 16)