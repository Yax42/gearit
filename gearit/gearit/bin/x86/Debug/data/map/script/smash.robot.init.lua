Robot:StaticCamera(Art_3, 0.125)
if Robot.Id < 2 then
	Robot:MoveTo(G:Art(Robot.Id))
else
	Robot:MoveTo(G:Art(2))
end
