using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public bool stopLevelUp;
    public int rerollPrice, rerollIncrement, freeReroll;
    public TextMeshProUGUI rerollText, numOfLevelUpsText;
    public GameObject levelUpUI;
    public GameplayManager gameplayManager;
    int randModifiers;
    float returnValue;
    public List<UpgradeUI> upgradeUIList;
    public Upgrades tempUpgrades;
    List<int> modifierList = new();
    public float numberOfLevelUps;
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
        numOfLevelUpsText.text = numberOfLevelUps.ToString();
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
        else
        {
            PopulateLevelUpGrowthStats();
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
            case UpdateStats.Modifier.Attack_Range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Base_Cold_Damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Base_Fire_Damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Base_Lightning_Damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Base_Physical_Damage: returnValue = 2; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Bleed_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Bleed_Effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Burn_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Burn_Effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            //case UpdateStats.Modifier.chain: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.Chill_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Chill_Effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Cold_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            //case UpdateStats.Modifier.combo: returnValue = ; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Cooldown: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Critical_Chance: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Critical_Damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Damage: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.Defense: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.Degen: returnValue = 1; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Fire_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.Life_Steal: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.Life_Steal_Chance: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.Lightning_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.Magnet_Range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Max_Health: returnValue = 10; if (randModifiers <= 2) returnValue += 5; else if (randModifiers <= 3) returnValue += 2; break;
            case UpdateStats.Modifier.Melee_Attack_Range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Cooldown: returnValue = 4; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Critical_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Critical_Damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.Melee_Life_Steal: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Melee_Life_Steal_Chance: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Melee_AoE: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Travel_Range: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Melee_Travel_Speed: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Movement_Speed: returnValue = 5; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Physical_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            //case UpdateStats.Modifier.pierce: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            //case UpdateStats.Modifier.projectile: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.Projectile_Attack_Range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Cooldown: returnValue = 4; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Critical_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Critical_Damage: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Damage: returnValue = 20; if (randModifiers <= 2) returnValue += 10; else if (randModifiers <= 3) returnValue += 5; break;
            case UpdateStats.Modifier.Projectile_Life_Steal: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.Projectile_Life_Steal_Chance: returnValue = 1; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break; //rare
            case UpdateStats.Modifier.Projectile_AoE: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Travel_Range: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Projectile_Travel_Speed: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Regen: returnValue = 1; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Shock_Chance: returnValue = 3; if (randModifiers <= 2) returnValue += 2; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.Shock_Effect: returnValue = 5; if (randModifiers <= 2) returnValue += 3; else if (randModifiers <= 3) returnValue += 1; break;
            case UpdateStats.Modifier.AoE: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            //case UpdateStats.Modifier.strike: returnValue = ; if (randModifiers <= 2) returnValue += ; else if (randModifiers <= 3) returnValue += ; break;
            case UpdateStats.Modifier.Travel_Range: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            case UpdateStats.Modifier.Travel_Speed: returnValue = 2; if (randModifiers <= 2) returnValue += 1; else if (randModifiers <= 3) returnValue += 0; break;
            default: Debug.Log("GetModifierValue has no switch case for " + mod); break;
        }
        return returnValue;
    }
    public bool CheckModifier(UpdateStats.Modifier mod) //Check modifier if they can be chosen
    {
        //Modifiers that can't be chosen.
        if (mod == UpdateStats.Modifier.Pierce || mod == UpdateStats.Modifier.Chain || mod == UpdateStats.Modifier.Projectile_Amount || mod == UpdateStats.Modifier.Combo
            || mod == UpdateStats.Modifier.Melee_Amount || mod == UpdateStats.Modifier.knockback || mod == UpdateStats.Modifier.Base_Cold_Damage
            || mod == UpdateStats.Modifier.Base_Fire_Damage || mod == UpdateStats.Modifier.Base_Lightning_Damage || mod == UpdateStats.Modifier.Base_Physical_Damage
            || mod == UpdateStats.Modifier.Return_|| mod == UpdateStats.Modifier.Backward_Split|| mod == UpdateStats.Modifier.Pierce_All)
        {
            return false;
        }
        else return true;
    }

}
