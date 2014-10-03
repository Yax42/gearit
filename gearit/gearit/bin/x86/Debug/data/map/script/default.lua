if Game.Frame == 0 then
	Robot_0:MoveTo(G:Art(2))
	Game:Robot(1):MoveTo(Art_3)
	Object_Ball.Static = true
	Object_Ball.Position = Art_1.Position
	left_turn = false
	right_turn = true
	return
end
if left_turn == true then
	if Robot_0:IsTouching(Object_Ball) == true then
		Object_Ball.Static = false
		left_turn = false
	end
elseif right_turn == true then
	if Robot_1:IsTouching(Object_Ball) == true then
		Object_Ball.Static = false
		right_turn = false
	end
else
	if Object_Ball:IsTouching(Object_right_zone) then
		Object_Ball.Static = true
		Object_Ball.Position = Art_0.Position
		left_turn = true
	end
	if Object_Ball:IsTouching(Object_left_zone) then
		Object_Ball.Static = true
		Object_Ball.Position = Art_1.Position
		right_turn = true
	end
end
