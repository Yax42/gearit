if Game.Frame == 0 then
	Robot_0:MoveTo(Art_0)
end
if Robot_0:TriggerData(1) then
	Robot_0:MoveTo(Art_1)
end
if Robot_0:TriggerData(0) then
	Game:Finish();
end
if Robot_0:IsTouching(Object_test) then
	Game:Finish();
end
