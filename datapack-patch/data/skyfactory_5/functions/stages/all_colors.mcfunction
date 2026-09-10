gamestage add @s all_colors
#Sets cloud, sun, and moon to green and disables sunrise colors
execute as @e[type=player,distance=..16] run colorfulskies disablesunrise @s true
execute as @e[type=player,distance=..16] run colorfulskies color @s cloud clear
execute as @e[type=player,distance=..16] run colorfulskies color @s sun clear
execute as @e[type=player,distance=..16] run colorfulskies color @s moon clear
execute as @e[type=player,distance=..16] run colorfulskies color @s sunrise clear
execute as @e[type=player,distance=..16] run colorfulskies color @s sky clear

#execute as @e[type=player,distance=..16] run give @s sf5_things:green_apple
execute as @e[type=player,distance=..16] run playsound minecraft:ui.toast.challenge_complete master @s ~ ~ ~ 0.35

execute as @e[type=player,distance=..16] run tag @s add all_colors_stage
