if ToReset then
	ToReset = false
	i = 0
	while i < G.RobotCount and i < 2 do
		G:Robot(i):MoveTo(G:Art(G:Robot(i).Id))
		G:Robot(i):Reset()
		i = i + 1
	end
end