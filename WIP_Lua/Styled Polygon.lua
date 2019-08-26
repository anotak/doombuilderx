-- styled polygon.lua by anotak

map_linedefs = Map.GetSelectedLinedefs()
map_vertices = Map.GetSelectedVertices()

if #map_linedefs <= 0 then
	UI.LogLine("no linedefs selected")
	return
end

hash_size = #(Map.GetVertices()) * 2



styles = {}
styles_keys = {}

function to_key(index1,index2)
	return (index1 + (index2 * 8)) % hash_size
end

function choose_style()
	return styles[styles_keys[math.ceil(math.random() * #styles_keys)]]
end

function add_style(mid_v)
	local linedefs = mid_v.GetLines()
	local mid_p = mid_v.position
	
	for i=1,#linedefs do
		for j=i+1,#linedefs do
			local first_l = linedefs[i]
			local second_l = linedefs[j]
			
			local index1 = first_l.GetIndex()
			local index2 = second_l.GetIndex()
			
			if index1 > index2 then
				second_l = first_l
				first_l = linedefs[j]
				
				index1 = first_l.GetIndex()
				index2 = second_l.GetIndex()
			end
			
			local start_v = first_l.start_vertex
			
			if start_v.GetIndex() == mid_v.GetIndex() then
				start_v = first_l.end_vertex
			end
			
			local end_v = second_l.start_vertex
			
			if end_v.GetIndex() == mid_v.GetIndex() then
				end_v = second_l.end_vertex
			end
			
			local start_p = start_v.position
			local end_p = end_v.position
			
			local total_angle = Line2D.From(start_p, end_p).GetAngle()
			local style_angle = total_angle - (Line2D.From(start_p, mid_p).GetAngle())
			local style_length = (mid_p - start_p).GetLength() / (end_p - start_p).GetLength()
			
			
			local key = to_key(index1,index2)
			while styles[key] ~= nil do
				key = key + 1
			end
			
			styles_keys[#styles_keys + 1] = key
			styles[key] = {}
			styles[key].index1 = index1
			styles[key].index2 = index2
			styles[key].angle = style_angle
			styles[key].length = style_length
		end -- j=i+1,#linedefs
	end -- i=1,#linedefs
end -- add_style(v)

for _,v in ipairs(map_vertices) do
	add_style(v)
end

local cursor = UI.GetMouseMapPosition(false,false)

UI.AddParameter("size", "size", 128, "how big is the polygon")
UI.AddParameter("angle", "angle", 0, "what angle to start the polygon at")
UI.AddParameter("sides", "# of sides", 6, "how many sides do you want your polygon to have")

parameters = UI.AskForParameters()

r = parameters.size
sides = parameters.sides
theta = parameters.angle
delta_angle = (2*math.pi) / sides

path = {}



-- current vector, target vector, recursive depth
function move_toward(cur, tgt, depth)
	if depth > 0 then
		path[#path + 1] = tgt
	else
		depth = depth + 1
		
		local style = choose_style()
		--local style = {angle = 0, length = .5}
		local total_angle = Line2D.From(cur, tgt).GetAngle()
		local total_length = (tgt - cur).GetLength()
		local mid = Vector2D.FromAngle(style.angle + total_angle, style.length * total_length) + cur
		move_toward(cur, mid, depth)
		move_toward(mid, tgt, depth)
	end
end

local start = Vector2D.From(cursor.x, cursor.y)
path[1] = start + Vector2D.FromAngle(theta, r)

while sides >= 0 do
	theta = theta - delta_angle
	move_toward(path[#path], start + Vector2D.FromAngle(theta, r), 0)
	sides = sides - 1
end

local p = Pen.From(start)
p.snaptogrid = false
p.stitchrange = 1

for _,point in ipairs(path) do
	-- FIXME SNAP TO ACCURACY
	p.position = point
	p.DrawVertex()
end

p.FinishPlacingVertices()



