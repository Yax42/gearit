if Robot.State == 0 then
	if FrameCount % 122 < 15 then
		spot0.Motor = 0.081
	elseif FrameCount % 122 < 48 then
		spot0.Motor = -0.001
	elseif FrameCount % 122 < 59 then
		spot0.Motor = 0.622
	end
end
if Robot.State == 0 then
	if FrameCount % 76 < 43 then
		spot1.Motor = -0.96
	elseif FrameCount % 76 < 24 then
		spot1.Motor = 0.933
	elseif FrameCount % 76 < 9 then
		spot1.Motor = -0.505
	end
end
if Robot.State == 0 then
	if FrameCount % 32 < 32 then
		spot2.Motor = 0.324
	end
end
if Robot.State == 0 then
	if FrameCount % 28 < 5 then
		spot3.Motor = 0.161
	elseif FrameCount % 28 < 23 then
		spot3.Motor = -0.122
	end
end
if Robot.State == 0 then
	if FrameCount % 32 < 32 then
		spot4.Motor = 0.324
	end
end
if Robot.State == 0 then
	if FrameCount % 49 < 49 then
		spot5.Motor = -0.605
	end
end
if Robot.State == 0 then
	if FrameCount % 77 < 23 then
		spot6.Motor = -0.323
	elseif FrameCount % 77 < 18 then
		spot6.Motor = -0.581
	elseif FrameCount % 77 < 36 then
		spot6.Motor = 0.567
	end
end
if Robot.State == 0 then
	if FrameCount % 104 < 48 then
		spot7.Motor = 0.205
	elseif FrameCount % 104 < 56 then
		spot7.Motor = -0.007
	end
end
if Robot.State == 0 then
	if FrameCount % 95 < 6 then
		spot8.Motor = -0.1
	elseif FrameCount % 95 < 50 then
		spot8.Motor = -0.808
	elseif FrameCount % 95 < 39 then
		spot8.Motor = -0.922
	end
end
if Robot.State == 0 then
	if FrameCount % 95 < 6 then
		spot9.Motor = -0.1
	elseif FrameCount % 95 < 50 then
		spot9.Motor = -0.808
	elseif FrameCount % 95 < 39 then
		spot9.Motor = -0.922
	end
end
