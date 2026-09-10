gamestage add @s colorless
#Sets cloud, sun, and moon to green and disables sunrise colors
execute as @e[type=player,distance=..16] run colorfulskies disablesunrise @s true
execute as @e[type=player,distance=..16] run colorfulskies color @s cloud 919191
execute as @e[type=player,distance=..16] run colorfulskies color @s sun 919191
execute as @e[type=player,distance=..16] run colorfulskies color @s moon 919191
execute as @e[type=player,distance=..16] run colorfulskies color @s sunrise 919191
execute as @e[type=player,distance=..16] run colorfulskies color @s sky 919191

#Sets Curios slots
curios set back @s 1
curios set belt @s 1
curios set quiver @s 1
curios set charm @s 2

#Sets spawnpoint
spawnpoint @s
