require 'header'

while true do
	if Input:pressed(K_A) then
	  spot0.motor = 1
	  spot1.motor = 1
	elseif Input:pressed(K_D) then
	  spot0.motor = -1
	  spot1.motor = -1
	else
	  spot0.motor = 0
	  spot1.motor = 0
	end
end