using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour
{
    public LevelUpManager levelupManager;
    public EnemyUpgradeManager enemyUpgradeManager;
    public Upgrades upgrade;
    public EnemyUpgradeManager.EnemyUpgrades enemyUpgrade = new();
    public TextMeshProUGUI upgradeText;
    public void ButtonClick()
    {
        if (upgrade != null)
        {
            UpdateStats.ApplyGlobalUpgrades(upgrade, false);
            levelupManager.numberOfLevelUps--;
            if (levelupManager.numberOfLevelUps > 0)
            {
                levelupManager.OpenUI();
            }
            else
            {
                levelupManager.CloseUI();
            }
        }
        else if (enemyUpgrade != null)
        {
            UpdateStats.ApplyEnemyUpgrades(enemyUpgrade);
            enemyUpgradeManager.CloseUI();
        }
    }
    public void SetUI()
    {
        if (upgrade != null)
        {
            upgradeText.text = UpdateStats.FormatItemUpgradeStatsToString(upgrade.levelModifiersList[0]); //Get stats string
        }
        else if (enemyUpgrade != null)
        {
            upgradeText.text = UpdateStats.FormatEnemyUpgradeToString(enemyUpgrade); //Get stats string
        }
    }
}
