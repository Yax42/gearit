
-- #BeginAutoGenerated
-- Auto generated code, do NOT modify

-- #EndAutoGenerated

ratioWheel = 1
ratioBump = 1

if Input:pressed(K_E) then
	spot3.Motor = ratioBump
	spot0.Motor = ratioBump
	spot2.Motor = -ratioBump
elseif Input:pressed(K_Q) then
	spot3.Motor = ratioBump
	spot1.Motor = -ratioBump
	spot2.Motor = -ratioBump
elseif Input:pressed(K_W) then
	spot3.Motor = ratioBump
	spot0.Motor = ratioBump
	spot1.Motor = -ratioBump
	spot2.Motor = -ratioBump
else
	spot3.Motor = -0.5
	spot1.Motor = 0.5
	spot0.Motor = -0.5
	spot2.Motor = 0.5
end

if Input:pressed(K_D) then
	spot12.Motor = ratioWheel
	spot13.Motor = ratioWheel
elseif Input:pressed(K_A) then
	spot12.Motor = -ratioWheel
	spot13.Motor = -ratioWheel
else
	spot12.Motor = 0
	spot13.Motor = 0
end

if Input:pressed(K_Space) then
	spot6.Motor = -1
	spot7.Motor = 1
	spot8.Motor = 1
	spot9.Motor = -1
else
	spot6.Motor = 0.1
	spot7.Motor = -0.1
	spot8.Motor = -0.1
	spot9.Motor = 0.1
end