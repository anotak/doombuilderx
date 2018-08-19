-- select_linedef_by_textures.lua by anotak
-- based on the linedef nearest to the mouse, selects all neighboring linedefs that share a texture
-- then selects the neighbors of those, and so on

-- we will be using a depth-first search algorithm
-- for more information on dfs, see https://en.wikipedia.org/wiki/Depth-first_search

-- deselect sectors/lines/vertices
Map.DeselectGeometry()

-- well, we need to know where we clicked
-- the first false means no snap to grid
-- the second means no snapping to geometry
cursor = UI.GetMouseMapPosition(false, false)

start_line = Map.NearestLinedef(cursor)

line_dfs = {}
line_dfs.searched_lines = {}
line_dfs.searched_vertices = {}

function line_dfs.select_lines_through_vert(current_vert)
	if line_dfs.searched_vertices[current_vert.GetIndex()] == nil then
		-- mark it as searched
		line_dfs.searched_vertices[current_vert.GetIndex()] = true
		
		local vert_lines = current_vert.GetLines()
		
		for i, line in ipairs(vert_lines) do
			line_dfs.select_lines_recursive(line)
		end
	end
end

function line_dfs.sidedef_has_texture(sidedef, texture)
	if sidedef == nil then
		return false
	end
	
	if sidedef.uppertex == texture then
		return true
	end
	if sidedef.lowertex == texture then
		return true
	end
	if sidedef.midtex == texture then
		return true
	end
	
	return false
end

function line_dfs.select_lines_recursive(current_line)
	-- check if the current sector hasn't been searched already
	if line_dfs.searched_lines[current_line.GetIndex()] == nil then
		-- mark it as searched
		line_dfs.searched_lines[current_line.GetIndex()] = true
		
		local no_texture_found = true
		local front = current_line.GetFront()
		local back = current_line.GetBack()
		for _, texture in ipairs(line_dfs.textures) do
			if line_dfs.sidedef_has_texture(front, texture)
					or line_dfs.sidedef_has_texture(back, texture) then
				no_texture_found = false
				break
			end
		end
		
		if no_texture_found then
			return
		end
		
		-- select it
		current_line.selected = true
		
		-- let's go find our neighbors through our vertices
		line_dfs.select_lines_through_vert(current_line.start_vertex)
		line_dfs.select_lines_through_vert(current_line.end_vertex)
	end
end

if start_line == nil then
	UI.LogLine("couldn't find a linedef. maybe no linedef on the map?");
else
	line_dfs.textures = {}
	
	front = start_line.GetFront()
	
	if front ~= nil then
		if front.midtex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = front.midtex
		end
		if front.uppertex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = front.uppertex
		end
		if front.lowertex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = front.lowertex
		end
	end
	
	back = start_line.GetBack()
	
	if back ~= nil then
		if back.midtex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = back.midtex
		end
		if back.uppertex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = back.uppertex
		end
		if back.lowertex ~= "-" then
			line_dfs.textures[#line_dfs.textures+1] = back.lowertex
		end
	end
	
	if #line_dfs.textures == 0 then
		-- if you clicked a line with no textures, select everything
		line_dfs.textures[#line_dfs.textures+1] = "-"
	end
	
	line_dfs.select_lines_recursive(start_line)
end
