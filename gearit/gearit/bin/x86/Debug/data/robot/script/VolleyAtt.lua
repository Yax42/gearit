
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
	
if Input:pressed(K_A) then
	spot2.Motor = -1
	spot3.Motor = -1	
elseif Input:pressed(K_D) then
	spot2.Motor = 1
	spot3.Motor = 1
else
	spot2.Motor = 0
	spot3.Motor = 0
end