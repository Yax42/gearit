if Game.RobotCount < 2 then
    return
end

if left_turn == true then
    if Game:Robot(0):IsTouching(Object_Ball) == true then
        Object_Ball.Static = false
        left_turn = false
    end
elseif right_turn == true then
    if Game:Robot(1):IsTouching(Object_Ball) == true then
        Object_Ball.Static = false
        right_turn = false
    end
else
    if Object_Ball:IsTouching(Object_right_ground) then
        Object_Ball.Static = true
		Game:Robot(0).IntScore = Game:Robot(0).IntScore + 1
        Object_Ball.Position = Art_0.Position
        left_turn = true
    end
    if Object_Ball:IsTouching(Object_left_ground) then
        Object_Ball.Static = true
		Game:Robot(1).IntScore = Game:Robot(1).IntScore + 1
        Object_Ball.Position = Art_1.Position
        right_turn = true
    end
end
if Game:Robot(0):IsTouching(Object_right_ground) then
	Game:Robot(0):MoveTo(Art_2)
elseif Game:Robot(1):IsTouching(Object_left_ground) then
	Game:Robot(1):MoveTo(Art_3)
end