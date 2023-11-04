using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStats : MonoBehaviour
{
    public GameplayManager gameplayManager;
    float currentMaxHP;
    public void ApplyUpgradeStats(Upgrades upgrade)
    {
        currentMaxHP = gameplayManager.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[upgrade.currentLevel].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[upgrade.currentLevel].modifier[i])
            {
                case Upgrades.LevelModifiers.Modifier.attackRangeMultiplier: gameplayManager.attackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleedChanceAdditive: gameplayManager.ailmentsChanceAdditive[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.bleedEffectAdditive: gameplayManager.ailmentsEffectAdditive[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burnChanceAdditive: gameplayManager.ailmentsChanceAdditive[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.burnEffectAdditive: gameplayManager.ailmentsEffectAdditive[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chainAdditive: gameplayManager.chainAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chillChanceAdditive: gameplayManager.ailmentsChanceAdditive[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.chillEffectAdditive: gameplayManager.ailmentsEffectAdditive[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldownMultiplier: gameplayManager.cooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.criticalChanceAdditive: gameplayManager.criticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.criticalDamageAdditive: gameplayManager.criticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.damageMultiplier: gameplayManager.damageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.defenseMultiplier: gameplayManager.defenseMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.degenAdditive: gameplayManager.degenAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.fireMultiplier: gameplayManager.damageTypeMultiplier[1] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.coldMultiplier: gameplayManager.damageTypeMultiplier[2] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lifeStealAdditive: gameplayManager.lifeStealAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lifeStealChanceAdditive: gameplayManager.lifeStealChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.lightningMultiplier: gameplayManager.damageTypeMultiplier[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.magnetRangeMultiplier: gameplayManager.magnetRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.maxHealthMultiplier: gameplayManager.maxHealthMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeAttackRangeMultiplier: gameplayManager.meleeAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeCooldownMultiplier: gameplayManager.meleeCooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeCriticalChanceAdditive: gameplayManager.meleeCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeCriticalDamageAdditive: gameplayManager.meleeCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeDamageMultiplier: gameplayManager.meleeDamageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.meleeSizeAdditive: gameplayManager.meleeSizeAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.moveSpeedMultiplier: gameplayManager.moveSpeedMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.physicalMultiplier: gameplayManager.damageTypeMultiplier[0] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.pierceAdditive: gameplayManager.pierceAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileAdditive: gameplayManager.projectileAdditive += (int)upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileAttackRangeMultiplier: gameplayManager.projectileAttackRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileCooldownMultiplier: gameplayManager.projectileCooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileCriticalChanceAdditive: gameplayManager.projectileCriticalChanceAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileCriticalDamageAdditive: gameplayManager.projectileCriticalDamageAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileDamageMultiplier: gameplayManager.projectileDamageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.projectileSizeAdditive: gameplayManager.projectileSizeAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.regenAdditive: gameplayManager.regenAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shockChanceAdditive: gameplayManager.ailmentsChanceAdditive[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.shockEffectAdditive: gameplayManager.ailmentsEffectAdditive[3] += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.sizeAdditive: gameplayManager.sizeAdditive += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.strikeAdditive: gameplayManager.tag += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travelRangeMultiplier: gameplayManager.travelRangeMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.travelSpeedMultiplier: gameplayManager.travelSpeedMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[upgrade.currentLevel].modifier[i]); break;
            }
        }
        gameplayManager.player.UpdatePlayerStats();
        for (int i = 0; i < gameplayManager.inventory.skillSlotList.Count; i++)
        {
            if (gameplayManager.inventory.skillSlotList[i].skillController != null)
            {
                gameplayManager.inventory.skillSlotList[i].skillController.UpdateSkillStats();
            }
        }
        if (currentMaxHP != gameplayManager.player.maxHealth) //if hp is upgraded, update current hp.
        {
            gameplayManager.player.currentHealth += gameplayManager.player.maxHealth - currentMaxHP;
            gameplayManager.player.UpdateHealthBar();
        }
    }
}
