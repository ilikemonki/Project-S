using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager current;
    public UpdateStats updateStats;
    public RectTransform canvasRectTransform;
    public ItemManager itemManager;
    public SkillController skillControllerToolTip; //Sets skill stats into this controller to calculate the stats to show in tool tip.
    [Header("Item Tool Tip")]
    public GameObject itemToolTipWindow;
    public RectTransform rectTransformWindow;
    public TextMeshProUGUI nameText, tagText, descriptionText, statText, skillUpgradesText;
    public LayoutElement layoutElement1, layoutElement2;
    Vector2 anchorPos;
    public void Awake()
    {
        current = this;
        itemToolTipWindow.SetActive(false);
    }
    public void Update()
    {
        if (itemToolTipWindow.activeSelf)
        {
            anchorPos = Input.mousePosition;
            if (anchorPos.x + rectTransformWindow.rect.width > canvasRectTransform.rect.width)
                anchorPos.x = canvasRectTransform.rect.width - rectTransformWindow.rect.width;
            if (anchorPos.y < rectTransformWindow.rect.height)
                anchorPos.y = rectTransformWindow.rect.height;
            rectTransformWindow.anchoredPosition = anchorPos;
        }
    }
    public static void ShowItemToolTip(ItemDescription itemDesc)
    {
        current.ClearToolTip();
        current.nameText.text = itemDesc.itemName;
        current.tagText.text = itemDesc.itemTags;
        current.descriptionText.text = itemDesc.description;
        if (itemDesc.itemType == ItemDescription.ItemType.PassiveItem || itemDesc.itemType == ItemDescription.ItemType.SkillGem)
        {
            current.layoutElement1.enabled = false;
            current.layoutElement2.enabled = false;
            current.statText.text = UpdateStats.FormatItemUpgradeStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.currentLevel]);
        }
        else if (itemDesc.itemType == ItemDescription.ItemType.SkillOrb) //skillorb's itemDesc doesn't have upgrade variable. Get from dragItem
        {
            current.layoutElement1.enabled = true;
            current.layoutElement2.enabled = true;
            DraggableItem dragItem = itemDesc.gameObject.GetComponent<DraggableItem>();
            if (dragItem.skillController != null) //if Orb's skill is equiped and exist, Get those stats and set as text.
            {
                current.statText.text = UpdateStats.FormatSkillStatsToString(dragItem.skillController);
            }
            else // if skill isn't equipped, get it from the prefab, set it to scToolTip, calculate its stats, then set as text.
            {
                current.CalculateSkillControllerPrefabStats(itemDesc);
                current.statText.text = UpdateStats.FormatSkillStatsToString(current.skillControllerToolTip);
            }
            current.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(dragItem.skillController.levelUpgrades);
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
        current.skillUpgradesText.text = string.Empty;
    }
    public void CalculateSkillControllerPrefabStats(ItemDescription itemDesc) //Calculate skill's prefab stats to show it's current stats in tooltip
    {
        foreach (SkillController prefab in itemManager.skillControllerPrefabsList) //Look for skill controller name.
        {
            if (prefab.skillOrbName.Equals(itemDesc.itemName)) //Get skill controller and set to skillControllerTooltip
            {
                skillControllerToolTip.baseAilmentsChance = prefab.baseAilmentsChance;
                skillControllerToolTip.baseAilmentsEffect = prefab.baseAilmentsEffect;
                skillControllerToolTip.baseAttackRange = prefab.baseAttackRange;
                skillControllerToolTip.baseChain = prefab.baseChain;
                skillControllerToolTip.baseCooldown = prefab.baseCooldown;
                skillControllerToolTip.baseCriticalChance = prefab.baseCriticalChance;
                skillControllerToolTip.baseCriticalDamage = prefab.baseCriticalDamage;
                skillControllerToolTip.baseDamage = prefab.baseDamage;
                skillControllerToolTip.baseDamageTypes = prefab.baseDamageTypes;
                skillControllerToolTip.baseKnockBack = prefab.baseKnockBack;
                skillControllerToolTip.baseLifeSteal = prefab.baseLifeSteal;
                skillControllerToolTip.baseLifeStealChance = prefab.baseLifeStealChance;
                skillControllerToolTip.basePierce = prefab.basePierce;
                skillControllerToolTip.baseProjectile = prefab.baseProjectile;
                skillControllerToolTip.size = prefab.size;
                skillControllerToolTip.baseStrike = prefab.baseStrike;
                skillControllerToolTip.baseTravelRange = prefab.baseTravelRange;
                skillControllerToolTip.baseTravelSpeed = prefab.baseTravelSpeed;
                skillControllerToolTip.addedAilmentsChance = prefab.addedAilmentsChance;
                skillControllerToolTip.addedAilmentsEffect = prefab.addedAilmentsEffect;
                skillControllerToolTip.addedAttackRange = prefab.addedAttackRange;
                skillControllerToolTip.addedChain = prefab.addedChain;
                skillControllerToolTip.addedCooldown = prefab.addedCooldown;
                skillControllerToolTip.addedCriticalChance = prefab.addedCriticalChance;
                skillControllerToolTip.addedCriticalDamage = prefab.addedCriticalDamage;
                skillControllerToolTip.addedDamage = prefab.addedDamage;
                skillControllerToolTip.addedDamageTypes = prefab.addedDamageTypes;
                skillControllerToolTip.addedKnockBack = prefab.addedKnockBack;
                skillControllerToolTip.addedLifeSteal = prefab.addedLifeSteal;
                skillControllerToolTip.addedLifeStealChance = prefab.addedLifeStealChance;
                skillControllerToolTip.addedPierce = prefab.addedPierce;
                skillControllerToolTip.addedProjectile = prefab.addedProjectile;
                skillControllerToolTip.addedSize = prefab.addedSize;
                skillControllerToolTip.addedStrike = prefab.addedStrike;
                skillControllerToolTip.addedTravelRange = prefab.addedTravelRange;
                skillControllerToolTip.addedTravelSpeed = prefab.addedTravelSpeed;
            }
            skillControllerToolTip.UpdateSkillStats();
            for (int i = 0; i < itemManager.skillLevelDict[itemDesc.itemName] - 1; i++)
            {
                UpdateStats.ApplySkillUpgrades(prefab.levelUpgrades, skillControllerToolTip, i);
            }
            break;
        }
    }
}
