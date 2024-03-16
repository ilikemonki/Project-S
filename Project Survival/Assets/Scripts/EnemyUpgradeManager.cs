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
    int randModifiers, valueBoost;
    public List<UpgradeUI> upgradeUIList;
    public EnemyUpgrades tempUpgrades = new();
    List<int> modifierList = new();
    //Hand of Fate
    public Slider handSlider;
    public float rand;
    bool moveHand, stopHand;
    public float handTimer;
    private void Update()
    {
        if (enemyUpgradeUI.activeSelf)
        {
            if (moveHand && !stopHand)
            {
                if (rand <= .5)
                {
                    handSlider.value -= Time.unscaledDeltaTime * 0.5f;
                }
                else
                {
                    handSlider.value += Time.unscaledDeltaTime * 0.5f;
                }
                if ((rand <= .5 && handSlider.value <= rand) || (rand >= .5 && handSlider.value >= rand))
                {
                    moveHand = false;
                    stopHand = true;
                }
            }
            else if (!moveHand && !stopHand)
            {
                handTimer += Time.unscaledDeltaTime;
                if (handTimer >= 2f)
                {
                    moveHand = true;
                }
            }
        }
    }
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
                if ((UpdateStats.EnemyModifier)num == UpdateStats.EnemyModifier.projectile) //if projectile is chosen, roll once more.
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
        HandOfFate();
    }
    public void HandOfFate()
    {
        rand = Random.Range(0f, 1f);
    }

    public float GetModifierValue(UpdateStats.EnemyModifier mod)
    {
        float value = 0;
        switch (mod)
        {
            case UpdateStats.EnemyModifier.attack_cooldown: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.cold_resistance: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.damage: value = 20; value += valueBoost; break;
            case UpdateStats.EnemyModifier.fire_resistance: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.lightning_resistance: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.max_health: value = 20; value += valueBoost; break;
            case UpdateStats.EnemyModifier.move_speed: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.physical_resistance: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.projectile: value = 1; break;
            case UpdateStats.EnemyModifier.projectile_size: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.projectile_travel_range: value = 5; value += valueBoost; break;
            case UpdateStats.EnemyModifier.projectile_travel_speed: value = 5; value += valueBoost; break;
            default: GameManager.DebugLog("GetModifierValue has no switch case for " + mod); break;
        }
        return value;
    }
    public void ButtonClick()
    {
        if (stopHand == true) CloseUI();
        moveHand = false;
        stopHand = true;
        handSlider.value = rand;
    }
    public void OpenUI()
    {
        if (enemyUpgradeUI.activeSelf == false)
        {
            GameManager.PauseGame();
            GetRandomEnemyGrowthStats();
            enemyUpgradeUI.SetActive(true);
            handSlider.value = 0.5f;
            handTimer = 0;
            moveHand = false;
            stopHand = false;
        }
    }
    public void CloseUI()
    {
        if (enemyUpgradeUI.activeSelf == true)
        {
            GameManager.UnpauseGame();
            enemyUpgradeUI.SetActive(false);
            handSlider.value = 0.5f;
            handTimer = 0;
            moveHand = false;
            stopHand = false;
        }
    }
}
