using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateStats : MonoBehaviour
{
    public enum Modifier
    {
        //additive
        strike, projectile, pierce, chain,
        base_damage, base_projectile_damage, base_melee_damage, base_physical_damage, base_fire_damage, base_cold_damage, base_lightning_damage,
        critical_chance, projectile_critical_chance, melee_critical_chance, critical_damage, projectile_critical_damage, melee_critical_damage,
        regen, degen, life_steal_chance, projectile_life_steal_chance, melee_life_steal_chance, life_steal, projectile_life_steal, melee_life_steal,
        bleed_chance, burn_chance, chill_chance, shock_chance,
        bleed_effect, burn_effect, chill_effect, shock_effect,
        knockback,
        //multiplier
        damage, projectile_damage, melee_damage, physical_damage, fire_damage, cold_damage, lightning_damage,
        attack_range, projectile_attack_range, melee_attack_range,
        travel_range, projectile_travel_range, melee_travel_range,
        travel_speed, projectile_travel_speed, melee_travel_speed,
        cooldown, projectile_cooldown, melee_cooldown,
        movement_speed,
        max_health,
        defense,
        magnet_range,
        size, projectile_size, melee_size,
    }
    public enum EnemyModifier
    {
        move_speed, max_health, damage, attack_cooldown, projectile, projectile_travel_speed, projectile_size,
        physical_resistance, fire_resistance, cold_resistance, lightning_resistance
    }
    public static UpdateStats instance;
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    public EnemyManager enemyManager;
    float currentMaxHP;
    public void Awake()
    {
        instance = this;
    }
    public static void ApplyGlobalUpgrades(Upgrades upgrade, bool unapplyUpgrades)
    {
        float x;
        int lv;
        if (upgrade.itemDescription != null)
            lv = upgrade.itemDescription.currentLevel;
        else
            lv = 0;
        instance.currentMaxHP = instance.gameplayManager.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[lv].modifier.Count; i++)
        {
            if (unapplyUpgrades)
                x = -upgrade.levelModifiersList[lv].amt[i];
            else
                x = upgrade.levelModifiersList[lv].amt[i];
            switch (upgrade.levelModifiersList[lv].modifier[i])
            {
                case Modifier.attack_range: instance.gameplayManager.attackRangeMultiplier += x; break;
                case Modifier.base_cold_damage: instance.gameplayManager.baseDamageTypeAdditive[2] += x; break;
                case Modifier.base_damage: instance.gameplayManager.baseDamageAdditive += x; break;
                case Modifier.base_fire_damage: instance.gameplayManager.baseDamageTypeAdditive[1] += x; break;
                case Modifier.base_lightning_damage: instance.gameplayManager.baseDamageTypeAdditive[3] += x; break;
                case Modifier.base_physical_damage: instance.gameplayManager.baseDamageTypeAdditive[0] += x; break;
                case Modifier.base_melee_damage: instance.gameplayManager.baseMeleeDamageAdditive += x; break;
                case Modifier.base_projectile_damage: instance.gameplayManager.baseProjectileDamageAdditive += x; break;
                case Modifier.bleed_chance: instance.gameplayManager.ailmentsChanceAdditive[0] += x; break;
                case Modifier.bleed_effect: instance.gameplayManager.ailmentsEffectMultiplier[0] += x; break;
                case Modifier.burn_chance: instance.gameplayManager.ailmentsChanceAdditive[1] += x; break;
                case Modifier.burn_effect: instance.gameplayManager.ailmentsEffectMultiplier[1] += x; break;
                case Modifier.chain: instance.gameplayManager.chainAdditive += (int)x; break;
                case Modifier.chill_chance: instance.gameplayManager.ailmentsChanceAdditive[2] += x; break;
                case Modifier.chill_effect: instance.gameplayManager.ailmentsEffectMultiplier[2] += x; break;
                case Modifier.cold_damage: instance.gameplayManager.damageTypeMultiplier[2] += x; break;
                case Modifier.cooldown: instance.gameplayManager.cooldownMultiplier += x; break;
                case Modifier.critical_chance: instance.gameplayManager.criticalChanceAdditive += x; break;
                case Modifier.critical_damage: instance.gameplayManager.criticalDamageAdditive += x; break;
                case Modifier.damage: instance.gameplayManager.damageMultiplier += x; break;
                case Modifier.defense: instance.gameplayManager.defenseMultiplier += x; break;
                case Modifier.degen: instance.gameplayManager.degenAdditive += x; break;
                case Modifier.fire_damage: instance.gameplayManager.damageTypeMultiplier[1] += x; break;
                case Modifier.life_steal: instance.gameplayManager.lifeStealAdditive += x; break;
                case Modifier.life_steal_chance: instance.gameplayManager.lifeStealChanceAdditive += x; break;
                case Modifier.lightning_damage: instance.gameplayManager.damageTypeMultiplier[3] += x; break;
                case Modifier.magnet_range: instance.gameplayManager.magnetRangeMultiplier += x; break;
                case Modifier.max_health: instance.gameplayManager.maxHealthMultiplier += x; break;
                case Modifier.melee_attack_range: instance.gameplayManager.meleeAttackRangeMultiplier += x; break;
                case Modifier.melee_cooldown: instance.gameplayManager.meleeCooldownMultiplier += x; break;
                case Modifier.melee_critical_chance: instance.gameplayManager.meleeCriticalChanceAdditive += x; break;
                case Modifier.melee_critical_damage: instance.gameplayManager.meleeCriticalDamageAdditive += x; break;
                case Modifier.melee_damage: instance.gameplayManager.meleeDamageMultiplier += x; break;
                case Modifier.melee_life_steal: instance.gameplayManager.meleeLifeStealAdditive += x; break;
                case Modifier.melee_life_steal_chance: instance.gameplayManager.meleeLifeStealChanceAdditive += x; break;
                case Modifier.melee_size: instance.gameplayManager.meleeSizeMultiplier += x; break;
                case Modifier.melee_travel_range: instance.gameplayManager.meleeTravelRangeMultipiler += x; break;
                case Modifier.melee_travel_speed: instance.gameplayManager.meleeTravelSpeedMultipiler += x; break;
                case Modifier.movement_speed: instance.gameplayManager.moveSpeedMultiplier += x; break;
                case Modifier.physical_damage: instance.gameplayManager.damageTypeMultiplier[0] += x; break;
                case Modifier.pierce: instance.gameplayManager.pierceAdditive += (int)x; break;
                case Modifier.projectile: instance.gameplayManager.projectileAdditive += (int)x; break;
                case Modifier.projectile_attack_range: instance.gameplayManager.projectileAttackRangeMultiplier += x; break;
                case Modifier.projectile_cooldown: instance.gameplayManager.projectileCooldownMultiplier += x; break;
                case Modifier.projectile_critical_chance: instance.gameplayManager.projectileCriticalChanceAdditive += x; break;
                case Modifier.projectile_critical_damage: instance.gameplayManager.projectileCriticalDamageAdditive += x; break;
                case Modifier.projectile_damage: instance.gameplayManager.projectileDamageMultiplier += x; break;
                case Modifier.projectile_life_steal: instance.gameplayManager.projectileLifeStealAdditive += x; break;
                case Modifier.projectile_life_steal_chance: instance.gameplayManager.projectileLifeStealChanceAdditive += x; break;
                case Modifier.projectile_size: instance.gameplayManager.projectileSizeMultiplier += x; break;
                case Modifier.projectile_travel_range: instance.gameplayManager.projectileTravelRangeMultipiler += x; break;
                case Modifier.projectile_travel_speed: instance.gameplayManager.projectileTravelSpeedMultipiler += x; break;
                case Modifier.regen: instance.gameplayManager.regenAdditive += x; break;
                case Modifier.shock_chance: instance.gameplayManager.ailmentsChanceAdditive[3] += x; break;
                case Modifier.shock_effect: instance.gameplayManager.ailmentsEffectMultiplier[3] += x; break;
                case Modifier.size: instance.gameplayManager.sizeMultiplier += x; break;
                case Modifier.strike: instance.gameplayManager.strikeAdditive += (int)x; break;
                case Modifier.travel_range: instance.gameplayManager.travelRangeMultipiler += x; break;
                case Modifier.travel_speed: instance.gameplayManager.travelSpeedMultipiler += x; break;
                default: Debug.Log("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[lv].modifier[i]); break;
            }
        }
        instance.gameplayManager.player.UpdatePlayerStats();
        for (int i = 0; i < instance.gameplayManager.inventory.activeSkillList.Count; i++) //update all active skills
        {
            if (instance.gameplayManager.inventory.activeSkillList[i].skillController != null)
            {
                instance.gameplayManager.inventory.activeSkillList[i].skillController.UpdateSkillStats();
            }
        }
        if (instance.currentMaxHP != instance.gameplayManager.player.maxHealth) //if hp is upgraded, update current hp.
        {
            instance.gameplayManager.player.currentHealth += instance.gameplayManager.player.maxHealth - instance.currentMaxHP;
            instance.gameplayManager.player.UpdateHealthBar();
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
                case Modifier.attack_range: skill.addedAttackRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.base_cold_damage: skill.addedBaseDamageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.base_damage: skill.addedBaseDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.base_fire_damage: skill.addedBaseDamageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.base_lightning_damage: skill.addedBaseDamageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.base_physical_damage: skill.addedBaseDamageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.bleed_chance: skill.addedAilmentsChance[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.bleed_effect: skill.addedAilmentsEffect[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.burn_chance: skill.addedAilmentsChance[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.burn_effect: skill.addedAilmentsEffect[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.chain: skill.addedChain += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.chill_chance: skill.addedAilmentsChance[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.chill_effect: skill.addedAilmentsEffect[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.cold_damage: skill.addedDamageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.cooldown: skill.addedCooldown += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.critical_chance: skill.addedCriticalChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.critical_damage: skill.addedCriticalDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.damage: skill.addedDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.fire_damage: skill.addedDamageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.knockback: skill.addedKnockBack += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.life_steal: skill.addedLifeSteal += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.life_steal_chance: skill.addedLifeStealChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.lightning_damage: skill.addedDamageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.physical_damage: skill.addedDamageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.pierce: skill.addedPierce += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.projectile: skill.addedProjectile += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.shock_chance: skill.addedAilmentsChance[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.shock_effect: skill.addedAilmentsEffect[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.size: skill.addedSize += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.strike: skill.addedStrike += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.travel_range: skill.addedTravelRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.travel_speed: skill.addedTravelSpeed += upgrade.levelModifiersList[level].amt[i]; break;
                default: Debug.Log("ApplySkillMod has no switch case for " + upgrade.levelModifiersList[level].modifier[i]); break;
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
                case Modifier.attack_range: skill.addedAttackRange += x; break;
                case Modifier.bleed_chance: skill.addedAilmentsChance[0] += x; break;
                case Modifier.bleed_effect: skill.addedAilmentsEffect[0] += x; break;
                case Modifier.burn_chance: skill.addedAilmentsChance[1] += x; break;
                case Modifier.burn_effect: skill.addedAilmentsEffect[1] += x; break;
                case Modifier.chain: skill.addedChain += (int)x; break;
                case Modifier.chill_chance: skill.addedAilmentsChance[2] += x; break;
                case Modifier.chill_effect: skill.addedAilmentsEffect[2] += x; break;
                case Modifier.cold_damage: skill.addedDamageTypes[2] += x; break;
                case Modifier.cooldown: skill.addedCooldown += x; break;
                case Modifier.critical_chance: skill.addedCriticalChance += x; break;
                case Modifier.critical_damage: skill.addedCriticalDamage += x; break;
                case Modifier.damage: skill.addedDamage += x; break;
                case Modifier.fire_damage: skill.addedDamageTypes[1] += x; break;
                case Modifier.knockback: skill.addedKnockBack += x; break;
                case Modifier.life_steal: skill.addedLifeSteal += x; break;
                case Modifier.life_steal_chance: skill.addedLifeStealChance += x; break;
                case Modifier.lightning_damage: skill.addedDamageTypes[3] += x; break;
                case Modifier.physical_damage: skill.addedDamageTypes[0] += x; break;
                case Modifier.pierce: skill.addedPierce += (int)x; break;
                case Modifier.projectile: skill.addedProjectile += (int)x; break;
                case Modifier.shock_chance: skill.addedAilmentsChance[3] += x; break;
                case Modifier.shock_effect: skill.addedAilmentsEffect[3] += x; break;
                case Modifier.size: skill.addedSize += x; break; //doesn't have baseSize
                case Modifier.strike: skill.addedStrike += (int)x; break;
                case Modifier.travel_range: skill.addedTravelRange += x; break;
                case Modifier.travel_speed: skill.addedTravelSpeed += x; break;
                default: Debug.Log("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    public static void ApplyEnemyUpgrades(EnemyUpgradeManager.EnemyUpgrades upgrade)
    {
        for (int i = 0; i < upgrade.modifier.Count; i++)
        {
            switch (upgrade.modifier[i])
            {
                case EnemyModifier.attack_cooldown: instance.gameplayManager.enemyAttackCooldownMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.cold_resistance: instance.gameplayManager.enemyResistances[2] += upgrade.amt[i]; break;
                case EnemyModifier.damage: instance.gameplayManager.enemyDamageMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.fire_resistance: instance.gameplayManager.enemyResistances[1] += upgrade.amt[i]; break;
                case EnemyModifier.lightning_resistance: instance.gameplayManager.enemyResistances[3] += upgrade.amt[i]; break;
                case EnemyModifier.max_health: instance.gameplayManager.enemyMaxHealthMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.move_speed: instance.gameplayManager.enemyMoveSpeedMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.physical_resistance: instance.gameplayManager.enemyResistances[0] += upgrade.amt[i]; break;
                case EnemyModifier.projectile: instance.gameplayManager.enemyProjectileAdditive += upgrade.amt[i]; break;
                case EnemyModifier.projectile_size: instance.gameplayManager.enemyProjectileSizeMultiplier += upgrade.amt[i]; break;
                //case EnemyModifier.projectile_travel_range: instance.gameplayManager.enemyProjectileTravelRangeMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.projectile_travel_speed: instance.gameplayManager.enemyProjectileTravelSpeedMultiplier += upgrade.amt[i]; break;
                default: Debug.Log("ApplyEnemyUpgrades has no switch case for " + upgrade.modifier[i]); break;
            }
        }
        instance.enemyManager.UpdateAllEnemyStats();
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
                    case Modifier.attack_range: skillUpgradesString += "\tAttack Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.base_cold_damage: skillUpgradesString += "\tBase Cold Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_damage: skillUpgradesString += "\tBase Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_fire_damage: skillUpgradesString += "\tBase Fire Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_lightning_damage: skillUpgradesString += "\tBase Lightning Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_physical_damage: skillUpgradesString += "\tBase Physical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_melee_damage: skillUpgradesString += "\tBase Melee Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.base_projectile_damage: skillUpgradesString += "\tBase Projectile Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.bleed_chance: skillUpgradesString += "\tBleed Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.bleed_effect: skillUpgradesString += "\tBleed Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.burn_chance: skillUpgradesString += "\tBurn Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.burn_effect: skillUpgradesString += "\tBurn Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.chain: skillUpgradesString += "\tChain: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.chill_chance: skillUpgradesString += "\tChill Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.chill_effect: skillUpgradesString += "\tChill Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.cold_damage: skillUpgradesString += "\tCold Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.cooldown: skillUpgradesString += "\tCooldown: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.critical_chance: skillUpgradesString += "\tCritical Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.critical_damage: skillUpgradesString += "\tCritical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.damage: skillUpgradesString += "\tDamage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.fire_damage: skillUpgradesString += "\tFire Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.knockback: skillUpgradesString += "\tKnockback: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.life_steal: skillUpgradesString += "\tLife Steal: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.life_steal_chance: skillUpgradesString += "\tLife Steal Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.lightning_damage: skillUpgradesString += "\tLightning Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.physical_damage: skillUpgradesString += "\tPhysical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.pierce: skillUpgradesString += "\tPierce: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.projectile: skillUpgradesString += "\tProjectile: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.shock_chance: skillUpgradesString += "\tShock Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.shock_effect: skillUpgradesString += "\tShock Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.size: skillUpgradesString += "\tSize: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.strike: skillUpgradesString += "\tStrike: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.travel_range: skillUpgradesString += "\tTravel Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.travel_speed: skillUpgradesString += "\tTravel Speed: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    default: Debug.Log("SkillUpgradesToString has no switch case for " + upgrade.levelModifiersList[k].modifier[i]); break;
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
                    if (instance.CheckPercentModifier(stats.modifier[i]))
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
                    if (instance.CheckPercentModifier(stats.modifier[i]))
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
                    if (instance.CheckPercentModifier(stats.modifier[i]))
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
                    if (instance.CheckPercentModifier(stats.modifier[i]))
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
                    fullString = "Physical Damage: " + (sc.damageTypes[i]) + "\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: " + (sc.damageTypes[i]) + "\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: " + (sc.damageTypes[i]) + "\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: " + (sc.damageTypes[i]) + "\n";
            }
        }
        if (sc.criticalChance > 0)
            fullString += "Critical Chance: " + sc.criticalChance + "%\n";
        if (sc.criticalDamage > instance.gameplayManager.criticalDamageAdditive)
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
        if (sc.travelRange > 0)
            fullString += "Travel Range: " + sc.travelRange + "\n";
        if (sc.travelSpeed > 0)
            fullString += "Travel Speed: " + sc.travelSpeed + "\n";
        if (sc.knockBack > 0)
            fullString += "Knockback: " + sc.knockBack + "\n";
        if (sc.size > 0)
            fullString += "Size: " + sc.size * 100 + "%\n";

        return fullString;
    }
    public static void FormatPlayerStatsToString()
    {
        instance.inventoryManager.playerStats1Text.text = string.Empty;
        string fullString = "";
        fullString += "Health: " + instance.gameplayManager.player.currentHealth + "/" + instance.gameplayManager.player.maxHealth + " (+" + instance.gameplayManager.maxHealthMultiplier + "%)\n";
        fullString += "Defense: " + instance.gameplayManager.player.defense + " (+" + instance.gameplayManager.defenseMultiplier + "%)\n";
        fullString += "Regen: " + instance.gameplayManager.player.regen + "\n";
        fullString += "Degen: " + instance.gameplayManager.player.degen + "\n";
        fullString += "Movement Speed: " + instance.gameplayManager.player.moveSpeed + " (+" + instance.gameplayManager.moveSpeedMultiplier + "%)\n";
        fullString += "Dash: " + instance.gameplayManager.player.playerMovement.maxCharges + " (+" + instance.gameplayManager.dashChargesAdditive + ")\n";
        fullString += "Dash Cooldown: " + instance.gameplayManager.player.playerMovement.dashCooldown + " (+" + instance.gameplayManager.dashCooldownMultiplier + "%)\n";
        fullString += "Dash Power: " + instance.gameplayManager.player.playerMovement.dashPower + " (+" + instance.gameplayManager.dashPowerMultiplier + "%)\n";
        fullString += "Magnet Range: " + instance.gameplayManager.player.magnetRange + " (+" + instance.gameplayManager.magnetRangeMultiplier + "%)\n";
        fullString += "Exp: " + instance.gameplayManager.exp + "/" + instance.gameplayManager.expCap + " (+" + instance.gameplayManager.expMultiplier + "%)\n";
        if (instance.gameplayManager.coinDropChanceMultipiler > 0)
            fullString += "Coin Drop Chance: +" + instance.gameplayManager.coinDropChanceMultipiler + "%\n";
        if (instance.gameplayManager.dropChanceMultiplier > 0)
            fullString += "Drop Chance: +" + instance.gameplayManager.dropChanceMultiplier + "%\n";
        instance.inventoryManager.playerStats1Text.text = fullString;
        fullString = string.Empty;
        //Second stats
        if (instance.gameplayManager.baseDamageAdditive > 0)
            fullString += "Base Damage: +" + instance.gameplayManager.baseDamageAdditive + "\n";
        if (instance.gameplayManager.baseProjectileDamageAdditive > 0)
            fullString += "Base Projectile Damage: +" + instance.gameplayManager.baseProjectileDamageAdditive + "\n";
        if (instance.gameplayManager.baseMeleeDamageAdditive > 0)
            fullString += "Base Melee Damage: +" + instance.gameplayManager.baseMeleeDamageAdditive + "\n";
        for (int i = 0; i < instance.gameplayManager.baseDamageTypeAdditive.Count; i++)
        {
            if (instance.gameplayManager.baseDamageTypeAdditive[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Base Physical Damage: +" + instance.gameplayManager.baseDamageTypeAdditive[i] + "\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Base Fire Damage</color>: +" + instance.gameplayManager.baseDamageTypeAdditive[i] + "\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Base Cold Damage</color>: +" + instance.gameplayManager.baseDamageTypeAdditive[i] + "\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Base Lightning Damage</color>: +" + instance.gameplayManager.baseDamageTypeAdditive[i] + "\n";
            }
        }
        if (instance.gameplayManager.damageMultiplier > 0)
            fullString += "Damage: +" + instance.gameplayManager.damageMultiplier + "%\n";
        if (instance.gameplayManager.projectileDamageMultiplier > 0)
            fullString += "Projectile Damage: +" + instance.gameplayManager.projectileDamageMultiplier + "%\n";
        if (instance.gameplayManager.meleeDamageMultiplier > 0)
            fullString += "Melee Damage: +" + instance.gameplayManager.meleeDamageMultiplier + "%\n";
        for (int i = 0; i < instance.gameplayManager.damageTypeMultiplier.Count; i++)
        {
            if (instance.gameplayManager.damageTypeMultiplier[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Damage: +" + instance.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: +" + instance.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: +" + instance.gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: +" + instance.gameplayManager.damageTypeMultiplier[i] + "%\n";
            }
        }
        if (instance.gameplayManager.criticalChanceAdditive > 0)
            fullString += "Critical Chance: +" + instance.gameplayManager.criticalChanceAdditive + "%\n";
        if (instance.gameplayManager.projectileCriticalChanceAdditive > 0)
            fullString += "Projectile Critical Chance: +" + instance.gameplayManager.projectileCriticalChanceAdditive + "%\n";
        if (instance.gameplayManager.meleeCriticalChanceAdditive > 0)
            fullString += "Melee Critical Chance: +" + instance.gameplayManager.meleeCriticalChanceAdditive + "%\n";
        if (instance.gameplayManager.criticalDamageAdditive > 0)
            fullString += "Critical Damage: +" + instance.gameplayManager.criticalDamageAdditive + "%\n";
        if (instance.gameplayManager.projectileCriticalDamageAdditive > 0)
            fullString += "Projectile Critical Damage: +" + instance.gameplayManager.projectileCriticalDamageAdditive + "%\n";
        if (instance.gameplayManager.meleeCriticalDamageAdditive > 0)
            fullString += "Melee Critical Damage: +" + instance.gameplayManager.meleeCriticalDamageAdditive + "%\n";
        if (instance.gameplayManager.cooldownMultiplier > 0)
            fullString += "Cooldown: +" + instance.gameplayManager.cooldownMultiplier + "%\n";
        if (instance.gameplayManager.projectileCooldownMultiplier > 0)
            fullString += "Projectile Cooldown: +" + instance.gameplayManager.projectileCooldownMultiplier + "%\n";
        if (instance.gameplayManager.meleeCooldownMultiplier > 0)
            fullString += "Melee Cooldown: +" + instance.gameplayManager.meleeCooldownMultiplier + "%\n";
        if (instance.gameplayManager.projectileAdditive > 0)
            fullString += "Projectile: +" + instance.gameplayManager.projectileAdditive + "\n";
        if (instance.gameplayManager.pierceAdditive > 0)
            fullString += "Pierce: +" + instance.gameplayManager.pierceAdditive + "\n";
        if (instance.gameplayManager.chainAdditive > 0)
            fullString += "Chain: +" + instance.gameplayManager.chainAdditive + "\n";
        if (instance.gameplayManager.strikeAdditive > 0)
            fullString += "Strike: +" + instance.gameplayManager.strikeAdditive + "\n";
        if (instance.gameplayManager.attackRangeMultiplier > 0)
            fullString += "Attack Range: +" + instance.gameplayManager.attackRangeMultiplier + "%\n";
        if (instance.gameplayManager.projectileAttackRangeMultiplier > 0)
            fullString += "Projectile Attack Range: +" + instance.gameplayManager.projectileAttackRangeMultiplier + "%\n";
        if (instance.gameplayManager.meleeAttackRangeMultiplier > 0)
            fullString += "Melee Attack Range: +" + instance.gameplayManager.meleeAttackRangeMultiplier + "%\n";
        for (int i = 0; i < instance.gameplayManager.ailmentsChanceAdditive.Count; i++)
        {
            if (i == 0) //physical damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Bleed Chance: +" + instance.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Bleed Effect: +" + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (+" + instance.gameplayManager.ailmentsEffectMultiplier[i] + "%)\n";
            }
            else if (i == 1) //Fire damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Burn Chance: +" + instance.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Burn Effect: +" + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (+" + instance.gameplayManager.ailmentsEffectMultiplier[i] + "%)\n";
            }
            else if (i == 2) //cold damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Chill Chance: +" + instance.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Chill Effect: +" + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (+" + instance.gameplayManager.ailmentsEffectMultiplier[i] + "%)\n";
            }
            else if (i == 3) //lightning damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Shock Chance: +" + instance.gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Shock Effect: +" + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (+" + instance.gameplayManager.ailmentsEffectMultiplier[i] + "%)\n";
            }
        }
        if (instance.gameplayManager.lifeStealChanceAdditive > 0)
            fullString += "Lifesteal Chance: +" + instance.gameplayManager.lifeStealChanceAdditive + "%\n";
        if (instance.gameplayManager.projectileLifeStealChanceAdditive > 0)
            fullString += "Projectile Lifesteal Chance: +" + instance.gameplayManager.projectileLifeStealChanceAdditive + "%\n";
        if (instance.gameplayManager.meleeLifeStealChanceAdditive > 0)
            fullString += "Melee Lifesteal Chance: +" + instance.gameplayManager.meleeLifeStealChanceAdditive + "%\n";
        if (instance.gameplayManager.lifeStealAdditive > 0)
            fullString += "Lifesteal: +" + instance.gameplayManager.lifeStealAdditive + "\n";
        if (instance.gameplayManager.projectileLifeStealAdditive > 0)
            fullString += "Projectile Lifesteal: +" + instance.gameplayManager.projectileLifeStealAdditive + "\n";
        if (instance.gameplayManager.meleeLifeStealAdditive > 0)
            fullString += "Melee Lifesteal: +" + instance.gameplayManager.meleeLifeStealAdditive + "\n";
        if (instance.gameplayManager.travelRangeMultipiler > 0)
            fullString += "Travel Range: +" + instance.gameplayManager.travelRangeMultipiler + "%\n";
        if (instance.gameplayManager.projectileTravelRangeMultipiler > 0)
            fullString += "Projectile Travel Range: +" + instance.gameplayManager.projectileTravelRangeMultipiler + "%\n";
        if (instance.gameplayManager.meleeTravelRangeMultipiler > 0)
            fullString += "Melee Travel Range: +" + instance.gameplayManager.meleeTravelRangeMultipiler + "%\n";
        if (instance.gameplayManager.travelSpeedMultipiler > 0)
            fullString += "Travel Speed: +" + instance.gameplayManager.travelSpeedMultipiler + "%\n";
        if (instance.gameplayManager.projectileTravelSpeedMultipiler > 0)
            fullString += "Projectile Travel Speed: +" + instance.gameplayManager.projectileTravelSpeedMultipiler + "%\n";
        if (instance.gameplayManager.meleeTravelSpeedMultipiler > 0)
            fullString += "Melee Travel Speed: +" + instance.gameplayManager.meleeTravelSpeedMultipiler + "%\n";
        if (instance.gameplayManager.sizeMultiplier > 0)
            fullString += "Size: +" + instance.gameplayManager.sizeMultiplier + "%\n";
        if (instance.gameplayManager.projectileSizeMultiplier > 0)
            fullString += "Projectile Size: +" + instance.gameplayManager.projectileSizeMultiplier + "%\n";
        if (instance.gameplayManager.meleeSizeMultiplier > 0)
            fullString += "Melee Size: +" + instance.gameplayManager.meleeSizeMultiplier + "%\n";
        instance.inventoryManager.playerStats2Text.text = fullString;
    }

    public static void FormatEnemyStatsToString()
    {
        instance.inventoryManager.enemyStatsText.text = string.Empty;
        string fullString = "";
        fullString += "Max Health: +" + instance.gameplayManager.enemyMaxHealthMultiplier + "%\n";
        for (int i = 0; i < instance.gameplayManager.enemyResistances.Count; i++)
        {
            if (instance.gameplayManager.enemyResistances[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString += "Physical Resistance: +" + instance.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Resistance</color>: +" + instance.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Resistance</color>: +" + instance.gameplayManager.enemyResistances[i] + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Resistance</color>: +" + instance.gameplayManager.enemyResistances[i] + "%\n";
            }
        }
        fullString += "Damage: +" + instance.gameplayManager.enemyDamageMultiplier + "%\n";
        fullString += "Movement Speed: +" + instance.gameplayManager.enemyMoveSpeedMultiplier + "%\n";
        fullString += "Attack Cooldown: +" + instance.gameplayManager.enemyAttackCooldownMultiplier + "%\n";
        fullString += "Projectile: +" + instance.gameplayManager.enemyProjectileAdditive + "\n";
        fullString += "Projectile Size: +" + instance.gameplayManager.enemyProjectileSizeMultiplier + "%\n";
        //fullString += "Projectile Travel Range: +" + instance.gameplayManager.enemyProjectileTravelRangeMultiplier + "%\n";
        fullString += "Projectile Travel Speed: +" + instance.gameplayManager.enemyProjectileTravelSpeedMultiplier + "%\n";
        instance.inventoryManager.enemyStatsText.text = fullString;
    }
    public static string FormatEnemyUpgradeToString(EnemyUpgradeManager.EnemyUpgrades mod)
    {
        string fullString = "";
        for (int i = 0; i < mod.modifier.Count; i++)
        {
            switch (mod.modifier[i])
            {
                case EnemyModifier.attack_cooldown: fullString += "Attack Cooldown: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.cold_resistance: fullString += "Cold Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.damage: fullString += "Damage: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.fire_resistance: fullString += "Fire Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.lightning_resistance: fullString += "Lightning Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.max_health: fullString += "Max Health: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.move_speed: fullString += "Movement Speed: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.physical_resistance: fullString += "Physical Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.projectile: fullString += "Projectile: +" + mod.amt[i] + "\n"; break;
                case EnemyModifier.projectile_size: fullString += "Projectile Size: +" + mod.amt[i] + "%\n"; break;
                //case EnemyModifier.projectile_travel_range: fullString += "Projectile Travel Range: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.projectile_travel_speed: fullString += "Projectile Travel Speed: +" + mod.amt[i] + "%\n"; break;
                default: Debug.Log("FormatEnemyUpgradeToString has no switch case for " + mod.modifier[i]); break;
            }
        }
        return fullString;
    }
    public bool CheckPercentModifier(Modifier mod) //Check if modifier is percent or flat value
    {
        if (mod == Modifier.strike || mod == Modifier.projectile || mod == Modifier.pierce || mod == Modifier.chain ||
            mod == Modifier.regen || mod == Modifier.degen || mod == Modifier.life_steal || mod == Modifier.knockback ||
            mod == Modifier.projectile_life_steal || mod == Modifier.melee_life_steal || 
            mod ==Modifier.base_cold_damage || mod == Modifier.base_damage || mod == Modifier.base_fire_damage || mod == Modifier.base_lightning_damage || mod == Modifier.base_physical_damage ||
            mod == Modifier.base_melee_damage || mod == Modifier.base_projectile_damage)
        {
            return true;
        }
        else return false;
    }
}
