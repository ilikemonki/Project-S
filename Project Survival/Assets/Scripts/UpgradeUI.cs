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
            upgradeText.text = updateStats.FormatStatsToString(upgrade.levelModifiersList[upgrade.itemDescription.currentLevel]); //Get stats string
        }
    }
}
