using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateStats : MonoBehaviour
{
    public GameplayManager gameplayManager;
    float currentMaxHP;
    public void ApplyGlobalUpgrades(Upgrades upgrade)
    {
        currentMaxHP = gameplayManager.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: gameplayManager.attackRangeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: gameplayManager.ailmentsChanceAdditive[0] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: gameplayManager.ailmentsEffectAdditive[0] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: gameplayManager.ailmentsChanceAdditive[1] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: gameplayManager.ailmentsEffectAdditive[1] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: gameplayManager.chainAdditive += (int)upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: gameplayManager.ailmentsChanceAdditive[2] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: gameplayManager.ailmentsEffectAdditive[2] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: gameplayManager.damageTypeMultiplier[2] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: gameplayManager.cooldownMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: gameplayManager.criticalChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: gameplayManager.criticalDamageAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: gameplayManager.damageMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.defense: gameplayManager.defenseMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.degen: gameplayManager.degenAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: gameplayManager.damageTypeMultiplier[1] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: gameplayManager.lifeStealAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: gameplayManager.lifeStealChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: gameplayManager.damageTypeMultiplier[3] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.magnet_range: gameplayManager.magnetRangeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.max_health: gameplayManager.maxHealthMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_attack_range: gameplayManager.meleeAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_cooldown: gameplayManager.meleeCooldownMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_chance: gameplayManager.meleeCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_damage: gameplayManager.meleeCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_damage: gameplayManager.meleeDamageMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_life_steal: gameplayManager.meleeLifeStealAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_life_steal_chance: gameplayManager.meleeLifeStealChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_size: gameplayManager.meleeSizeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_travel_range: gameplayManager.meleeTravelRangeMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_travel_speed: gameplayManager.meleeTravelSpeedMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.movement_speed: gameplayManager.moveSpeedMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: gameplayManager.damageTypeMultiplier[0] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: gameplayManager.pierceAdditive += (int)upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: gameplayManager.projectileAdditive += (int)upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_attack_range: gameplayManager.projectileAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_cooldown: gameplayManager.projectileCooldownMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_chance: gameplayManager.projectileCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_damage: gameplayManager.projectileCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_damage: gameplayManager.projectileDamageMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_life_steal: gameplayManager.projectileLifeStealAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_life_steal_chance: gameplayManager.projectileLifeStealChanceAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_size: gameplayManager.projectileSizeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_travel_range: gameplayManager.projectileTravelRangeMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_travel_speed: gameplayManager.projectileTravelSpeedMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.regen: gameplayManager.regenAdditive += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: gameplayManager.ailmentsChanceAdditive[3] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: gameplayManager.ailmentsEffectAdditive[3] += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: gameplayManager.sizeMultiplier += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.strike: gameplayManager.strikeAdditive += (int)upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: gameplayManager.travelRangeMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: gameplayManager.travelSpeedMultipiler += upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier[i]); break;
            }
        }
        gameplayManager.player.UpdatePlayerStats();
        for (int i = 0; i < gameplayManager.inventory.activeSkillList.Count; i++) //update all active skills
        {
            if (gameplayManager.inventory.activeSkillList[i].skillController != null)
            {
                gameplayManager.inventory.activeSkillList[i].skillController.UpdateSkillStats();
            }
        }
        if (currentMaxHP != gameplayManager.player.maxHealth) //if hp is upgraded, update current hp.
        {
            gameplayManager.player.currentHealth += gameplayManager.player.maxHealth - currentMaxHP;
            gameplayManager.player.UpdateHealthBar();
        }
    }
    //Upgrade skill when it levels.
    public void ApplySkillUpgrades(Upgrades upgrade, SkillController skill, int level)
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
        //for (int i = 0; i < skill.damageTypes.Count; i++) //Update damage
        //{
        //    if (skill.baseDamageTypes[i] > 0)
        //    {
        //        skill.damageTypes[i] = (skill.baseDamageTypes[i] * (1 + (gameplayManager.damageTypeMultiplier[i] + skill.damage) / 100)) * (1 - gameplayManager.resistances[i] / 100);
        //    }
        //}
        //skill.highestDamageType = skill.damageTypes.IndexOf(Mathf.Max(skill.damageTypes.ToArray()));  //Find highest damage type.
        //skill.UpdateSize();
    }
    //Apply gem modifiers to skill.
    public void ApplyGemUpgrades(Upgrades upgrade, SkillController skill)
    {
        for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[0].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.addedAttackRange += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.addedAilmentsChance[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.addedAilmentsEffect[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.addedAilmentsChance[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.addedAilmentsEffect[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.addedChain += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.addedAilmentsChance[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.addedAilmentsEffect[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.addedDamageTypes[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.addedCooldown += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.addedCriticalChance += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.addedCriticalDamage += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.addedDamage += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.addedDamageTypes[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.knockback: skill.addedKnockBack += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.addedLifeSteal += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.addedLifeStealChance += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.addedDamageTypes[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.addedDamageTypes[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.addedPierce += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.addedProjectile += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.addedAilmentsChance[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.addedAilmentsEffect[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.addedSize += upgrade.levelModifiersList[0].amt[i]; break; //doesn't have baseSize
                case Upgrades.LevelModifiers.Modifier.strike: skill.addedStrike += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.addedTravelRange += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.addedTravelSpeed += upgrade.levelModifiersList[0].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
    }
    //Unapply gem modifiers to skill.
    public void UnapplyGemUpgrades(Upgrades upgrade, SkillController skill)
    {
        for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[0].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.addedAttackRange -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.addedAilmentsChance[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.addedAilmentsEffect[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.addedAilmentsChance[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.addedAilmentsEffect[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.addedChain -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.addedAilmentsChance[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.addedAilmentsEffect[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.addedDamageTypes[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.addedCooldown -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.addedCriticalChance -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.addedCriticalDamage -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.addedDamage -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.addedDamageTypes[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.knockback: skill.addedKnockBack -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.addedLifeSteal -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.addedLifeStealChance -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.addedDamageTypes[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.addedDamageTypes[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.addedPierce -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.addedProjectile -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.addedAilmentsChance[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.addedAilmentsEffect[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.addedSize -= upgrade.levelModifiersList[0].amt[i]; break; //doesn't have baseSize
                case Upgrades.LevelModifiers.Modifier.strike: skill.addedStrike -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.addedTravelRange -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.addedTravelSpeed -= upgrade.levelModifiersList[0].amt[i]; break;
                default: GameManager.DebugLog("UnapplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
    }
    public string FormatSkillUpgradesToString(Upgrades upgrade) //Format skill level up stats to string for Tooltip.
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
    public string FormatItemUpgradeStatsToString(Upgrades.LevelModifiers stats)
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
                    if (CheckPercentModifier(stats.modifier[i]))
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
                    if (CheckPercentModifier(stats.modifier[i]))
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
                    if (CheckPercentModifier(stats.modifier[i]))
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
                    if (CheckPercentModifier(stats.modifier[i]))
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
    public string FormatSkillStatsToString(SkillController sc) //Return stats of the skill controller as string.
    {
        string fullString = "";
        for(int i = 0; i < sc.damageTypes.Count; i++)
        {
            if (sc.damageTypes[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Damage: " + sc.damageTypes[i] + "\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: " + sc.damageTypes[i] + "\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: " + sc.damageTypes[i] + "\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: " + sc.damageTypes[i] + "\n";
            }
        }
        if (sc.criticalChance > 0)
            fullString += "Critical Chance: " + sc.criticalChance + "%\n";
        if (sc.criticalDamage > gameplayManager.criticalDamageAdditive)
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
    public void FormatPlayerStats1ToString(TextMeshProUGUI text1)
    {
        text1.text = string.Empty;
        string fullString = "";
        fullString += "Health: " + gameplayManager.player.currentHealth + "/" + gameplayManager.player.maxHealth + " (+" + gameplayManager.maxHealthMultiplier + "%)\n";
        fullString += "Defense: " + gameplayManager.player.defense + " (+" + gameplayManager.defenseMultiplier + "%)\n";
        fullString += "Regen: " + gameplayManager.player.regen + "\n";
        fullString += "Degen: " + gameplayManager.player.degen + "\n";
        fullString += "Movement Speed: " + gameplayManager.player.moveSpeed + " (+" + gameplayManager.moveSpeedMultiplier + "%)\n";
        fullString += "Dash: " + gameplayManager.player.playerMovement.maxCharges + "\n";
        fullString += "Dash Cooldown: " + gameplayManager.player.playerMovement.dashCooldown + " (+" + gameplayManager.dashCooldownMultiplier + "%)\n";
        fullString += "Dash Power: " + gameplayManager.player.playerMovement.dashPower + " (+" + gameplayManager.dashPowerMultiplier + "%)\n";
        fullString += "Magnet Range: " + gameplayManager.player.magnetRange + " (+" + gameplayManager.magnetRangeMultiplier + "%)\n";
        fullString += "Exp: " + gameplayManager.exp + "/" + gameplayManager.expCap + "\n";

        text1.text = fullString;
    }
    public void FormatPlayerStats2ToString(TextMeshProUGUI text2)
    {
        text2.text = string.Empty;
        string fullString = "";
        if (gameplayManager.damageMultiplier > 0)
            fullString += "Damage: +" + gameplayManager.damageMultiplier + "%\n";
        if (gameplayManager.projectileDamageMultiplier > 0)
            fullString += "Projectile Damage: +" + gameplayManager.projectileDamageMultiplier + "%\n";
        if (gameplayManager.meleeDamageMultiplier > 0)
            fullString += "Melee Damage: +" + gameplayManager.meleeDamageMultiplier + "%\n";
        for (int i = 0; i < gameplayManager.damageTypeMultiplier.Count; i++)
        {
            if (gameplayManager.damageTypeMultiplier[i] > 0)
            {
                if (i == 0) //physical damage
                    fullString = "Physical Damage: +" + gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: +" + gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: +" + gameplayManager.damageTypeMultiplier[i] + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: +" + gameplayManager.damageTypeMultiplier[i] + "%\n";
            }
        }
        if (gameplayManager.criticalChanceAdditive > 0)
            fullString += "Critical Chance: +" + gameplayManager.criticalChanceAdditive + "%\n";
        if (gameplayManager.projectileCriticalChanceAdditive > 0)
            fullString += "Projectile Critical Chance: +" + gameplayManager.projectileCriticalChanceAdditive + "%\n";
        if (gameplayManager.meleeCriticalChanceAdditive > 0)
            fullString += "Melee Critical Chance: +" + gameplayManager.meleeCriticalChanceAdditive + "%\n";
        if (gameplayManager.criticalDamageAdditive > 0)
            fullString += "Critical Damage: +" + gameplayManager.criticalDamageAdditive + "%\n";
        if (gameplayManager.projectileCriticalDamageAdditive > 0)
            fullString += "Projectile Critical Damage: +" + gameplayManager.projectileCriticalDamageAdditive + "%\n";
        if (gameplayManager.meleeCriticalDamageAdditive > 0)
            fullString += "Melee Critical Damage: +" + gameplayManager.meleeCriticalDamageAdditive + "%\n";
        if (gameplayManager.cooldownMultiplier > 0)
            fullString += "Cooldown: +" + gameplayManager.cooldownMultiplier + "%\n";
        if (gameplayManager.projectileCooldownMultiplier > 0)
            fullString += "Projectile Cooldown: +" + gameplayManager.projectileCooldownMultiplier + "%\n";
        if (gameplayManager.meleeCooldownMultiplier > 0)
            fullString += "Melee Cooldown: +" + gameplayManager.meleeCooldownMultiplier + "%\n";
        if (gameplayManager.projectileAdditive > 0)
            fullString += "Projectile: +" + gameplayManager.projectileAdditive + "\n";
        if (gameplayManager.pierceAdditive > 0)
            fullString += "Pierce: +" + gameplayManager.pierceAdditive + "\n";
        if (gameplayManager.chainAdditive > 0)
            fullString += "Chain: +" + gameplayManager.chainAdditive + "\n";
        if (gameplayManager.strikeAdditive > 0)
            fullString += "Strike: +" + gameplayManager.strikeAdditive + "\n";
        if (gameplayManager.attackRangeMultiplier > 0)
            fullString += "Attack Range: +" + gameplayManager.attackRangeMultiplier + "%\n";
        if (gameplayManager.projectileAttackRangeMultiplier > 0)
            fullString += "Projectile Attack Range: +" + gameplayManager.projectileAttackRangeMultiplier + "%\n";
        if (gameplayManager.meleeAttackRangeMultiplier > 0)
            fullString += "Melee Attack Range: +" + gameplayManager.meleeAttackRangeMultiplier + "%\n";
        for (int i = 0; i < gameplayManager.ailmentsChanceAdditive.Count; i++)
        {
            if (i == 0) //physical damage
            {
                if (gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Bleed Chance: +" + gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Bleed Effect: +" + gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 1) //Fire damage
            {
                if (gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Burn Chance: +" + gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Burn Effect: +" + gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 2) //cold damage
            {
                if (gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Chill Chance: +" + gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Chill Effect: +" + gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
            else if (i == 3) //lightning damage
            {
                if (gameplayManager.ailmentsChanceAdditive[i] > 0)
                    fullString += "Shock Chance: +" + gameplayManager.ailmentsChanceAdditive[i] + "%\n";
                fullString += "Shock Effect: +" + gameplayManager.ailmentsEffectAdditive[i] + "%\n";
            }
        }
        if (gameplayManager.lifeStealChanceAdditive > 0)
            fullString += "Lifesteal Chance: +" + gameplayManager.lifeStealChanceAdditive + "%\n";
        if (gameplayManager.projectileLifeStealChanceAdditive > 0)
            fullString += "Projectile Lifesteal Chance: +" + gameplayManager.projectileLifeStealChanceAdditive + "%\n";
        if (gameplayManager.meleeLifeStealChanceAdditive > 0)
            fullString += "Melee Lifesteal Chance: +" + gameplayManager.meleeLifeStealChanceAdditive + "%\n";
        if (gameplayManager.lifeStealAdditive > 0)
            fullString += "Lifesteal: +" + gameplayManager.lifeStealAdditive + "\n";
        if (gameplayManager.projectileLifeStealAdditive > 0)
            fullString += "Projectile Lifesteal: +" + gameplayManager.projectileLifeStealAdditive + "\n";
        if (gameplayManager.meleeLifeStealAdditive > 0)
            fullString += "Melee Lifesteal: +" + gameplayManager.meleeLifeStealAdditive + "\n";
        if (gameplayManager.travelSpeedMultipiler > 0)
            fullString += "Travel Speed: +" + gameplayManager.travelSpeedMultipiler + "\n";
        if (gameplayManager.projectileTravelSpeedMultipiler > 0)
            fullString += "Projectile Travel Speed: +" + gameplayManager.projectileTravelSpeedMultipiler + "\n";
        if (gameplayManager.meleeTravelSpeedMultipiler > 0)
            fullString += "Melee Travel Speed: +" + gameplayManager.meleeTravelSpeedMultipiler + "\n";
        if (gameplayManager.travelRangeMultipiler > 0)
            fullString += "Travel Range: +" + gameplayManager.travelRangeMultipiler + "\n";
        if (gameplayManager.projectileTravelRangeMultipiler > 0)
            fullString += "Projectile Travel Range: +" + gameplayManager.projectileTravelRangeMultipiler + "\n";
        if (gameplayManager.meleeTravelRangeMultipiler > 0)
            fullString += "Melee Travel Range: +" + gameplayManager.meleeTravelRangeMultipiler + "\n";
        if (gameplayManager.sizeMultiplier > 0)
            fullString += "Size: +" + gameplayManager.sizeMultiplier + "%\n";
        if (gameplayManager.projectileSizeMultiplier > 0)
            fullString += "Projectile Size: +" + gameplayManager.projectileSizeMultiplier + "%\n";
        if (gameplayManager.meleeSizeMultiplier > 0)
            fullString += "Melee Size: +" + gameplayManager.meleeSizeMultiplier + "%\n";

        text2.text = fullString;
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
