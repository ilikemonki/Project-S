using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateStats : MonoBehaviour
{
    public static UpdateStats current;
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    float currentMaxHP;
    public void Awake()
    {
        current = this;
    }
    public static void ApplyGlobalUpgrades(Upgrades upgrade, bool unapplyUpgrades)
    {
        float x;
        int lv;
        if (upgrade.itemDescription != null)
            lv = upgrade.itemDescription.currentLevel;
        else
            lv = 0;
        current.currentMaxHP = current.gameplayManager.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[lv].modifier.Count; i++)
        {
            if (unapplyUpgrades)
                x = -upgrade.levelModifiersList[lv].amt[i];
            else
                x = upgrade.levelModifiersList[lv].amt[i];
            switch (upgrade.levelModifiersList[lv].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: current.gameplayManager.attackRangeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: current.gameplayManager.ailmentsChanceAdditive[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: current.gameplayManager.ailmentsEffectAdditive[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: current.gameplayManager.ailmentsChanceAdditive[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: current.gameplayManager.ailmentsEffectAdditive[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.chain: current.gameplayManager.chainAdditive += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: current.gameplayManager.ailmentsChanceAdditive[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: current.gameplayManager.ailmentsEffectAdditive[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: current.gameplayManager.damageTypeMultiplier[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: current.gameplayManager.cooldownMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: current.gameplayManager.criticalChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: current.gameplayManager.criticalDamageAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.damage: current.gameplayManager.damageMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.defense: current.gameplayManager.defenseMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.degen: current.gameplayManager.degenAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: current.gameplayManager.damageTypeMultiplier[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: current.gameplayManager.lifeStealAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: current.gameplayManager.lifeStealChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: current.gameplayManager.damageTypeMultiplier[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.magnet_range: current.gameplayManager.magnetRangeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.max_health: current.gameplayManager.maxHealthMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_attack_range: current.gameplayManager.meleeAttackRangeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_cooldown: current.gameplayManager.meleeCooldownMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_chance: current.gameplayManager.meleeCriticalChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_damage: current.gameplayManager.meleeCriticalDamageAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_damage: current.gameplayManager.meleeDamageMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_life_steal: current.gameplayManager.meleeLifeStealAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_life_steal_chance: current.gameplayManager.meleeLifeStealChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_size: current.gameplayManager.meleeSizeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_travel_range: current.gameplayManager.meleeTravelRangeMultipiler += x; break;
                case Upgrades.LevelModifiers.Modifier.melee_travel_speed: current.gameplayManager.meleeTravelSpeedMultipiler += x; break;
                case Upgrades.LevelModifiers.Modifier.movement_speed: current.gameplayManager.moveSpeedMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: current.gameplayManager.damageTypeMultiplier[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.pierce: current.gameplayManager.pierceAdditive += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.projectile: current.gameplayManager.projectileAdditive += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_attack_range: current.gameplayManager.projectileAttackRangeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_cooldown: current.gameplayManager.projectileCooldownMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_chance: current.gameplayManager.projectileCriticalChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_damage: current.gameplayManager.projectileCriticalDamageAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_damage: current.gameplayManager.projectileDamageMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_life_steal: current.gameplayManager.projectileLifeStealAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_life_steal_chance: current.gameplayManager.projectileLifeStealChanceAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_size: current.gameplayManager.projectileSizeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_travel_range: current.gameplayManager.projectileTravelRangeMultipiler += x; break;
                case Upgrades.LevelModifiers.Modifier.projectile_travel_speed: current.gameplayManager.projectileTravelSpeedMultipiler += x; break;
                case Upgrades.LevelModifiers.Modifier.regen: current.gameplayManager.regenAdditive += x; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: current.gameplayManager.ailmentsChanceAdditive[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: current.gameplayManager.ailmentsEffectAdditive[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.size: current.gameplayManager.sizeMultiplier += x; break;
                case Upgrades.LevelModifiers.Modifier.strike: current.gameplayManager.strikeAdditive += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: current.gameplayManager.travelRangeMultipiler += x; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: current.gameplayManager.travelSpeedMultipiler += x; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[lv].modifier[i]); break;
            }
        }
        current.gameplayManager.player.UpdatePlayerStats();
        for (int i = 0; i < current.gameplayManager.inventory.activeSkillList.Count; i++) //update all active skills
        {
            if (current.gameplayManager.inventory.activeSkillList[i].skillController != null)
            {
                current.gameplayManager.inventory.activeSkillList[i].skillController.UpdateSkillStats();
            }
        }
        if (current.currentMaxHP != current.gameplayManager.player.maxHealth) //if hp is upgraded, update current hp.
        {
            current.gameplayManager.player.currentHealth += current.gameplayManager.player.maxHealth - current.currentMaxHP;
            current.gameplayManager.player.UpdateHealthBar();
        }
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    //Upgrade skill when it levels.
    public static void ApplySkillUpgrades(Upgrades upgrade, SkillController skill, int level)
    {
        for (int i = 0; i < upgrade.levelModifiersList[level].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[level].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.addedAttackRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.addedAilmentsChance[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.addedAilmentsEffect[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.addedAilmentsChance[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.addedAilmentsEffect[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.addedChain += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.addedAilmentsChance[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.addedAilmentsEffect[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.addedDamageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.addedCooldown += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.addedCriticalChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.addedCriticalDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.addedDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.addedDamageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.knockback: skill.addedKnockBack += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.addedLifeSteal += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.addedLifeStealChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.addedDamageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.addedDamageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.addedPierce += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.addedProjectile += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.addedAilmentsChance[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.addedAilmentsEffect[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.addedSize += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.strike: skill.addedStrike += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.addedTravelRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.addedTravelSpeed += upgrade.levelModifiersList[level].amt[i]; break;
                default: GameManager.DebugLog("ApplySkillMod has no switch case for " + upgrade.levelModifiersList[level].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    //Apply gem modifiers to skill.
    public static void ApplyGemUpgrades(Upgrades upgrade, SkillController skill, bool unapplyUpgrades)
    {
        float x;
        for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
        {
            if (unapplyUpgrades)
                x = -upgrade.levelModifiersList[0].amt[i];
            else
                x = upgrade.levelModifiersList[0].amt[i];
            switch (upgrade.levelModifiersList[0].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.addedAttackRange += x; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.addedAilmentsChance[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.addedAilmentsEffect[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.addedAilmentsChance[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.addedAilmentsEffect[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.addedChain += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.addedAilmentsChance[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.addedAilmentsEffect[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.addedDamageTypes[2] += x; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.addedCooldown += x; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.addedCriticalChance += x; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.addedCriticalDamage += x; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.addedDamage += x; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.addedDamageTypes[1] += x; break;
                case Upgrades.LevelModifiers.Modifier.knockback: skill.addedKnockBack += x; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.addedLifeSteal += x; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.addedLifeStealChance += x; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.addedDamageTypes[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.addedDamageTypes[0] += x; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.addedPierce += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.addedProjectile += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.addedAilmentsChance[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.addedAilmentsEffect[3] += x; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.addedSize += x; break; //doesn't have baseSize
                case Upgrades.LevelModifiers.Modifier.strike: skill.addedStrike += (int)x; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.addedTravelRange += x; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.addedTravelSpeed += x; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    public static string FormatSkillUpgradesToString(Upgrades upgrade) //Format skill level up stats to string for Tooltip.
    {
        string skillUpgradesString = "";
        for (int k = 0; k < upgrade.levelModifiersList.Count; k++) //number of level ups (4)
        {
            skillUpgradesString += "Lv. " + (k + 2) + "\n";
            for (int i = 0; i < upgrade.levelModifiersList[k].modifier.Count; i++) //number of upgrades
            {
                switch (upgrade.levelModifiersList[k].modifier[i])
                {
                    case Upgrades.LevelModifiers.Modifier.attack_range: skillUpgradesString += "\tAttack Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.bleed_chance: skillUpgradesString += "\tBleed Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.bleed_effect: skillUpgradesString += "\tBleed Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.burn_chance: skillUpgradesString += "\tBurn Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.burn_effect: skillUpgradesString += "\tBurn Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.chain: skillUpgradesString += "\tChain: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.chill_chance: skillUpgradesString += "\tChill Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.chill_effect: skillUpgradesString += "\tChill Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.cold_damage: skillUpgradesString += "\tCold Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.cooldown: skillUpgradesString += "\tCooldown: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.critical_chance: skillUpgradesString += "\tCritical Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.critical_damage: skillUpgradesString += "\tCritical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.damage: skillUpgradesString += "\tDamage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.fire_damage: skillUpgradesString += "\tFire Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.knockback: skillUpgradesString += "\tKnockback: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.life_steal: skillUpgradesString += "\tLife Steal: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.life_steal_chance: skillUpgradesString += "\tLife Steal Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.lightning_damage: skillUpgradesString += "\tLightning Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.physical_damage: skillUpgradesString += "\tPhysical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.pierce: skillUpgradesString += "\tPierce: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.projectile: skillUpgradesString += "\tProjectile: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.shock_chance: skillUpgradesString += "\tShock Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.shock_effect: skillUpgradesString += "\tShock Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.size: skillUpgradesString += "\tSize: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.strike: skillUpgradesString += "\tStrike: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.travel_range: skillUpgradesString += "\tTravel Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Upgrades.LevelModifiers.Modifier.travel_speed: skillUpgradesString += "\tTravel Speed: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    default: GameManager.DebugLog("SkillUpgradesToString has no switch case for " + upgrade.levelModifiersList[k].modifier[i]); break;
                }
            }
        }
        return skillUpgradesString;
    }
    public static string FormatItemUpgradeStatsToString(Upgrades.LevelModifiers stats)
    {
        string statNameString = "";
        string fullString = "";
        for (int i = 0; i < stats.modifier.Count; i++)
        {
            statNameString = stats.modifier[i].ToString();
            if (statNameString.Contains("_"))
            {
                statNameString = statNameString.Replace('_', ' ');
            }
            statNameString = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(statNameString.ToLower()); //Capitalizes each word
            if (stats.amt[i] > 0) //if positive, green w/ + sign.
            {
                if (fullString.Equals("")) //For first modifier only
                {
                    if (current.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = statNameString + ": " + "<color=green>+" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = statNameString + ": " + "<color=green>+" + stats.amt[i] + "%</color>";
                    }
                }
                else
                {
                    if (current.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=green>+" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=green>+" + stats.amt[i] + "%</color>";
                    }
                }
            }
            else //red for negative
            {
                if (fullString.Equals("")) //For first modifier only
                {
                    if (current.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = statNameString + ": " + "<color=red>" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = statNameString + ": " + "<color=red>" + stats.amt[i] + "</color>";
                    }
                }
                else
                {
                    if (current.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=red>" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=red>" + stats.amt[i] + "%</color>";
                    }
                }
            }
        }
        return fullString;
    }
    public static string FormatSkillStatsToString(SkillController sc) //Return stats of the skill controller as string.
    {
        string fullString = "";
        for(int i = 0; i < sc.damageTypes.Count; i++)
        {
            if (sc.damageTypes[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Damage: " + (sc.baseDamageTypes[i] * (1 + (current.gameplayManager.damageTypeMultiplier[i] + sc.damage) / 100)) + "\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: " + (sc.baseDamageTypes[i] * (1 + (current.gameplayManager.damageTypeMultiplier[i] + sc.damage) / 100)) + "\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: " + (sc.baseDamageTypes[i] * (1 + (current.gameplayManager.damageTypeMultiplier[i] + sc.damage) / 100)) + "\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: " + (sc.baseDamageTypes[i] * (1 + (current.gameplayManager.damageTypeMultiplier[i] + sc.damage) / 100)) + "\n";
            }
        }
        if (sc.criticalChance > 0)
            fullString += "Critical Chance: " + sc.criticalChance + "%\n";
        if (sc.criticalDamage > current.gameplayManager.criticalDamageAdditive)
            fullString += "Critical Damage: " + sc.criticalDamage + "%\n";
        if (sc.cooldown > 0)
            fullString += "Cooldown: " + sc.cooldown + "s\n";
        if (sc.projectile > 0)
            fullString += "Projectile: " + sc.projectile + "\n";
        if (sc.pierce > 0)
            fullString += "Pierce: " + sc.pierce + "\n";
        if (sc.chain > 0)
            fullString += "Chain: " + sc.chain + "\n";
        if (sc.strike > 0)
            fullString += "Strike: " + sc.strike + "\n";
        if (sc.attackRange > 0)
            fullString += "Attack Range: " + sc.attackRange + "%\n";
        for (int i = 0; i < sc.ailmentsChance.Count; i++)
        {
            if (sc.ailmentsChance[i] > 0)
            {
                if (i == 0) //physical damage
                {
                    fullString += "Bleed Chance: " + sc.ailmentsChance[i] + "%\n"; 
                    if (sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) >= 1)
                        fullString += "Bleed Damage: " + sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) + "\n";
                    else
                        fullString += "Bleed Damage: " + 1 + "\n";
                }
                else if (i == 1) //Fire damage
                {
                    fullString += "Burn Chance: " + sc.ailmentsChance[i] + "%\n";
                    if (sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) >= 1)
                        fullString += "Burn Damage: " + sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) + "\n";
                    else
                        fullString += "Burn Damage: " + 1 + "\n"; ;
                }
                else if (i == 2) //cold damage
                {
                    fullString += "Chill Chance: " + sc.ailmentsChance[i] + "%\n"; fullString += "Chill Effect: " + sc.ailmentsEffect[i] + "%\n";
                }
                else if (i == 3) //lightning damage
                {
                    fullString += "Shock Chance: " + sc.ailmentsChance[i] + "%\n"; fullString += "Shock Effect: " + sc.ailmentsEffect[i] + "%\n";
                }
            }
        }
        if (sc.lifeStealChance > 0)
            fullString += "Lifesteal Chance: " + sc.lifeStealChance + "%\n";
        if (sc.lifeSteal > 0)
            fullString += "Lifesteal: " + sc.lifeSteal + "\n";
        if (sc.travelSpeed > 0)
            fullString += "Travel Speed: " + sc.travelSpeed + "\n";
        if (sc.travelRange > 0)
            fullString += "Travel Range: " + sc.travelRange + "\n";
        if (sc.knockBack > 0)
            fullString += "Knockback: " + sc.knockBack + "\n";
        if (sc.size > 0)
            fullString += "Size: " + sc.size * 100 + "%\n";

        return fullString;
    }
    public static void FormatPlayerStatsToString()
    {
        current.inventoryManager.playerStats1Text.text = string.Empty;
        string fullString = "";
        fullString += "Health: " + current.gameplayManager.player.currentHealth + "/" + current.gameplayManager.player.maxHealth + " (+" + current.gameplayManager.maxHealthMultiplier + "%)\n";
        fullString += "Defense: " + current.gameplayManager.player.defense + " (+" + current.gameplayManager.defenseMultiplier + "%)\n";
        fullString += "Regen: " + current.gameplayManager.player.regen + "\n";
        fullString += "Degen: " + current.gameplayManager.player.degen + "\n";
        fullString += "Movement Speed: " + current.gameplayManager.player.moveSpeed + " (+" + current.gameplayManager.moveSpeedMultiplier + "%)\n";
        fullString += "Dash: " + current.gameplayManager.player.playerMovement.maxCharges + "\n";
        fullString += "Dash Cooldown: " + current.gameplayManager.player.playerMovement.dashCooldown + " (+" + current.gameplayManager.dashCooldownMultiplier + "%)\n";
        fullString += "Dash Power: " + current.gameplayManager.player.playerMovement.dashPower + " (+" + current.gameplayManager.dashPowerMultiplier + "%)\n";
        fullString += "Magnet Range: " + current.gameplayManager.player.magnetRange + " (+" + current.gameplayManager.magnetRangeMultiplier + "%)\n";
        fullString += "Exp: " + current.gameplayManager.exp + "/" + current.gameplayManager.expCap + "\n";
        current.inventoryManager.playerStats1Text.text = fullString;
        fullString = string.Empty;
        //Second stats
        if (current.gameplayManager.damageMultiplier > 0)
            fullString += "Damage: +" + current.gameplayManager.damageMultiplier + "%\n";
        if (current.gameplayManager.projectileDamageMultiplier > 0)
            fullString += "Projectile Damage: +" + current.gameplayManager.projectileDamageMultiplier + "%\n";
        if (current.gameplayManager.meleeDamageMultiplier > 0)
            fullString += "Melee Damage: +" + current.gameplayManager.meleeDamageMultiplier + "%\n";
        for (int i = 0; i < current.gameplayManager.damageTypeMultiplier.Count; i++)
        {
            if (current.gameplayManager.damageTypeMultiplier[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Damage: +" + current.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: +" + current.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: +" + current.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: +" + current.gameplayManager.damageTypeMultiplier[i] + "%\n";
            }
        }
        if (current.gameplayManager.criticalChanceAdditive > 0)
            fullString += "Critical Chance: +" + current.gameplayManager.criticalChanceAdditive + "%\n";
        if (current.gameplayManager.projectileCriticalChanceAdditive > 0)
            fullString += "Projectile Critical Chance: +" + current.gameplayManager.projectileCriticalChanceAdditive + "%\n";
        if (current.gameplayManager.meleeCriticalChanceAdditive > 0)
            fullString += "Melee Critical Chance: +" + current.gameplayManager.meleeCriticalChanceAdditive + "%\n";
        if (current.gameplayManager.criticalDamageAdditive > 0)
            fullString += "Critical Damage: +" + current.gameplayManager.criticalDamageAdditive + "%\n";
        if (current.gameplayManager.projectileCriticalDamageAdditive > 0)
            fullString += "Projectile Critical Damage: +" + current.gameplayManager.projectileCriticalDamageAdditive + "%\n";
        if (current.gameplayManager.meleeCriticalDamageAdditive > 0)
            fullString += "Melee Critical Damage: +" + current.gameplayManager.meleeCriticalDamageAdditive + "%\n";
        if (current.gameplayManager.cooldownMultiplier > 0)
            fullString += "Cooldown: +" + current.gameplayManager.cooldownMultiplier + "%\n";
        if (current.gameplayManager.projectileCooldownMultiplier > 0)
            fullString += "Projectile Cooldown: +" + current.gameplayManager.projectileCooldownMultiplier + "%\n";
        if (current.gameplayManager.meleeCooldownMultiplier > 0)
            fullString += "Melee Cooldown: +" + current.gameplayManager.meleeCooldownMultiplier + "%\n";
        if (current.gameplayManager.projectileAdditive > 0)
            fullString += "Projectile: +" + current.gameplayManager.projectileAdditive + "\n";
        if (current.gameplayManager.pierceAdditive > 0)
            fullString += "Pierce: +" + current.gameplayManager.pierceAdditive + "\n";
        if (current.gameplayManager.chainAdditive > 0)
            fullString += "Chain: +" + current.gameplayManager.chainAdditive + "\n";
        if (current.gameplayManager.strikeAdditive > 0)
            fullString += "Strike: +" + current.gameplayManager.strikeAdditive + "\n";
        if (current.gameplayManager.attackRangeMultiplier > 0)
            fullString += "Attack Range: +" + current.gameplayManager.attackRangeMultiplier + "%\n";
        if (current.gameplayManager.projectileAttackRangeMultiplier > 0)
            fullString += "Projectile Attack Range: +" + current.gameplayManager.projectileAttackRangeMultiplier + "%\n";
        if (current.gameplayManager.meleeAttackRangeMultiplier > 0)
            fullString += "Melee Attack Range: +" + current.gameplayManager.meleeAttackRangeMultiplier + "%\n";
        for (int i = 0; i < current.gameplayManager.ailmentsChanceAdditive.Count; i++)
        {
            if (i == 0) //physical damage
            {
                if (current.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Bleed Chance: +" + current.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Bleed Effect: +" + current.gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 1) //Fire damage
            {
                if (current.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Burn Chance: +" + current.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Burn Effect: +" + current.gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 2) //cold damage
            {
                if (current.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Chill Chance: +" + current.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Chill Effect: +" + current.gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 3) //lightning damage
            {
                if (current.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Shock Chance: +" + current.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Shock Effect: +" + current.gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
        }
        if (current.gameplayManager.lifeStealChanceAdditive > 0)
            fullString += "Lifesteal Chance: +" + current.gameplayManager.lifeStealChanceAdditive + "%\n";
        if (current.gameplayManager.projectileLifeStealChanceAdditive > 0)
            fullString += "Projectile Lifesteal Chance: +" + current.gameplayManager.projectileLifeStealChanceAdditive + "%\n";
        if (current.gameplayManager.meleeLifeStealChanceAdditive > 0)
            fullString += "Melee Lifesteal Chance: +" + current.gameplayManager.meleeLifeStealChanceAdditive + "%\n";
        if (current.gameplayManager.lifeStealAdditive > 0)
            fullString += "Lifesteal: +" + current.gameplayManager.lifeStealAdditive + "\n";
        if (current.gameplayManager.projectileLifeStealAdditive > 0)
            fullString += "Projectile Lifesteal: +" + current.gameplayManager.projectileLifeStealAdditive + "\n";
        if (current.gameplayManager.meleeLifeStealAdditive > 0)
            fullString += "Melee Lifesteal: +" + current.gameplayManager.meleeLifeStealAdditive + "\n";
        if (current.gameplayManager.travelSpeedMultipiler > 0)
            fullString += "Travel Speed: +" + current.gameplayManager.travelSpeedMultipiler + "%\n";
        if (current.gameplayManager.projectileTravelSpeedMultipiler > 0)
            fullString += "Projectile Travel Speed: +" + current.gameplayManager.projectileTravelSpeedMultipiler + "%\n";
        if (current.gameplayManager.meleeTravelSpeedMultipiler > 0)
            fullString += "Melee Travel Speed: +" + current.gameplayManager.meleeTravelSpeedMultipiler + "%\n";
        if (current.gameplayManager.travelRangeMultipiler > 0)
            fullString += "Travel Range: +" + current.gameplayManager.travelRangeMultipiler + "%\n";
        if (current.gameplayManager.projectileTravelRangeMultipiler > 0)
            fullString += "Projectile Travel Range: +" + current.gameplayManager.projectileTravelRangeMultipiler + "%\n";
        if (current.gameplayManager.meleeTravelRangeMultipiler > 0)
            fullString += "Melee Travel Range: +" + current.gameplayManager.meleeTravelRangeMultipiler + "%\n";
        if (current.gameplayManager.sizeMultiplier > 0)
            fullString += "Size: +" + current.gameplayManager.sizeMultiplier + "%\n";
        if (current.gameplayManager.projectileSizeMultiplier > 0)
            fullString += "Projectile Size: +" + current.gameplayManager.projectileSizeMultiplier + "%\n";
        if (current.gameplayManager.meleeSizeMultiplier > 0)
            fullString += "Melee Size: +" + current.gameplayManager.meleeSizeMultiplier + "%\n";
        current.inventoryManager.playerStats2Text.text = fullString;
    }

    public static void FormatEnemyStatsToString()
    {
        current.inventoryManager.enemyStatsText.text = string.Empty;
        string fullString = "";
        fullString += "Health: +" + current.gameplayManager.enemyMaxHealthMultiplier + "%\n";
        for (int i = 0; i < current.gameplayManager.enemyResistances.Count; i++)
        {
            if (current.gameplayManager.enemyResistances[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Resistance: +" + current.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Resistance</color>: +" + current.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Resistance</color>: +" + current.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Resistance</color>: +" + current.gameplayManager.enemyResistances[i] + "%\n";
            }
        }
        fullString += "Damage: +" + current.gameplayManager.enemyDamageMultiplier + "%\n";
        fullString += "Movement Speed: +" + current.gameplayManager.enemyMoveSpeedMultiplier + "%\n";
        fullString += "Attack Cooldown: +" + current.gameplayManager.enemyAttackCooldownMultiplier + "%\n";
        fullString += "Projectile Travel Speed: +" + current.gameplayManager.enemyProjectileTravelSpeedMultiplier + "%\n";
        fullString += "Projectile Travel Range: +" + current.gameplayManager.enemyProjectileTravelRangeMultiplier + "%\n";
        fullString += "Exp: +" + current.gameplayManager.enemyExpMultiplier + "%\n";
        if (current.gameplayManager.coinDropChanceMultipiler > 0)
            fullString += "Coin Drop Chance: +" + current.gameplayManager.coinDropChanceMultipiler + "%\n";
        if (current.gameplayManager.dropChanceMultiplier > 0)
            fullString += "Drop Chance: +" + current.gameplayManager.dropChanceMultiplier + "%\n";
        current.inventoryManager.enemyStatsText.text = fullString;
    }
    public bool CheckPercentModifier(Upgrades.LevelModifiers.Modifier mod) //Check if modifier is percent or flat value
    {
        if (mod == Upgrades.LevelModifiers.Modifier.strike || mod == Upgrades.LevelModifiers.Modifier.projectile || mod == Upgrades.LevelModifiers.Modifier.pierce || mod == Upgrades.LevelModifiers.Modifier.chain ||
            mod == Upgrades.LevelModifiers.Modifier.regen || mod == Upgrades.LevelModifiers.Modifier.degen || mod == Upgrades.LevelModifiers.Modifier.life_steal || mod == Upgrades.LevelModifiers.Modifier.knockback)
        {
            return true;
        }
        else return false;
    }
}
