-- Cellular Automata.lua
-- based on code from Grid.c in the Brogue source code by Brian Walker
-- particularly the function createBlobOnGrid(..)
-- this has been reproduced under the AGPL terms of Brogue's source code
-- (see Cellular Automata License.txt)
-- the original license message is reproduced here:
--[[
/*
 *  Architect.c
 *  Brogue
 *
 *  Created by Brian Walker on 1/10/09.
 *  Copyright 2012. All rights reserved.
 *  
 *  This file is part of Brogue.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
--]]

-- well we need a class for all our grids
GridClass = {}
function GridClass:get(x, y)
	x = (x + self.w) % (self.w + 1)
	y = (y + self.h) % (self.h + 1)
	
	return self[y*self.w + x]
end

function GridClass:set(x, y, value)
	x = (x + self.w) % (self.w + 1)
	y = (y + self.h) % (self.h + 1)
	
	self[y*self.w + x] = value
end

function GridClass:init(value, nw, nh)
	if nw ~= nil and nh ~= nil then
		self.w = nw
		self.h = nh
	end
	
	if self.w == nil then
		error("self.w is nil for init")
	end
	if self.h == nil then
		error("self.h is nil for init")
	end
	
	for y=1,self.h do
		for x=1,self.w do
			self:set(x,y,value)
		end
	end
end

function GridClass:output()
	out_string = "[\n"
	
	for y=1,self.h do
		for x=1,self.w do
			out_string = out_string .. " " .. tostring(self:get(x,y))
		end
		out_string = out_string .. "\n"
	end
	
	out_string = out_string .. "]\n"
	
	UI.LogLine(out_string)
end

function GridClass:check_if_within(x,y)
	if x > self.w or y > self.h then
		return false
	end
	
	if x < 1 or y < 1 then
		return false
	end
	
	return true
end


function GridClass.new(w, h)
	local grid = {}
	setmetatable(grid, {__index = GridClass})
	grid:init(0,w,h)
	return grid
end

function GridClass.duplicate(other)
	local grid = GridClass.new(other.w,other.h)
	
	
	for y=1,other.h do
		for x=1,other.w do
			grid:set(x,y,other:get(x,y))
		end
	end
	
	--UI.LogLine("duplicated1")
	--grid:output()
	return grid
end

function rand_percent(p)
	if math.random() < (p/100) then
		return 1
	end
	return 0
end

nbDirs = {up = {0,-1}, down = {0,1}, left = {-1,0}, right = {1,0}, upleft = {-1,-1}, downleft = {-1,1}, upright = {1,-1}, downright = {1,1}}
nbCardinalDirs = {up = {0,-1}, down = {0,1}, left = {-1,0}, right = {1,0}}

--[[enum directions {
    NO_DIRECTION    = -1,
	// Cardinal directions; must be 0-3:
	UP				= 0,
	DOWN			= 1,
	LEFT			= 2,
	RIGHT			= 3,
	// Secondary directions; must be 4-7:
	UPLEFT			= 4,
	DOWNLEFT		= 5,
	UPRIGHT			= 6,
	DOWNRIGHT		= 7,
    
    DIRECTION_COUNT = 8,
};--]]

-- void cellularAutomataRound(short **grid, char birthParameters[9], char survivalParameters[9]) {
function cellularAutomataRound(grid, birthParameters, survivalParameters)
    -- short i, j, nbCount, newX, newY;
    -- enum directions dir;
	
    -- short **buffer2;
    
    local buffer2 = GridClass.duplicate(grid) --  Make a backup of grid in buffer2, so that each generation is isolated.
	--UI.LogLine("duplicated2")
	--buffer2:output()
    
    -- for(i=0; i<DCOLS; i++) {
        -- for(j=0; j<DROWS; j++) {
	for y=1,grid.h do
		for x=1,grid.w do
            -- nbCount = 0;
			local nbCount = 1
			
			for _,dir in pairs(nbDirs) do
				local newX = x + dir[1]
				local newY = y + dir[2]
				if buffer2:check_if_within(newX, newY) and buffer2:get(newX, newY) ~= 0 then
					nbCount = nbCount + 1
				end
			end
            if buffer2:get(x,y) == 0 and birthParameters[nbCount] == 1 then
				--UI.LogLine(1)
                grid:set(x, y, 1) -- birth
            elseif buffer2:get(x,y) ~= 0 and survivalParameters[nbCount] == 1 then
				--UI.LogLine(0)
                -- survival, do nothing
			else
				-- death
				grid:set(x, y, 0)
			end
			
			-- done w this cell
		end
	end
end

-- Marks a cell as being a member of blobNumber, then recursively iterates through the rest of the blob
function fillContiguousRegion(grid, x, y, fillValue)
    local dir
	local newX, newY
	local numberOfCells = 1
	
	grid:set(x,y, fillValue)
	
	-- Iterate through the four cardinal neighbors.
	for _,dir in pairs(nbCardinalDirs) do
		local newX = x + dir[1]
		local newY = y + dir[2]
		if grid:check_if_within(newX, newY) and grid:get(newX,newY) == 1 then -- If the neighbor is an unmarked region cell,
			numberOfCells = numberOfCells + fillContiguousRegion(grid, newX, newY, fillValue) -- then recurse.
		end
	end
	
	return numberOfCells
end

-- Loads up grid with the results of a cellular automata simulation.
-- returns the x,y, width, height of the blob
function create_blob_on_grid(grid,
							roundCount,
							minBlobWidth, minBlobHeight,
							maxBlobWidth, maxBlobHeight,
							
							percentSeeded,
							birthParameters,
							survivalParameters)
	-- returns minx, miny, width, height
	local minx, miny, width, height
	--local i, j, k
	local blobNumber, blobSize, topBlobNumber, topBlobSize
    local topBlobMinX, topBlobMinY, topBlobMaxX, topBlobMaxY, blobWidth, blobHeight
	
	-- //short buffer2[maxBlobWidth][maxBlobHeight]; // buffer[][] is already a global short array
	local foundACellThisLine
	
	if #birthParameters ~= 9 then
		error("wrong number of birth parameters " .. #birthParameters .. " instead of 9")
	end
	
	if #survivalParameters ~= 9 then
		error("wrong number of survival parameters " .. #survivalParameters .. " instead of 9")
	end
	
	-- Generate blobs until they satisfy the minBlobWidth and minBlobHeight restraints
	
	repeat
		-- Clear buffer.
        grid:init(0)
		
		-- Fill relevant portion with noise based on the percentSeeded argument.
		for x=1,maxBlobWidth do
			for y=1,maxBlobHeight do
				grid:set(x,y,rand_percent(percentSeeded))
			end
		end
		
		--UI.LogLine("Random starting noise:")
		--grid:output()
		
		-- Some iterations of cellular automata
		for k=1,roundCount do
			cellularAutomataRound(grid, birthParameters, survivalParameters)
			--grid:output()
		end
		
		--UI.LogLine("After automata step:")
		--grid:output()
		
		--Now to measure the result. These are best-of variables; start them out at worst-case values.
		topBlobSize =   0
		topBlobNumber = 0
		topBlobMinX =   maxBlobWidth
		topBlobMaxX =   0
		topBlobMinY =   maxBlobHeight
		topBlobMaxY =   0
		
		-- Fill each blob with its own number, starting with 2 (since 1 means floor), and keeping track of the biggest:
		blobNumber = 2
		
		for y=1,grid.h do
			for x=1,grid.w do
				if grid:get(x,y) == 1 then -- an unmarked blob
					-- Mark all the cells and returns the total size:
					blobSize = fillContiguousRegion(grid, x, y, blobNumber)
					if (blobSize > topBlobSize) then -- if this blob is a new record
						topBlobSize = blobSize
						topBlobNumber = blobNumber
					end
					blobNumber = blobNumber + 1
				end
			end
		end
		
		--UI.LogLine("post blobs")
		--grid:output()
		
		
		
		-- Figure out the top blob's height and width:
		-- First find the max & min x:
		for x=1,grid.w do
			foundACellThisLine = false;
			for y=1,grid.h do
				if grid:get(x,y) == topBlobNumber then
					foundACellThisLine = true;
					break
				end
			end
			
			if foundACellThisLine then
				if x < topBlobMinX then
					topBlobMinX = x
				end
				
				if x > topBlobMaxX then
					topBlobMaxX = x
				end
			end
		end
		
		-- Then the max & min y:
		for y=1,grid.h do
			foundACellThisLine = false
			
			for x=1,grid.w do
				if grid:get(x,y) == topBlobNumber then
					foundACellThisLine = true;
					break
				end
			end
			if foundACellThisLine then
				if y < topBlobMinY then
					topBlobMinY = y
				end
				if y > topBlobMaxY then
					topBlobMaxY = y
				end
			end
		end
		
		blobWidth =		(topBlobMaxX - topBlobMinX) + 1
		blobHeight =	(topBlobMaxY - topBlobMinY) + 1
	until not (blobWidth < minBlobWidth or blobHeight < minBlobHeight or topBlobNumber == 0)
	
	-- Replace the winning blob with 1's, and everything else with 0's:
    for y=1,grid.h do
		for x=1,grid.w do
		if grid:get(x,y) == topBlobNumber then
				grid:set(x,y, 1)
			else
				grid:set(x,y, 0)
			end
		end
	end
	
	--UI.LogLine("done:")
	--grid:output()
	
	return topBlobMinX, topBlobMinY, blobWidth, blobHeight
end -- create_blob_on_grid
 
 
---------------------------------------------------------------
-- "main" so to speak
--------------------------------------------------------------- 

UI.AddParameter("w", "width (in cells)", 40, "how many cells east/west")
UI.AddParameter("h", "height (in cells)", 40, "how many cells north/south")
UI.AddParameter("cell_size_x", "individual cell width", 64, "how wide are individual cells")
UI.AddParameter("cell_size_y", "individual cell height", 64, "how tall are individual cells")
UI.AddParameter("startingpercent", "starting percent", 55, "how to fill cells at start")
UI.AddParameter("minx", "minimum blob width (cells)", 5, "if the blob is smaller than this, reroll")
UI.AddParameter("miny", "minimum blob height (cells)", 5, "if the blob is smaller than this, reroll")
UI.AddParameter("roundcount", "automata iterations", 5, "how many times to perform automata")
parameters = UI.AskForParameters()
 
grid = GridClass.new(parameters.w,parameters.h)
--grid:output()

 
create_blob_on_grid(grid,
					parameters.roundcount, -- roundcount
					parameters.minx, parameters.miny, -- min
					grid.w, grid.h, -- max
					parameters.startingpercent,
					{0,0,0,0, 0,0,1,1, 1}, -- birth
					{0,0,0,0, 1,1,1,1, 1}) -- survival
-- ok cool

local cursor = UI.GetMouseMapPosition(false,false)
local cell_size_x = parameters.cell_size_x
local cell_size_y = parameters.cell_size_y
for x=1,grid.w do
	for y=1,grid.h do
		if grid:get(x,y) == 1 then
			p = Pen.From(x * cell_size_x + cursor.x,y * cell_size_y + cursor.y)
			p.snaptogrid = false
			p.stitchrange = 2
			
			p.DrawVertex()
			
			p.MoveForward(cell_size_y)
			p.DrawVertex()
			
			p.TurnRightDegrees(90)
			p.MoveForward(cell_size_x)
			p.DrawVertex()
			
			p.TurnRightDegrees(90)
			p.MoveForward(cell_size_y)
			p.DrawVertex()
			
			p.TurnRightDegrees(90)
			p.MoveForward(cell_size_x)
			p.DrawVertex()
			
			
			p.FinishPlacingVertices()
		end
	end
end
	













