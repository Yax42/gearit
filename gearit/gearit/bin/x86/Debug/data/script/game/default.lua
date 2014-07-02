Robot_0.Position = Art_0.Position
a = Robot_0:TriggerData(0)
Robot_0:ScoreRobot(0)
--Robot_0.MoveTo(Art_0.Position)
while true do

if Robot_0:TriggerData(0) > 0 then
	Robot_0:TriggerData(0, 0)
	Robot_0:MoveTo(Art_1)
elseif Robot_0:TriggerData(1) > 0 then
	Game:Finish()
end
	




end
