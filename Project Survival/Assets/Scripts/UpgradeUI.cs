using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour, IPointerClickHandler
{
    public LevelUpManager levelupManager;
    public Upgrades upgrade;
    public TextMeshProUGUI upgradeText;

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateStats.ApplyGlobalUpgrades(upgrade, false);
        levelupManager.CloseUI();
    }
    public void SetUI()
    {
        if (upgrade != null)
        {
            upgradeText.text = UpdateStats.FormatItemUpgradeStatsToString(upgrade.levelModifiersList[0]); //Get stats string
        }
    }
}
