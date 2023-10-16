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
                case Upgrades.LevelModifiers.Modifier.damageMultiplier: gameplayManager.damageMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.defenseMultiplier: gameplayManager.defenseMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.cooldownMultiplier: gameplayManager.cooldownMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.maxHealthMultiplier: gameplayManager.maxHealthMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                case Upgrades.LevelModifiers.Modifier.moveSpeedMultiplier: gameplayManager.moveSpeedMultiplier += upgrade.levelModifiersList[upgrade.currentLevel].amt[i]; break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[upgrade.currentLevel].modifier[i]); break;
            }
        }
        gameplayManager.player.UpdatePlayerStats();
        if (currentMaxHP != gameplayManager.player.maxHealth)
        {
            gameplayManager.player.currentHealth += gameplayManager.player.maxHealth - currentMaxHP;
            gameplayManager.player.UpdateHealthBar();
        }
        GameManager.DebugLog("Applied Upgrades");
    }
}
