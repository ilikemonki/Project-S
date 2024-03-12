using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public bool stopLevelUp;
    public int rerollPrice, rerollIncrement;
    public TextMeshProUGUI rerollText;
    public GameObject levelUpUI;
    public List<Upgrades> levelUpGrowthList; 
    public GameplayManager gameplayManager;
    int randModifiers, valueBoost;
    public List<UpgradeUI> upgradeUIList;
    public Upgrades tempUpgrades;
    List<int> modifierList = new();
    public void GetRandomLevelUpGrowthStats()
    {
        for (int u = 0; u < upgradeUIList.Count; u++)
        {
            upgradeUIList[u].upgrade.levelModifiersList.Clear();
            modifierList.Clear();
            tempUpgrades.levelModifiersList.Clear();
            randModifiers = Random.Range(2, 7); //Number of modifiers
            int num;
            for (int i = 0; i < 1000; i++) //Get modifiers
            {
                num = Random.Range(0, System.Enum.GetValues(typeof(Upgrades.LevelModifiers.Modifier)).Length);
                if (!modifierList.Contains(num) && num > 3 && num != 26) //add mod to list
                {
                    modifierList.Add(num);
                    if (modifierList.Count == randModifiers) break;
                }
            }
            if (randModifiers <= 2) //Boost stats that have less modifiers.
                valueBoost = 5;
            else if (randModifiers <= 3)
                valueBoost = 3;
            else if (randModifiers <= 4)
                valueBoost = 2;
            else valueBoost = 0;
            Upgrades.LevelModifiers lvMod = new();
            tempUpgrades.levelModifiersList.Add(lvMod);
            for (int j = 0; j < modifierList.Count; j++) //Set tempUpgrades with modifier and values.
            {
                Upgrades.LevelModifiers.Modifier mod = (Upgrades.LevelModifiers.Modifier)modifierList[j];
                tempUpgrades.levelModifiersList[0].modifier.Add(mod);
                tempUpgrades.levelModifiersList[0].amt.Add(GetModifierValue(mod));
            }
            upgradeUIList[u].upgrade.levelModifiersList.AddRange(tempUpgrades.levelModifiersList);
            upgradeUIList[u].SetUI();
        }
    }
    public void Reroll()
    {
        if (rerollPrice <= gameplayManager.coins)
        {
            gameplayManager.GainCoins(-rerollPrice);
            rerollPrice += rerollIncrement;
            rerollText.text = rerollPrice + "\n Reroll";
            GetRandomLevelUpGrowthStats();
        }
    }

    public void OpenUI()
    {
        rerollText.text = rerollPrice + "\n Reroll"; 
        if (levelUpUI.activeSelf == false)
        {
            GameManager.PauseGame();
            GetRandomLevelUpGrowthStats();
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
    public float GetModifierValue(Upgrades.LevelModifiers.Modifier mod)
    {
        float value = 0;
        switch (mod)
        {
            case Upgrades.LevelModifiers.Modifier.attack_range: value = Random.Range(1, 4); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.bleed_chance: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.bleed_effect: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.burn_chance: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.burn_effect: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            //case Upgrades.LevelModifiers.Modifier.chain: value = Random.Range(1, 5); break;
            case Upgrades.LevelModifiers.Modifier.chill_chance: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.chill_effect: value = Random.Range(1, 2); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.cold_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.cooldown: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.critical_chance: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.critical_damage: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.defense: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.degen: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.fire_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.life_steal: value = Random.Range(1, 2); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.life_steal_chance: value = Random.Range(1, 2); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.lightning_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.magnet_range: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.max_health: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_attack_range: value = Random.Range(1, 5); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_cooldown: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.melee_critical_chance: value = Random.Range(1, 5); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_critical_damage: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_life_steal: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.melee_life_steal_chance: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.melee_size: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_travel_range: value = Random.Range(1, 5); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.melee_travel_speed: value = Random.Range(1, 5); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.movement_speed: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.physical_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            //case Upgrades.LevelModifiers.Modifier.pierce: value = Random.Range(1, 5); break;
            //case Upgrades.LevelModifiers.Modifier.projectile: value = Random.Range(1, 5); break;
            case Upgrades.LevelModifiers.Modifier.projectile_attack_range: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_cooldown: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_critical_chance: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_critical_damage: value = Random.Range(1, 5); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.projectile_damage: value = Random.Range(1, 11); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.projectile_life_steal: value = Random.Range(1, 2); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_life_steal_chance: value = Random.Range(1, 2); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_size: value = Random.Range(1, 4); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.projectile_travel_range: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.projectile_travel_speed: value = Random.Range(1, 6); if (valueBoost > 0) value += Random.Range(1, valueBoost); break;
            case Upgrades.LevelModifiers.Modifier.regen: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.shock_chance: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.shock_effect: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.size: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            //case Upgrades.LevelModifiers.Modifier.strike: value = Random.Range(1, 5); break;
            case Upgrades.LevelModifiers.Modifier.travel_range: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            case Upgrades.LevelModifiers.Modifier.travel_speed: value = Random.Range(1, 3); if (valueBoost > 2) value += Random.Range(0, valueBoost - 1); break;
            default: GameManager.DebugLog("ReturnModifierValue has no switch case for " + mod); break;
        }
        return value;
    }

}
