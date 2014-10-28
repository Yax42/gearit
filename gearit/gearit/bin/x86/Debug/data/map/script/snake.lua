if G.RobotCount == 0 then
	return
end

Object_Snake_0.Static = false


for i=1,11 do
G:Object(i).Static = false
	G:Object(i).Speed = G:Object(i):Direction(G:Object(i - 1), 8 - i * 0.3)
end
Object_Snake_0.Speed = Object_Snake_0:Direction(Robot, 8)
