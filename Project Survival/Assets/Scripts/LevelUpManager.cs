using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public bool stopLevelUp;
    public int rerollPrice, rerollIncrement, freeReroll;
    public TextMeshProUGUI rerollText;
    public GameObject levelUpUI;
    public GameplayManager gameplayManager;
    int randModifiers;
    float returnValue;
    public List<UpgradeUI> upgradeUIList;
    public Upgrades tempUpgrades;
    List<int> modifierList = new();
    public void PopulateLevelUpGrowthStats()
    {
        for (int u = 0; u < upgradeUIList.Count; u++)
        {
            upgradeUIList[u].upgrade.levelModifiersList.Clear();
            modifierList.Clear();
            tempUpgrades.levelModifiersList.Clear();
            randModifiers = Random.Range(2, 5); //Number of modifiers
            int num;
            for (int i = 0; i < 1000; i++) //Get modifiers
            {
                num = Random.Range(0, System.Enum.GetValues(typeof(UpdateStats.Modifier)).Length);
                if (!modifierList.Contains(num) && CheckModifier((UpdateStats.Modifier)num)) //add mod to list
                {
                    modifierList.Add(num);
                    if (modifierList.Count == randModifiers) break;
                }
            }
            Upgrades.LevelModifiers lvMod = new();
            tempUpgrades.levelModifiersList.Add(lvMod);
            for (int j = 0; j < modifierList.Count; j++) //Set tempUpgrades with modifier and values.
            {
                UpdateStats.Modifier mod = (UpdateStats.Modifier)modifierList[j];
                tempUpgrades.levelModifiersList[0].modifier.Add(mod);
                tempUpgrades.levelModifiersList[0].amt.Add(GetModifierValue(mod));
            }
            upgradeUIList[u].upgrade.levelModifiersList.AddRange(tempUpgrades.levelModifiersList);
            upgradeUIList[u].SetUI();
        }
    }
    public void Reroll()
    {
        if (freeReroll > 0)
        {
            freeReroll--;
            if (freeReroll > 0)
                rerollText.text = freeReroll + " Free Reroll";
            else
                rerollText.text = rerollPrice.ToString() + "\n Reroll";
            PopulateLevelUpGrowthStats();
        }
        else if (rerollPrice <= gameplayManager.coins)
        {
            gameplayManager.GainCoins(-rerollPrice);
            rerollPrice += rerollIncrement;
            rerollText.text = rerollPrice + "\n Reroll";
            PopulateLevelUpGrowthStats();
        }
    }

    public void OpenUI()
    {
        freeReroll = gameplayManager.freeLevelupRerollAdditive;
        if (freeReroll > 0)
            rerollText.text = freeReroll + " Free Reroll";
        else
            rerollText.text = rerollPrice + "\n Reroll"; 
        if (levelUpUI.activeSelf == false)
        {
            GameManager.PauseGame();
            PopulateLevelUpGrowthStats();
            levelUpUI.SetActive(true);
        }
    }
    public void CloseUI()
    {
        if (levelUpUI.activeSelf == true)
        {
            GameManager.UnpauseGame();
            levelUpUI.SetActive(false);
        }
    }
    public float GetModifierValue(UpdateStats.Modifier mod)
    {
        returnValue = 0;
        switch (mod)
        {
            case UpdateStats.Modifier.attack_range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.base_cold_damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.base_fire_damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.base_lightning_damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.base_physical_damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.bleed_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.bleed_effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.burn_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.burn_effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            //case UpdateStats.Modifier.chain: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.chill_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.chill_effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.cold_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.cooldown: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.critical_chance: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.critical_damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.damage: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.defense: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.degen: returnValue = 1; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.fire_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.life_steal: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.life_steal_chance: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.lightning_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.magnet_range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.max_health: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.melee_attack_range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_cooldown: returnValue = 4; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_critical_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_critical_damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.melee_life_steal: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.melee_life_steal_chance: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.melee_size: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_travel_range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.melee_travel_speed: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.movement_speed: returnValue = 5; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.physical_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            //case UpdateStats.Modifier.pierce: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            //case UpdateStats.Modifier.projectile: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.projectile_attack_range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_cooldown: returnValue = 4; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_critical_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_critical_damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.projectile_life_steal: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.projectile_life_steal_chance: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.projectile_size: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_travel_range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.projectile_travel_speed: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.regen: returnValue = 1; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.shock_chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.shock_effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.size: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            //case UpdateStats.Modifier.strike: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.travel_range: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.travel_speed: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            default: Debug.Log("GetModifierValue has no switch case for " + mod); break;
        }
        return returnValue;
    }
    public bool CheckModifier(UpdateStats.Modifier mod) //Check modifier if they can be chosen
    {
        if (mod == UpdateStats.Modifier.pierce || mod == UpdateStats.Modifier.chain || mod == UpdateStats.Modifier.projectile_amount 
            || mod == UpdateStats.Modifier.melee_amount || mod == UpdateStats.Modifier.knockback || mod == UpdateStats.Modifier.base_cold_damage
            || mod == UpdateStats.Modifier.base_fire_damage || mod == UpdateStats.Modifier.base_lightning_damage || mod == UpdateStats.Modifier.base_physical_damage)
        {
            return false;
        }
        else return true;
    }

}
