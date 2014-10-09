
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

if Input:pressed(K_Q) then
	spot6.Motor = -0.1
	spot7.Motor = 0.09
elseif Input:pressed(K_E) then
	spot6.Motor = 0.1
	spot7.Motor = -0.09
else
	spot7.Frozen = true
	spot6.Frozen = true
end


if Input:pressed(K_Right) then
	spot8.Motor = 0.1
elseif Input:pressed(K_Left) then

	spot8.Motor = -0.1
else
	spot8.Frozen = true
end

if Input:pressed(K_Up) then
	spot10.Motor = 1
	spot9.Motor = -1
elseif Input:pressed(K_Down) then
	spot10.Motor = -1
	spot9.Motor = 1
else
	spot10.Frozen = true
	spot9.Frozen = true
end

if Input:pressed(K_X) then
	spot5.Motor = 1
	spot4.Motor = -1
end


if spot5.Frozen and Input:released(K_X) then
	spot5:AddLimitsCycle(1)
end
if spot4.Frozen and Input:released(K_X) then
	spot4:AddLimitsCycle(-1)
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
	spot18.Motor = -1
	spot17.Motor = -1
	spot20.Motor = -1
	spot19.Motor = -1
	
elseif Input:pressed(K_D) then
	spot2.Motor = 1
	spot3.Motor = 1
	spot18.Motor = 1
	spot17.Motor = 1
	spot20.Motor = 1
	spot19.Motor = 1
else
	spot20.Motor = 0
	spot19.Motor = 0
	spot2.Motor = 0
	spot3.Motor = 0
	spot18.Motor = 0
	spot17.Motor = 0
end

if Input:pressed(K_G) then
	spot20.Motor = -1
	spot19.Motor = 1
end
	
if Input:justPressed(K_Space) and spot1.Frozen and spot0.Frozen then
	spot1:AddLimitsCycle(1)
	spot0:AddLimitsCycle(-1)
	spot1.Motor = 1
	spot0.Motor = -1
end
if Input:pressed(K_Z) then
	spot1:AddLimitsCycle(-1)
	spot1.Motor = -1
end
if Input:pressed(K_C) then
	spot0:AddLimitsCycle(1)
	spot0.Motor = 1
end

--if spot1.HitMax then
--	spot1.Motor = -1
--end
--if spot0.HitMin then
--	spot0.Motor = 1
--end