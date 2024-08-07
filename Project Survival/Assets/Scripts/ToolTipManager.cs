using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
    public RectTransform canvasRectTransform;
    public ItemManager itemManager;
    public SkillController skillControllerToolTip; //Sets skill stats into this controller to calculate the stats to show in tool tip.
    [Header("Item Tool Tip")]
    public GameObject itemToolTipWindow;
    public RectTransform rectTransformWindow;
    public TextMeshProUGUI nameText, tagText, descriptionText, statText, skillUpgradesText, showBaseStatsText;
    public LayoutElement layoutElement1, layoutElement2;
    public bool showBaseStats;
    ItemDescription itemDescription;
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
            anchorPos = Input.mousePosition / canvasRectTransform.localScale.x;
            if (anchorPos.x + rectTransformWindow.rect.width > canvasRectTransform.rect.width)
                anchorPos.x = canvasRectTransform.rect.width - rectTransformWindow.rect.width;
            if (anchorPos.y < rectTransformWindow.rect.height)
                anchorPos.y = rectTransformWindow.rect.height;
            rectTransformWindow.anchoredPosition = anchorPos;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (instance.itemDescription!= null)
                {
                    if (instance.itemDescription.itemType.Equals(ItemDescription.ItemType.SkillOrb))
                    {
                        if (instance.showBaseStats)
                        {
                            showBaseStatsText.text = "Press 'Tab' to show base stats.";
                            instance.showBaseStats = false;
                        }
                        else
                        {
                            showBaseStatsText.text = "Press 'Tab' to show total stats.";
                            instance.showBaseStats = true;
                        }
                        ShowItemToolTip(itemDescription);
                    }
                }
            }
        }
    }
    public static void ShowItemToolTip(ItemDescription itemDesc)
    {
        instance.itemDescription = itemDesc;
        instance.ClearToolTip();
        if (instance.showBaseStats)
            instance.nameText.text = itemDesc.itemName + " (Base)";
        else
            instance.nameText.text = itemDesc.itemName;
        instance.tagText.text = itemDesc.itemTags;
        if (!string.IsNullOrWhiteSpace(itemDesc.behavior))
        {
            instance.tagText.text += "\nBehavior: " + itemDesc.behavior;
        }
        instance.descriptionText.text = itemDesc.description;
        if (itemDesc.itemType.Equals(ItemDescription.ItemType.PassiveItem)|| itemDesc.itemType.Equals(ItemDescription.ItemType.SkillGem))
        {
            instance.showBaseStatsText.text = "";
            if (itemDesc.pItemEffect != null) //if has passive effect
            {
                if (itemDesc.pItemEffect.damage > 0)
                    instance.descriptionText.text += "\n<color=orange>Damage: </color>" + itemDesc.pItemEffect.damage;
                if (itemDesc.pItemEffect.chance > 0)
                    instance.descriptionText.text += "\n<color=orange>Chance: </color>" + itemDesc.pItemEffect.chance + "%";
                if (itemDesc.pItemEffect.cooldown > 0)
                    instance.descriptionText.text += "\n<color=orange>Cooldown: </color>" + itemDesc.pItemEffect.cooldown + "s";
                if (!string.IsNullOrWhiteSpace(itemDesc.pItemEffect.totalrecordedString))
                    instance.descriptionText.text += "\n\n" + itemDesc.pItemEffect.totalrecordedString + " " + itemDesc.pItemEffect.totalRecorded.ToString();
            }
            if (itemDesc.description.Length >= 40) //if description is 40+ length, limit window
            {
                instance.layoutElement1.enabled = true;
                instance.layoutElement2.enabled = true;
            }
            else
            {
                instance.layoutElement1.enabled = false;
                instance.layoutElement2.enabled = false;
            }
            if (itemDesc.upgrade.levelModifiersList.Count > 0)
                instance.statText.text = UpdateStats.FormatItemUpgradeStatsToString(itemDesc.upgrade.levelModifiersList[0]);
        }
        else if (itemDesc.itemType.Equals(ItemDescription.ItemType.SkillOrb)) //skillorb's itemDesc doesn't have upgrade variable. Get from dragItem
        {
            if (instance.showBaseStats)
                instance.showBaseStatsText.text = "Press 'Tab' to show total stats.";
            else
                instance.showBaseStatsText.text = "Press 'Tab' to show base stats.";
            instance.layoutElement1.enabled = true;
            instance.layoutElement2.enabled = true;
            DraggableItem dragItem = itemDesc.gameObject.GetComponent<DraggableItem>();
            if (dragItem == null) //when item is in shop.
            {
                instance.CalculateSkillControllerPrefabStats(itemDesc);
                if (instance.showBaseStats)
                    instance.statText.text = UpdateStats.FormatBaseSkillStatsToString(instance.skillControllerToolTip);
                else
                    instance.statText.text = UpdateStats.FormatSkillStatsToString(instance.skillControllerToolTip);
                instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(instance.skillControllerToolTip.levelUpgrades);
            }
            else
            {
                if (dragItem.skillController != null) //if Orb's skill is equiped and exist, Get those stats and set as text.
                {
                    instance.CalculateSkillControllerPrefabStats(itemDesc);
                    if (instance.showBaseStats)
                    {
                        instance.statText.text = UpdateStats.FormatBaseSkillStatsToString(instance.skillControllerToolTip);
                        instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(instance.skillControllerToolTip.levelUpgrades);
                    }
                    else
                    {
                        instance.statText.text = UpdateStats.FormatSkillStatsToString(dragItem.skillController);
                        instance.skillUpgradesText.text = UpdateStats.FormatSkillUpgradesToString(dragItem.skillController.levelUpgrades);
                    }
                }
                else // if skill isn't equipped, get it from the prefab, set it to scToolTip, calculate its stats, then set as text.
                {
                    instance.CalculateSkillControllerPrefabStats(itemDesc);
                    if (instance.showBaseStats)
                        instance.statText.text = UpdateStats.FormatBaseSkillStatsToString(instance.skillControllerToolTip);
                    else
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
                skillControllerToolTip.baseProjectileAmount = prefab.baseProjectileAmount;
                skillControllerToolTip.aoe = prefab.aoe;
                skillControllerToolTip.baseMeleeAmount = prefab.baseMeleeAmount;
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
                skillControllerToolTip.addedProjectileAmount = prefab.addedProjectileAmount;
                skillControllerToolTip.addedAoe = prefab.addedAoe;
                skillControllerToolTip.addedMeleeAmount = prefab.addedMeleeAmount;
                skillControllerToolTip.addedTravelRange = prefab.addedTravelRange;
                skillControllerToolTip.addedTravelSpeed = prefab.addedTravelSpeed;
                if (showBaseStats)
                {
                    for (int i = 0; i < skillControllerToolTip.damageTypes.Count; i++)
                    {
                        skillControllerToolTip.addedAilmentsChance[i] = 0;
                        skillControllerToolTip.addedAilmentsEffect[i] = 0;
                        skillControllerToolTip.addedBaseDamageTypes[i] = 0;
                        skillControllerToolTip.addedDamageTypes[i] = 0;
                    }
                    skillControllerToolTip.addedAttackRange = 0;
                    skillControllerToolTip.addedChain = 0;
                    skillControllerToolTip.addedCombo = 0;
                    skillControllerToolTip.addedCooldown = 0;
                    skillControllerToolTip.addedCriticalChance = 0;
                    skillControllerToolTip.addedCriticalDamage = 0;
                    skillControllerToolTip.addedDamage = 0;
                    skillControllerToolTip.addedDuration = 0;
                    skillControllerToolTip.addedKnockBack = 0;
                    skillControllerToolTip.addedLifeSteal = 0;
                    skillControllerToolTip.addedLifeStealChance = 0;
                    skillControllerToolTip.addedPierce = 0;
                    skillControllerToolTip.addedProjectileAmount = 0;
                    skillControllerToolTip.addedAoe = 0;
                    skillControllerToolTip.addedMeleeAmount = 0;
                    skillControllerToolTip.addedTravelRange = 0;
                    skillControllerToolTip.addedTravelSpeed = 0;
                }
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
        skillControllerToolTip.aoe = 0;
        skillControllerToolTip.lifeStealChance = 0; 
        skillControllerToolTip.lifeSteal = 0;
        skillControllerToolTip.meleeAmount = 0;
        skillControllerToolTip.combo = 0;
        skillControllerToolTip.projectileAmount = 0;
        skillControllerToolTip.pierce = 0;
        skillControllerToolTip.chain = 0;
    }
}
