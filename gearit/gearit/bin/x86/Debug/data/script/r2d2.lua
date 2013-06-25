require 'header'

while true do
	if Input:pressed(K_W) then
	  spot0.motor = -1
	  spot2.motor = 1
	else
	  spot0.motor = 0
	  spot2.motor = 0
	end

end