import crafttweaker.api.text.Component;

// Script for setting up custom tooltips
// var tool_belt_tip = Component.translatable("some.lang.key").setStyle(<constant:formatting:gold>);

/*
 var amazon = Component.literal("Amazon").setStyle(<constant:formatting:gold>);
 var twitch = Component.literal("Twitch").setStyle(<constant:formatting:light_purple>);
 var twitchPrime = Component.literal("Twitch Prime").setStyle(<constant:formatting:light_purple>);
 var message = Component.literal("Hey there! Do you want to know about ")
                + twitchPrime
                + Component.literal(" Oh! You may be asking, \"What's ")
                + twitchPrime
                + Component.literal("\"? Let me tell ya! When you connect your ")
                + amazon
                + Component.literal(" account to your ")
                + twitch
                + Component.literal(" account, you can get 1 free sub to ANY streamer on ")
                + twitch
                + Component.literal(", every month!");

 <item:minecraft:diamond>.addTooltip(message);

 Component.literal("Amazon").withStyle(style => style.withColor(0xFF55FF))

*/


  var tool_belt_tip = Component.literal("Улучшается поясной сумкой в наковальне").setStyle(<constant:formatting:gold>);
 <item:toolbelt:belt>.addTooltip(tool_belt_tip);


// I've been playing this game for like 10 years. I've probably played AE2 over 50 times
// and I still cannot remember what the heck devices use channels.
// I am now going to create a way to let me and other people who also
// do not know whether something uses a channel or not.
// You don't have to thank me, but I do ask that you subscribe to my
// twitch channel and use code darkosto at checkout to get
// negative 10% off your next purchase!

 var uses_channel = Component.literal("**Использует канал**").setStyle(<constant:formatting:yellow>);

 <item:ae2:drive>.addTooltip(uses_channel);
 <item:ae2:spatial_io_port>.addTooltip(uses_channel);
 <item:ae2:me_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:redstone_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:item_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:fluid_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:fe_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:light_p2p_tunnel>.addTooltip(uses_channel);
 <item:ae2:wireless_access_point>.addTooltip(uses_channel);
 <item:ae2:conversion_monitor>.addTooltip(uses_channel);
 <item:ae2:cable_pattern_provider>.addTooltip(uses_channel);
 <item:ae2:cable_interface>.addTooltip(uses_channel);
 <item:ae2:pattern_access_terminal>.addTooltip(uses_channel);
 <item:ae2:terminal>.addTooltip(uses_channel);
 <item:ae2:crafting_terminal>.addTooltip(uses_channel);
 <item:ae2:pattern_encoding_terminal>.addTooltip(uses_channel);
 <item:ae2:formation_plane>.addTooltip(uses_channel);
 <item:ae2:annihilation_plane>.addTooltip(uses_channel);
 <item:ae2:export_bus>.addTooltip(uses_channel);
 <item:ae2:import_bus>.addTooltip(uses_channel);
 <item:ae2:storage_bus>.addTooltip(uses_channel);
 <item:ae2:spatial_anchor>.addTooltip(uses_channel);
 <item:ae2:pattern_provider>.addTooltip(uses_channel);
 <item:ae2:256k_crafting_storage>.addTooltip(uses_channel);
 <item:ae2:64k_crafting_storage>.addTooltip(uses_channel);
 <item:ae2:16k_crafting_storage>.addTooltip(uses_channel);
 <item:ae2:4k_crafting_storage>.addTooltip(uses_channel);
 <item:ae2:1k_crafting_storage>.addTooltip(uses_channel);
 <item:ae2:io_port>.addTooltip(uses_channel);
 <item:ae2:interface>.addTooltip(uses_channel);
 <item:ae2:chest>.addTooltip(uses_channel);

// Create
 var extruder = Component.literal("Создаёт булыжник, камень и базальт").setStyle(<constant:formatting:yellow>);
 <item:create_mechanical_extruder:mechanical_extruder>.addTooltip(extruder);

 // Things I find funny
  var blooming_onion = Component.literal("Юридически чисто!").setStyle(<constant:formatting:white>);
  blooming_onion.setStyle(<constant:minecraft:formatting:italic>);
  <item:sf5_things:block_of_blooming_onion>.addTooltip(blooming_onion);

// OB Trophies
 var obtrophies = Component.literal("Нажмите, чтобы переключить спавн").setStyle(<constant:formatting:yellow>);
 <item:obtrophies:trophy>.addTooltip(obtrophies);


// Energetic Sheep
 var energetic_wool1 = Component.literal("Можно запитывать устройства нажатием").setStyle(<constant:formatting:yellow>);
 var energetic_wool2 = Component.literal("по совместимым машинам с шерстью в руке").setStyle(<constant:formatting:yellow>);

 <item:energeticsheep:white_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:orange_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:magenta_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:light_blue_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:yellow_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:lime_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:pink_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:gray_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:light_gray_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:cyan_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:purple_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:blue_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:brown_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:green_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:red_energetic_wool>.addTooltip(energetic_wool1);
 <item:energeticsheep:black_energetic_wool>.addTooltip(energetic_wool1);

 <item:energeticsheep:white_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:orange_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:magenta_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:light_blue_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:yellow_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:lime_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:pink_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:gray_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:light_gray_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:cyan_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:purple_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:blue_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:brown_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:green_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:red_energetic_wool>.addTooltip(energetic_wool2);
 <item:energeticsheep:black_energetic_wool>.addTooltip(energetic_wool2);



  var energetic_shears1 = Component.literal("Можно запитывать устройства нажатием").setStyle(<constant:formatting:yellow>);
  var energetic_shears2 = Component.literal("ножницами в руке или вставив в совместимый механизм").setStyle(<constant:formatting:yellow>);
  <item:energeticsheep:energetic_shears>.addTooltip(energetic_shears1);
  <item:energeticsheep:energetic_shears>.addTooltip(energetic_shears2);

// Crafting Station
  var crafting_station1 = Component.literal("Можно крафтить любые упорядоченные ").setStyle(<constant:formatting:yellow>);
  var crafting_station2 = Component.literal("и неупорядоченные рецепты").setStyle(<constant:formatting:yellow>);
  <item:craftingstation:crafting_station>.addTooltip(crafting_station1);
  <item:craftingstation:crafting_station>.addTooltip(crafting_station2);

// Draconic Evolution Chaos Shards
  var chaos_shard1 = Component.literal("Находится в центре Острова Хаоса ").setStyle(<constant:formatting:yellow>);
  var chaos_shard2 = Component.literal("в Краю после победы над").setStyle(<constant:formatting:yellow>);
  var chaos_shard3 = Component.literal("Стражем Хаоса").setStyle(<constant:formatting:yellow>);
  <item:draconicevolution:chaos_shard>.addTooltip(chaos_shard1);
  <item:draconicevolution:chaos_shard>.addTooltip(chaos_shard2);
  <item:draconicevolution:chaos_shard>.addTooltip(chaos_shard3);

// RFTools Dimensions
  var lost_knowledge1 = Component.literal("Выпадает из странников Края и злых мобов или из сундуков измерений RFTools").withStyle(style => style.withColor(0xf9ffff));
  // var lost_knowledge2 = Component.literal("or found in RFTools Dimension Loot Chests").setStyle(<constant:formatting:yellow>);
  <item:rftoolsdim:common_lost_knowledge>.addTooltip(lost_knowledge1);
 // <item:rftoolsdim:common_lost_knowledge>.addTooltip(lost_knowledge2);
  <item:rftoolsdim:uncommon_lost_knowledge>.addTooltip(lost_knowledge1);
 // <item:rftoolsdim:uncommon_lost_knowledge>.addTooltip(lost_knowledge2);
  <item:rftoolsdim:rare_lost_knowledge>.addTooltip(lost_knowledge1);
 // <item:rftoolsdim:rare_lost_knowledge>.addTooltip(lost_knowledge2);
  <item:rftoolsdim:legendary_lost_knowledge>.addTooltip(lost_knowledge1);
 // <item:rftoolsdim:legendary_lost_knowledge>.addTooltip(lost_knowledge2);

// Mystical Agriculture
  var inferium_essence = Component.literal("Выпадает из лаймовой листвы").setStyle(<constant:formatting:yellow>);
  <item:mysticalagriculture:inferium_essence>.addTooltip(inferium_essence);

// Gateways
// Ghast Cow and Wither
var exploding_boss_gateway = Component.literal("Внимание! Призывает взрывающегося босса").setStyle(<constant:formatting:red>);
val exploding_mobs = ["gateways:normal/ghast_cow", "gateways:titan/ghast_cow", "gateways:normal/wither", "gateways:titan/wither"];
<item:gateways:gate_pearl>.onlyIf("wither_pearl", stack => {
  val gateway = stack.tag["gateway"];
  return (gateway as string) in exploding_mobs;
}).addTooltip(exploding_boss_gateway);

// General Bosses
var boss_mob_gateway = Component.literal("Внимание! Призывает босса").setStyle(<constant:formatting:red>);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:normal/ender_dragon"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:titan/ender_dragon"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:normal/warden"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:titan/warden"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:normal/mother_lava_squid"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:titan/mother_lava_squid"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:normal/elder_guardian"}).addTooltip(boss_mob_gateway);
<item:gateways:gate_pearl>.withTag({gateway: "gateways:titan/elder_guardian"}).addTooltip(boss_mob_gateway);

// Ugly Steel Plating
var ugly_steel_plating = Component.literal("Закрывает блендеры и принтеры для повышения FPS").setStyle(<constant:formatting:yellow>);
  <item:forbiddensmoothies:ugly_steel_plating>.addTooltip(ugly_steel_plating);

// Dimensional Painting
var overworld_painting = Component.literal("Телепортирует в Верхний мир").setStyle(<constant:formatting:yellow>);
<item:dimpaintings:overworld_painting>.addTooltip(overworld_painting);

var nether_painting = Component.literal("Телепортирует в Незер").setStyle(<constant:formatting:yellow>);
<item:dimpaintings:nether_painting>.addTooltip(nether_painting);

var end_painting = Component.literal("Телепортирует в Край").setStyle(<constant:formatting:yellow>);
<item:dimpaintings:end_painting>.addTooltip(end_painting);

// EnderIO Conduit Probe
var conduit_probe = Component.literal("Shift + колесо мыши для смены режима").setStyle(<constant:formatting:yellow>);
<item:enderio:conduit_probe>.addTooltip(conduit_probe);
