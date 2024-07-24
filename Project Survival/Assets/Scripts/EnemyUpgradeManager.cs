using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUpgradeManager : MonoBehaviour
{
    public class EnemyUpgrades
    {
        public List<UpdateStats.EnemyModifier> modifier = new();
        public List<float> amt = new();
        public void ClearAll()
        {
            modifier.Clear();
            amt.Clear();
        }
    }
    public GameObject enemyUpgradeUI;
    public GameplayManager gameplayManager;
    public Options options;
    int randModifiers, valueBoost;
    public List<UpgradeUI> upgradeUIList;
    public EnemyUpgrades tempUpgrades = new();
    List<int> modifierList = new();
    public void GetRandomEnemyGrowthStats()
    {
        for (int u = 0; u < upgradeUIList.Count; u++)
        {
            modifierList.Clear();
            upgradeUIList[u].enemyUpgrade.modifier.Clear();
            upgradeUIList[u].enemyUpgrade.amt.Clear();
            tempUpgrades.ClearAll();
            randModifiers = Random.Range(1, 4); //Number of modifiers
            int num;
            for (int i = 0; i < 1000; i++) //Get modifiers
            {
                num = Random.Range(0, System.Enum.GetValues(typeof(UpdateStats.EnemyModifier)).Length);
                if ((UpdateStats.EnemyModifier)num == UpdateStats.EnemyModifier.Projectile) //if projectile is chosen, roll once more.
                    num = Random.Range(0, System.Enum.GetValues(typeof(UpdateStats.EnemyModifier)).Length);
                if (!modifierList.Contains(num)) //add mod to list
                {
                    modifierList.Add(num);
                    if (modifierList.Count == randModifiers) break;
                }
            }
            if (randModifiers <= 1) //Boost stats that have less modifiers.
                valueBoost = 10;
            else if (randModifiers <= 2)
                valueBoost = 5;
            else valueBoost = 0;
            for (int j = 0; j < modifierList.Count; j++) //Set tempUpgrades with modifier and values.
            {
                UpdateStats.EnemyModifier mod = (UpdateStats.EnemyModifier)modifierList[j];
                tempUpgrades.modifier.Add(mod);
                tempUpgrades.amt.Add(GetModifierValue(mod));
            }
            upgradeUIList[u].enemyUpgrade.modifier.AddRange(tempUpgrades.modifier);
            upgradeUIList[u].enemyUpgrade.amt.AddRange(tempUpgrades.amt);
            upgradeUIList[u].SetUI();
        }
    }

    public float GetModifierValue(UpdateStats.EnemyModifier mod)
    {
        float value = 0;
        switch (mod)
        {
            case UpdateStats.EnemyModifier.Attack_Cooldown: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Cold_Reduction: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Damage: value = 20; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Fire_Reduction: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Lightning_Reduction: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Max_Health: value = 20; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Move_Speed: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Physical_Reduction: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Projectile: value = 1; break;
            //case UpdateStats.EnemyModifier.projectile_size: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.Projectile_Travel_Speed: value = 5; value += valueBoost; break;
            default: Debug.Log("GetModifierValue has no switch case for " + mod); break;
        }
        return value;
    }
    public void OpenUI()
    {
        if (enemyUpgradeUI.activeSelf == false)
        {
            GameManager.PauseGame();
            GetRandomEnemyGrowthStats();
            enemyUpgradeUI.SetActive(true);
        }
    }
    public void ConfirmButton()
    {
        enemyUpgradeUI.SetActive(false);
        options.OpenCloseUI();
        options.ShopUIButton();
    }
}
