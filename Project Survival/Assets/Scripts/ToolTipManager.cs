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
    public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText;
    public Image itemImage;
    public void Awake()
    {
        current = this;
    }
    public void Update()
    {
        if (itemToolTipWindow.activeSelf)
        {
            Vector2 anchorPos = Input.mousePosition / canvasRectTransform.localScale.x;
            if (anchorPos.x + rectTransformWindow.rect.width > canvasRectTransform.rect.width)
                anchorPos.x = canvasRectTransform.rect.width - rectTransformWindow.rect.width;
            if (anchorPos.y + rectTransformWindow.rect.height > canvasRectTransform.rect.height)
                anchorPos.y = canvasRectTransform.rect.height - rectTransformWindow.rect.height;
            rectTransformWindow.anchoredPosition = anchorPos;
        }
    }
    public static void ShowItemToolTip(ItemDescription itemDesc)
    {
        current.ClearToolTip();
        current.itemImage.sprite = itemDesc.itemSprite;
        current.nameText.text = itemDesc.itemName;
        current.tagText.text = itemDesc.itemTags;
        if (itemDesc.itemType == ItemDescription.ItemType.PassiveItem)
        {
            if (string.IsNullOrWhiteSpace(itemDesc.description)) //if there is no description. set statOnlyText.text
            {
                current.statOnlyText.text = current.updateStats.FormatCurrentLevelStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.currentLevel]);
            }
            else
            {
                current.descriptionText.text = itemDesc.description;
                current.statText.text = current.updateStats.FormatCurrentLevelStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.currentLevel]);
            }
        }
        else if (itemDesc.itemType == ItemDescription.ItemType.SkillOrb)
        {
            current.descriptionText.text = itemDesc.description;
            DraggableItem dragItem = itemDesc.gameObject.GetComponent<DraggableItem>();
            if (dragItem.skillController != null) //if Orb's skill is equiped and exist, Get those stats and set as text.
            {
                current.statText.text = current.updateStats.FormatSkillStatsToString(dragItem.skillController);
            }
            else // if skill doesn't exist, get it from the prefab, set it to scToolTip, calculate stats, then set as text.
            {
                current.CalculateSkillControllerPrefabStats(itemDesc);
                current.statText.text = current.updateStats.FormatSkillStatsToString(current.skillControllerToolTip);
            }
        }
        else if (itemDesc.itemType == ItemDescription.ItemType.SkillGem)
        {
            if (string.IsNullOrWhiteSpace(itemDesc.description)) //if there is no description. set statOnlyText.text
            {
                current.statOnlyText.text = current.updateStats.FormatCurrentLevelStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.currentLevel]);
            }
            else
            {
                current.descriptionText.text = itemDesc.description;
                current.statText.text = current.updateStats.FormatCurrentLevelStatsToString(itemDesc.upgrade.levelModifiersList[itemDesc.currentLevel]);
            }
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
    public void CalculateSkillControllerPrefabStats(ItemDescription itemDesc) //Set prefab stats
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
                updateStats.ApplySkillUpgrades(prefab.levelUpgrades, skillControllerToolTip, i);
            }
            break;
        }
    }
}
