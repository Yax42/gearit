
-- #BeginAutoGenerated
-- Auto generated code, do NOT modify

-- #EndAutoGenerated

if Input:pressed(K_W) then
	spot1.Motor = 1
	spot0.Motor = -1
end
if Input:pressed(K_S) then
	spot1.Motor = -1
	spot0.Motor = 1
end

if Input:pressed(K_R) then
	spot11.Motor = 1
	spot12.Motor = -1
	spot13.Motor = -1
	spot14.Motor = 1
else
	spot11.Motor = -0.2
	spot12.Motor = 0.2
	spot13.Motor = 0.2
	spot14.Motor = -0.2
end

if Input:pressed(K_LeftShift) then
	force = 1
else
	force = 0.2
end

if Input:pressed(K_A) then
	spot2.Motor = -force
	spot3.Motor = -force
	spot5.Motor = -force
	spot4.Motor = -force
elseif Input:pressed(K_D) then
	spot2.Motor = force
	spot3.Motor = force
	spot5.Motor = force
	spot4.Motor = force
else
	spot2.Motor = 0
	spot3.Motor = 0
	spot5.Motor = 0
	spot4.Motor = 0
end

spot2.Frozen = Input:pressed(K_Space)
spot3.Frozen = Input:pressed(K_Space)
spot5.Frozen = Input:pressed(K_Space)
spot4.Frozen = Input:pressed(K_Space)


