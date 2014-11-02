if Object_1.PosY > initPosY or Object_1.PosY < initPosY - 18 then
	speed = speed * -1
end

if T > oldT + 0.04 then
	Object_1.PosY = Object_1.PosY + speed -- todo : gerer la vitesse avec le FrameCount
	oldT = Game.Time
end

T = Game.Time