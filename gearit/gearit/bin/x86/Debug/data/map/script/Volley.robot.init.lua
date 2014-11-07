if Game.RobotCount == 1 then
	Robot:MoveTo(Art_2)
else
	Robot:MoveTo(Art_3)
end

Robot:StaticCamera(Art_4, 0.2)