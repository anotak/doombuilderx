G = 11
PATHS = 2

grid = {}
for i = 1, G do
	grid[i] = {}
	for j = 1, G do
		grid[i][j] = 0
	end
end

math.randomseed(os.clock()*100000000000)
--math.randomseed(2147483647)

for i = 1, PATHS do
	P = {6,6}
	ind = 0
	while P[1] >= 1 and P[2] >= 1 and P[1] <= G and P[2] <= G do
		UI.LogLine(tostring(P[1]) .. " " .. tostring(P[2]))
		
		if grid[P[1]][P[2]] == 0 then
			grid[P[1]][P[2]] = ind+1
		end

		r = math.random(1,4)
		if r == 1 then P[1] = P[1]-1 end
		if r == 2 then P[1] = P[1]+1 end
		if r == 3 then P[2] = P[2]-1 end
		if r == 4 then P[2] = P[2]+1 end
		ind = ind+1
	end
end