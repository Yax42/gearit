require 'header'


while true do
	if Input:pressed(K_Left) then
		roue1.motor = 1
		roue2.motor = 1
	elseif Input:pressed(K_Right) then
		roue1.motor = -1
		roue2.motor = -1
	else
		roue1.motor = 0
		roue1.motor = 0
	end
	
	if Input:pressed(K_S) then
		boulet.motor = 1
	elseif Input:pressed(K_Q) then
		boulet.motor = -1
	end
	
	if Input:pressed(K_W) then
		tige.motor = 1
	elseif Input:pressed(K_X) then
		tige.motor = -1
	end

end