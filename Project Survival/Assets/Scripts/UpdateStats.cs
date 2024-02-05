using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStats : MonoBehaviour
{
    public GameplayManager gameplayManager;
    float currentMaxHP;
    public void ApplyGlobalUpgrades(Upgrades upgrade)
    {
        currentMaxHP = gameplayManager.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[upgrade.currentLevel].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[upgrade.currentLevel].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: gameplayManager.attackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: gameplayManager.ailmentsChanceAdditive[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: gameplayManager.ailmentsEffectAdditive[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: gameplayManager.ailmentsChanceAdditive[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: gameplayManager.ailmentsEffectAdditive[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: gameplayManager.chainAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: gameplayManager.ailmentsChanceAdditive[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: gameplayManager.ailmentsEffectAdditive[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: gameplayManager.damageTypeMultiplier[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: gameplayManager.cooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: gameplayManager.criticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: gameplayManager.criticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: gameplayManager.damageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.defense: gameplayManager.defenseMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.degen: gameplayManager.degenAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: gameplayManager.damageTypeMultiplier[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: gameplayManager.lifeStealAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: gameplayManager.lifeStealChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: gameplayManager.damageTypeMultiplier[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.magnet_range: gameplayManager.magnetRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.max_health: gameplayManager.maxHealthMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_attack_range: gameplayManager.meleeAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_cooldown: gameplayManager.meleeCooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_chance: gameplayManager.meleeCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_critical_damage: gameplayManager.meleeCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_damage: gameplayManager.meleeDamageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.melee_size: gameplayManager.meleeSizeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.movement_speed: gameplayManager.moveSpeedMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: gameplayManager.damageTypeMultiplier[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: gameplayManager.pierceAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: gameplayManager.projectileAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_attack_range: gameplayManager.projectileAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_cooldown: gameplayManager.projectileCooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_chance: gameplayManager.projectileCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_critical_damage: gameplayManager.projectileCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_damage: gameplayManager.projectileDamageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile_size: gameplayManager.projectileSizeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.regen: gameplayManager.regenAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: gameplayManager.ailmentsChanceAdditive[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: gameplayManager.ailmentsEffectAdditive[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: gameplayManager.sizeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.strike: gameplayManager.strikeAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: gameplayManager.projectileDistanceMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: gameplayManager.projectileSpeedMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[upgrade.currentLevel].modifier[i]); break;
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
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.attackRange += upgrade.levelModifiersList[level].amt[i] / 100; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.ailmentsChance[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.ailmentsEffect[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.ailmentsChance[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.ailmentsEffect[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.chain += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.ailmentsChance[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.ailmentsEffect[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.damageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.cooldown += upgrade.levelModifiersList[level].amt[i] / 100; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.criticalChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.criticalDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.damage += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.damageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.lifeSteal += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.lifeStealChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.damageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.damageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.pierce += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.projectile += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.ailmentsChance[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.ailmentsEffect[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.size += upgrade.levelModifiersList[level].amt[i] / 100; break;
                case Upgrades.LevelModifiers.Modifier.strike: skill.strike += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.travelRange += upgrade.levelModifiersList[level].amt[i] / 100; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.travelSpeed += upgrade.levelModifiersList[level].amt[i] / 100; break;
                default: GameManager.DebugLog("ApplySkillMod has no switch case for " + upgrade.levelModifiersList[level].modifier[i]); break;
            }
        }
        for (int i = 0; i < skill.damageTypes.Count; i++) //Update damage
        {
            if (skill.baseDamageTypes[i] > 0)
            {
                skill.damageTypes[i] = (skill.baseDamageTypes[i] * (1 + (gameplayManager.damageTypeMultiplier[i] + skill.damage) / 100)) * (1 - gameplayManager.resistances[i] / 100);
            }
        }
        skill.highestDamageType = skill.damageTypes.IndexOf(Mathf.Max(skill.damageTypes.ToArray()));  //Find highest damage type.
        skill.UpdateSize();
    }
    //Apply gem modifiers to skill.
    public void ApplyGemUpgrades(Upgrades upgrade, SkillController skill)
    {
        for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[0].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.attackRange += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.ailmentsChance[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.ailmentsEffect[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.ailmentsChance[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.ailmentsEffect[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.chain += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.ailmentsChance[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.ailmentsEffect[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.damageTypes[2] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.cooldown += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.criticalChance += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.criticalDamage += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.damage += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.damageTypes[1] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.lifeSteal += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.lifeStealChance += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.damageTypes[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.damageTypes[0] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.pierce += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.projectile += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.ailmentsChance[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.ailmentsEffect[3] += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.size += upgrade.levelModifiersList[0].amt[i]; break; //doesn't have baseSize
                case Upgrades.LevelModifiers.Modifier.strike: skill.strike += (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.travelRange += upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.travelSpeed += upgrade.levelModifiersList[0].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
    }
    //Unapply gem modifiers to skill.
    public void UnapplyGemUpgrades(Upgrades upgrade, SkillController skill)
    {
        for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[0].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attack_range: skill.attackRange -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_chance: skill.ailmentsChance[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleed_effect: skill.ailmentsEffect[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_chance: skill.ailmentsChance[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burn_effect: skill.ailmentsEffect[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chain: skill.chain -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_chance: skill.ailmentsChance[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chill_effect: skill.ailmentsEffect[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cold_damage: skill.damageTypes[2] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldown: skill.cooldown -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_chance: skill.criticalChance -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.critical_damage: skill.criticalDamage -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damage: skill.damage -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fire_damage: skill.damageTypes[1] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal: skill.lifeSteal -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.life_steal_chance: skill.lifeStealChance -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightning_damage: skill.damageTypes[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physical_damage: skill.damageTypes[0] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierce: skill.pierce -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectile: skill.projectile -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_chance: skill.ailmentsChance[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shock_effect: skill.ailmentsEffect[3] -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.size: skill.size -= upgrade.levelModifiersList[0].amt[i]; break; //doesn't have baseSize
                case Upgrades.LevelModifiers.Modifier.strike: skill.strike -= (int)upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_range: skill.travelRange -= upgrade.levelModifiersList[0].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travel_speed: skill.travelSpeed -= upgrade.levelModifiersList[0].amt[i]; break;
                default: GameManager.DebugLog("UnapplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
            }
        }
    }
}
