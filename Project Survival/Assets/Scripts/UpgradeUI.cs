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

    public void OnPointerClick(PointerEventData eventData)
    {
        updateStats.ApplyUpgradeStats(upgrade);
        upgrade.currentLevel++;
        if (upgrade.currentLevel == upgrade.levelModifiersList.Count)
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
            image.sprite = upgrade.sprite;
            nameText.text = upgrade.name;
            levelText.text = upgrade.currentLevel.ToString() + "/" + upgrade.levelModifiersList.Count;
            descriptionText.text = upgrade.description;
            upgradeText.text = "";
            for (int i = 0; i < upgrade.levelModifiersList[upgrade.currentLevel].modifier.Count; i++)
            {
                if (upgrade.levelModifiersList[upgrade.currentLevel].amt[i] > 0)
                {
                    upgradeText.text = upgradeText.text + "\n" +
                        "<color=green>+" + upgrade.levelModifiersList[upgrade.currentLevel].amt[i] + "%</color> " + upgrade.levelModifiersList[upgrade.currentLevel].modifier[i];
                }
                else
                {
                    upgradeText.text = upgradeText.text + "\n" +
                        "<color=red>" + upgrade.levelModifiersList[upgrade.currentLevel].amt[i] + "%</color> " + upgrade.levelModifiersList[upgrade.currentLevel].modifier[i];
                }
            }
        }
    }
}
