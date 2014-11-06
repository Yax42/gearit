if Robot.Id > 1 then
	return;
end

if Robot:TriggerData(0) and Robot.Id < 6 then
	ToReset = true
	Opponent = 0
	if Robot.Id == 0 then
		Opponent = 1
	end
	if G.RobotCount > 1 then
		G:Robot(Opponent).IntScore = G:Robot(Opponent).IntScore + 1
	end
end
