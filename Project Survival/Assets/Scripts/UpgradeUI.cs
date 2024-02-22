using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour, IPointerClickHandler
{
    public LevelUpManager levelupManager;
    public UpdateStats updateStats;
    public Upgrades upgrade;
    public Image image;
    public TextMeshProUGUI nameText, levelText, descriptionText, upgradeText;
    string modString;

    public void OnPointerClick(PointerEventData eventData)
    {
        updateStats.ApplyGlobalUpgrades(upgrade);
        upgrade.itemDescription.currentLevel++;
        if (upgrade.itemDescription.currentLevel == upgrade.levelModifiersList.Count)
        {
            levelupManager.maxedUpgradesList.Add(upgrade);
            levelupManager.upgradesList.Remove(upgrade);
        }
        levelupManager.CloseUI();
    }
    public void SetUI()
    {
        if (upgrade != null)
        {
            image.sprite = upgrade.itemDescription.itemSprite;
            nameText.text = upgrade.name;
            levelText.text = upgrade.itemDescription.currentLevel.ToString() + "/" + upgrade.levelModifiersList.Count;
            descriptionText.text = upgrade.itemDescription.description;
            upgradeText.text = "";
            for (int i = 0; i < upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier.Count; i++)
            {
                //create one with no percentage.
                modString = upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier[i].ToString();
                if (modString.Contains("_"))
                {
                    modString = modString.Replace('_', ' ');
                }
                if (upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i] > 0) //if positive, green w/ + sign.
                {
                    if (IsNotPercentModifier(upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier[i]))
                    {
                        upgradeText.text = upgradeText.text + "\n" +
                            "<color=green>+" + upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i] + "</color> " + modString + ".";
                    }
                    else
                    {
                        upgradeText.text = upgradeText.text + "\n" +
                            "<color=green>+" + upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i] + "%</color> " + modString + ".";
                    }
                }
                else
                {
                    if (IsNotPercentModifier(upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].modifier[i]))
                    {
                        upgradeText.text = upgradeText.text + "\n" +
                            "<color=red>" + upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i] + "</color> " + modString + ".";
                    }
                    else
                    {
                        upgradeText.text = upgradeText.text + "\n" +
                            "<color=red>" + upgrade.levelModifiersList[upgrade.itemDescription.currentLevel].amt[i] + "%</color> " + modString + ".";
                    }
                }
            }
        }
    }
    public bool IsNotPercentModifier(Upgrades.LevelModifiers.Modifier mod)
    {
        if (mod == Upgrades.LevelModifiers.Modifier.strike || mod == Upgrades.LevelModifiers.Modifier.projectile || mod == Upgrades.LevelModifiers.Modifier.pierce || mod == Upgrades.LevelModifiers.Modifier.chain ||
            mod == Upgrades.LevelModifiers.Modifier.regen || mod == Upgrades.LevelModifiers.Modifier.degen || mod == Upgrades.LevelModifiers.Modifier.life_steal)
        {
            return true;
        }
        else return false;
    }
}
