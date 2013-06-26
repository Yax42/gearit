require 'header'

jump_time = 0

while true do
	if Input:pressed(K_D) then
	  wheel0.motor = -1
	  wheel1.motor = -1
	elseif Input:pressed(K_A) then
	  wheel0.motor = 1
	  wheel1.motor = 1
	else
	  wheel0.motor = 0
	  wheel1.motor = 0
	end
	if jump_time == 0 and Input:pressed(K_W) then
	  jump_time = 20000
	end
	if jump_time > 15000 then
	  r0a.motor = 1
	  r1a.motor = -1
	  r2a.motor = 1

	  r0b.motor = -1
	  r1b.motor = 1
	  r2b.motor = -1
	else
	  r0a.motor = -0.3
	  r1a.motor = 0.3
	  r2a.motor = -0.3
	  
	  r0b.motor = 0.3
	  r1b.motor = -0.3
	  r2b.motor = 0.3
	end
	if jump_time > 0 then
	  jump_time = jump_time - 1
	end
end