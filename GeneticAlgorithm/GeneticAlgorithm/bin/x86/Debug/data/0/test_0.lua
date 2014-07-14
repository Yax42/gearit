if Game.Frame == 0 then
	Robot_0:MoveTo(Art_0)
	Robot_0.State = 0
end


if Robot_0:TriggerData(0) or Game.Frame == 30 * 60 then
	Robot_0:IntScore(0)
	Robot_0:FloatScore(Robot_0:Distance(Art_1))
	Game:Finish()
elseif Robot_0:TriggerData(2) then
	Robot_0:IntScore(1)
	Robot_0:FloatScore(Game.Frame)
	Game:Finish()
end
