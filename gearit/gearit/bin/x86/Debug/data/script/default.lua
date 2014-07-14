if Robot.State == 0 then
	if FrameCount % 246 < 7 then
		spot0.Motor = 0,01064818
	elseif FrameCount % 246 < 38 then
		spot0.Motor = 0,1397838
	elseif FrameCount % 246 < 149 then
		spot0.Motor = 0,1058184
	elseif FrameCount % 246 < 52 then
		spot0.Motor = 0,2927569
	end
end
if Robot.State == 0 then
	if FrameCount % 197 < 28 then
		spot1.Motor = 0,2758957
	elseif FrameCount % 197 < 51 then
		spot1.Motor = 0,05575131
	elseif FrameCount % 197 < 16 then
		spot1.Motor = 0,8358598
	elseif FrameCount % 197 < 102 then
		spot1.Motor = 0,00140491
	end
end
if Robot.State == 0 then
	if FrameCount % 528 < 46 then
		spot2.Motor = 0,11694
	elseif FrameCount % 528 < 140 then
		spot2.Motor = 0,9970732
	elseif FrameCount % 528 < 132 then
		spot2.Motor = 0,3021188
	elseif FrameCount % 528 < 210 then
		spot2.Motor = 0,1334221
	end
end
if Robot.State == 0 then
	if FrameCount % 235 < 235 then
		spot3.Motor = 0,04943264
	end
end
