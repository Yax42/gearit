require 'header'
jump_time = 0

while true do
// #!#! Lua generated - Do not modify
// Generated
if Input:pressed(K_D) then
spot0.motor = 1
spot1.motor = 1
end
if Input:pressed(K_A) then
spot1.motor = -1
spot0.motor = -1
end
// #!#! End Lua generated
end