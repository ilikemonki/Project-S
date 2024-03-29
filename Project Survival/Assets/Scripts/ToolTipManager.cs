using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
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
        instance = this;
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
        instance.ClearToolTip();
        instance.nameText.text = itemDesc.itemName;
        instance.tagText.text = itemDesc.itemTags;
        instance.descriptionText.text = itemDesc.description;
        if (itemDesc.itemType == ItemDescription.ItemType.PassiveItem || itemDesc.itemType == ItemDescription.ItemType.SkillGem)
        {
            instance.layoutElement1.enabled = false;
            instance.layoutElement2.enabled = false;
            instance.statText.text = UpdateStats.FormatItemUpgradeStatsToString(itemDesc.upgrade.levelModifiersList[0]);
        }
        else if (itemDesc.itemType == ItemDescription.ItemType.SkillOrb) //skillorb's itemDesc doesn't have upgrade variable. Get from dragItem
        {
            instance.layoutElement1.enabled = true;
            instance.layoutElement2.enabled = true;
            DraggableItem dragItem = itemDesc.gameObject.GetComponent<DraggableItem>();
            if (dragItem == null) //when item is in shop.
            {
                instance.CalculateSkillControllerPrefabStats(itemDesc);
                instance.statText.text = UpdateStats.FormatSkillStatsToString(instance.skillControllerToolTip);
                instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(instance.skillControllerToolTip.levelUpgrades);
            }
            else
            {
                if (dragItem.skillController != null) //if Orb's skill is equiped and exist, Get those stats and set as text.
                {
                    instance.statText.text = UpdateStats.FormatSkillStatsToString(dragItem.skillController);
                    instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(dragItem.skillController.levelUpgrades);
                }
                else // if skill isn't equipped, get it from the prefab, set it to scToolTip, calculate its stats, then set as text.
                {
                    instance.CalculateSkillControllerPrefabStats(itemDesc);
                    instance.statText.text = UpdateStats.FormatSkillStatsToString(instance.skillControllerToolTip);
                    instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(instance.skillControllerToolTip.levelUpgrades);
                }
            }
        }
        instance.itemToolTipWindow.SetActive(true);
    }
    public static void HideToolTip()
    {
        instance.itemToolTipWindow.SetActive(false);
    }
    public void ClearToolTip()
    {
        instance.nameText.text = string.Empty;
        instance.tagText.text = string.Empty;
        instance.descriptionText.text = string.Empty;
        instance.statText.text = string.Empty;
        instance.skillUpgradesText.text = string.Empty;
    }
    public void CalculateSkillControllerPrefabStats(ItemDescription itemDesc) //Calculate skill's prefab stats to show it's current stats in tooltip if skill not equipped
    {
        foreach (SkillController prefab in itemManager.skillControllerPrefabsList) //Look for skill controller name.
        {
            if (prefab.skillOrbDescription.itemName.Equals(itemDesc.itemName)) //Get skill controller and set to skillControllerTooltip
            {
                ClearToolTipSkillController();
                if (itemManager.skillLevelDict.ContainsKey(itemDesc.itemName))
                {
                    skillControllerToolTip.level = itemManager.skillLevelDict[itemDesc.itemName];
                    skillControllerToolTip.exp = itemManager.skillExpDict[itemDesc.itemName];
                }
                else
                {
                    skillControllerToolTip.level = 1;
                    skillControllerToolTip.exp = 0;
                }
                skillControllerToolTip.isMelee = prefab.isMelee;
                skillControllerToolTip.baseAilmentsChance.AddRange(prefab.baseAilmentsChance);
                skillControllerToolTip.baseAilmentsEffect.AddRange(prefab.baseAilmentsEffect);
                skillControllerToolTip.baseAttackRange = prefab.baseAttackRange;
                skillControllerToolTip.baseChain = prefab.baseChain;
                skillControllerToolTip.baseCombo = prefab.baseCombo;
                skillControllerToolTip.baseCooldown = prefab.baseCooldown;
                skillControllerToolTip.baseCriticalChance = prefab.baseCriticalChance;
                skillControllerToolTip.baseCriticalDamage = prefab.baseCriticalDamage;
                skillControllerToolTip.baseDamageTypes.AddRange(prefab.baseDamageTypes);
                skillControllerToolTip.baseKnockBack = prefab.baseKnockBack;
                skillControllerToolTip.baseLifeSteal = prefab.baseLifeSteal;
                skillControllerToolTip.baseLifeStealChance = prefab.baseLifeStealChance;
                skillControllerToolTip.basePierce = prefab.basePierce;
                skillControllerToolTip.baseProjectile = prefab.baseProjectile;
                skillControllerToolTip.size = prefab.size;
                skillControllerToolTip.baseStrike = prefab.baseStrike;
                skillControllerToolTip.baseTravelRange = prefab.baseTravelRange;
                skillControllerToolTip.baseTravelSpeed = prefab.baseTravelSpeed;
                skillControllerToolTip.addedAilmentsChance.AddRange(prefab.addedAilmentsChance);
                skillControllerToolTip.addedAilmentsEffect.AddRange(prefab.addedAilmentsEffect);
                skillControllerToolTip.addedAttackRange = prefab.addedAttackRange;
                skillControllerToolTip.addedBaseDamageTypes.AddRange(prefab.addedBaseDamageTypes);
                skillControllerToolTip.addedChain = prefab.addedChain;
                skillControllerToolTip.addedCombo = prefab.addedCombo;
                skillControllerToolTip.addedCooldown = prefab.addedCooldown;
                skillControllerToolTip.addedCriticalChance = prefab.addedCriticalChance;
                skillControllerToolTip.addedCriticalDamage = prefab.addedCriticalDamage;
                skillControllerToolTip.addedDamage = prefab.addedDamage;
                skillControllerToolTip.addedDamageTypes.AddRange(prefab.addedDamageTypes);
                skillControllerToolTip.addedDuration = prefab.addedDuration;
                skillControllerToolTip.addedKnockBack = prefab.addedKnockBack;
                skillControllerToolTip.addedLifeSteal = prefab.addedLifeSteal;
                skillControllerToolTip.addedLifeStealChance = prefab.addedLifeStealChance;
                skillControllerToolTip.addedPierce = prefab.addedPierce;
                skillControllerToolTip.addedProjectile = prefab.addedProjectile;
                skillControllerToolTip.addedSize = prefab.addedSize;
                skillControllerToolTip.addedStrike = prefab.addedStrike;
                skillControllerToolTip.addedTravelRange = prefab.addedTravelRange;
                skillControllerToolTip.addedTravelSpeed = prefab.addedTravelSpeed;
                skillControllerToolTip.levelUpgrades.levelModifiersList.AddRange(prefab.levelUpgrades.levelModifiersList);
                if (itemManager.skillLevelDict.ContainsKey(itemDesc.itemName))
                {
                    for (int i = 0; i < itemManager.skillLevelDict[itemDesc.itemName] - 1; i++) //Apply the level upgrades.
                    {
                        UpdateStats.ApplySkillUpgrades(prefab.levelUpgrades, skillControllerToolTip, i);
                    }
                }
                skillControllerToolTip.UpdateSkillStats();
                break;
            }
        }
    }
    public void ClearToolTipSkillController()
    {
        skillControllerToolTip.baseAilmentsChance.Clear();
        skillControllerToolTip.baseAilmentsEffect.Clear();
        skillControllerToolTip.baseDamageTypes.Clear();
        skillControllerToolTip.addedAilmentsChance.Clear();
        skillControllerToolTip.addedAilmentsEffect.Clear();
        skillControllerToolTip.addedBaseDamageTypes.Clear();
        skillControllerToolTip.addedDamageTypes.Clear();
        skillControllerToolTip.levelUpgrades.levelModifiersList.Clear(); 
        for (int i = 0; i < skillControllerToolTip.damageTypes.Count; i++)
        {
            skillControllerToolTip.damageTypes[i] = 0;
            skillControllerToolTip.ailmentsChance[i] = 0;
            skillControllerToolTip.ailmentsEffect[i] = 0;
        }
        skillControllerToolTip.damage = 0;
        skillControllerToolTip.travelSpeed = 0;
        skillControllerToolTip.attackRange = 0;
        skillControllerToolTip.travelRange = 0;
        skillControllerToolTip.duration = 0;
        skillControllerToolTip.cooldown = 0;
        skillControllerToolTip.knockBack = 0;
        skillControllerToolTip.criticalChance = 0; 
        skillControllerToolTip.criticalDamage = 0;
        skillControllerToolTip.size = 0;
        skillControllerToolTip.lifeStealChance = 0; 
        skillControllerToolTip.lifeSteal = 0;
        skillControllerToolTip.strike = 0;
        skillControllerToolTip.combo = 0;
        skillControllerToolTip.projectile = 0;
        skillControllerToolTip.pierce = 0;
        skillControllerToolTip.chain = 0;
    }
}
