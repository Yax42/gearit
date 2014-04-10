require 'header'

jump_time = 0

while true do
	if Input:pressed(K_D) then
	  wheel1.motor = -1
	  wheel2.motor = -1
	elseif Input:pressed(K_A) then
	  wheel1.motor = 1
	  wheel2.motor = 1
	else
	  wheel1.motor = 0
	  wheel2.motor = 0
	end
	if Input:pressed(K_W) then
	  arm1.motor = -1
	  arm2.motor = -1
	elseif Input:pressed(K_S) then
	  arm1.motor = 1
	  arm2.motor = 1
	else
	  arm1.motor = 0
	  arm2.motor = 0
	end
end