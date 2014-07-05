if Input:pressed(K_S) then
	spot0.MaxAngle = 0
end

if Input:pressed(K_D) then
	spot0.Motor = 1
	spot0.Frozen = false
		spot1.Motor = 1
	spot1.Frozen = false

elseif Input:pressed(K_A) then
	spot0.Motor = -1
	spot0.Frozen = false
		spot1.Motor = -1
	spot1.Frozen = false
else
	spot1.Frozen = true

	
	spot0.Frozen = true
end

