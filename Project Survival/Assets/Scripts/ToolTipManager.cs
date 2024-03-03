using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager current;
    public UpdateStats updateStats;
    [Header("Item Tool Tip")]
    public GameObject itemToolTipWindow;
    public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText;
    public Image itemImage;

    public void Awake()
    {
        current = this;
    }
    public static void ShowItemToolTip(ItemDescription itemDesc)
    {
        current.ClearToolTip();
        current.itemImage.sprite = itemDesc.itemSprite;
        current.nameText.text = itemDesc.itemName;
        current.tagText.text = itemDesc.itemTags;
        current.descriptionText.text = itemDesc.description;
        if (string.IsNullOrWhiteSpace(itemDesc.description)) //if there is no description. set statOnlyText.text
        {
            //current.statOnlyText.text = current.updateStats.FormatStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.upgrade.itemDescription.currentLevel]);
        }
        else
        {
            current.descriptionText.text = itemDesc.description;
            //current.statText.text = current.updateStats.FormatStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.upgrade.itemDescription.currentLevel]);
        }
        current.itemToolTipWindow.SetActive(true);
    }
    public static void HideToolTip()
    {
        current.itemToolTipWindow.SetActive(false);
    }
    public void ClearToolTip()
    {
        current.nameText.text = string.Empty;
        current.tagText.text = string.Empty;
        current.descriptionText.text = string.Empty;
        current.statText.text = string.Empty;
        current.statOnlyText.text = string.Empty;
    }
}
