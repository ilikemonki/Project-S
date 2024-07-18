using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public SkillController skillController;
        public ActiveSkillDrop activeSkillDrop;
        public Toggle autoToggle;
        public List<ActiveSkillDrop> skillGemDropList;
    }
    public List<Skill> activeSkillList = new();
    public List<TextMeshProUGUI> generalStatistics = new(); //Statistics. List must be in order of the GameManager.totalIntStats and totalFloatStats list.
    public List<ActiveSkillUI> activeSkillUIList = new();
    public SkillSlotUI uiPrefab;
    public GameObject skillParent;
    public GameObject skillPoolParent;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public GameplayManager gameplayManager;
    public InventorySkillDrop inventoryOrbDrop, inventoryGemDrop;
    public ItemManager itemManager;
    public PItemSlotUI pItemSlotUIPrefab;
    public GameObject pItemInventoryParent;
    public TextMeshProUGUI playerStats1Text, playerStats2Text, enemyStatsText;
    private void Start()
    {
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public void DropInInventory(DraggableItem draggableItem)
    {
        if(draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
        {
            activeSkillUIList[draggableItem.activeSkillDrop.activeSlotNum].RemoveSkill();
            draggableItem.slotUI.inUseText.gameObject.SetActive(false);
            activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController = null;
            draggableItem.activeSkillDrop.draggableItem = null;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = draggableItem.slotUI.fadedImage.transform;   //set new parent
            draggableItem.transform.SetParent(draggableItem.currentParent);
            draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.activeSlotNum + 1).ToString();
            draggableItem.activeSkillDrop = null;
            //Destroy Skill Controller when moving orb to inventory.
            draggableItem.skillController.enabled = false;
            Destroy(draggableItem.skillController.poolParent);
            Destroy(draggableItem.skillController.stayOnPlayerParent);
            Destroy(draggableItem.skillController.gameObject);
            draggableItem.skillController = null;
            PItemEffectManager.CheckAllPItemCondition(PItemEffectManager.ConditionTag.ActiveSkill);
        }
        else
        {
            if (draggableItem.slotUI.inUseText.gameObject.activeSelf) //if there are no more gems in inventory, move active dragItem to Inventory slotUI. Else just add to quantity.
            {
                draggableItem.slotUI.inUseText.gameObject.SetActive(false);
                draggableItem.activeSkillDrop.draggableItem = null;
                draggableItem.isInInventory = true;
                draggableItem.currentParent = draggableItem.slotUI.fadedImage.transform;   //set new parent
                draggableItem.transform.SetParent(draggableItem.currentParent);
                draggableItem.activeSkillDrop = null;
                foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
                {
                    if (draggableItem.itemDescription.itemName.Equals(dItem.itemDescription.itemName))
                    {
                        draggableItem.itemDescription.quantityInInventory = 1;
                        int numInInventory = itemManager.skillGemList[dItem];
                        itemManager.skillGemList.Remove(dItem);
                        itemManager.skillGemList.Add(draggableItem, numInInventory);
                        break;
                    }
                }
                draggableItem.slotUI.amountText.text = draggableItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[draggableItem].ToString();
            }
            else
            {
                foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
                {
                    if (draggableItem.itemDescription.itemName.Equals(dItem.itemDescription.itemName))
                    {
                        dItem.itemDescription.quantityInInventory++;
                        dItem.slotUI.amountText.text = dItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[dItem].ToString();
                        draggableItem.activeSkillDrop.draggableItem = null;
                        Destroy(draggableItem.gameObject, 0);   //Destroy the dragged item.
                        break;
                    }
                }
            }
        }
    }
    public void DropInActiveSkill(DraggableItem draggableItem, Transform parent) //From Inventory to Active Skill slot
    {
        if (draggableItem.itemDescription.quantityInInventory > 1 && draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillGem) //if there is more than 1 amount in inventory, spawn new dragItem and put it back in inventory
        {
            foreach (DraggableItem prefab in itemManager.t1GemPrefabList)
            {
                if (draggableItem.itemDescription.itemName.Equals(prefab.itemDescription.itemName))
                {
                    DraggableItem newItem = Instantiate(prefab, draggableItem.slotUI.fadedImage.transform); //Create new item back into inventory
                    newItem.inventory = this;
                    newItem.slotUI = draggableItem.slotUI;
                    newItem.name = draggableItem.name;
                    newItem.itemDescription.itemName = draggableItem.itemDescription.itemName;
                    newItem.itemDescription.itemSprite = draggableItem.itemDescription.itemSprite;
                    newItem.itemDescription.itemType = draggableItem.itemDescription.itemType;
                    newItem.itemDescription.quantityInInventory = draggableItem.itemDescription.quantityInInventory;
                    newItem.itemDescription.quantityInInventory--;
                    itemManager.skillGemList.Add(newItem, itemManager.skillGemList[draggableItem]);
                    itemManager.skillGemList.Remove(draggableItem);
                    newItem.slotUI.amountText.text = newItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[newItem].ToString();
                    newItem.isInInventory = true;
                    newItem.currentParent = draggableItem.currentParent;   //set new parent
                    //draggableItem gets dropped in active skill slot
                    draggableItem.isInInventory = false;
                    draggableItem.currentParent = parent;   //set new parent}
                    break;
                }
            }
        }
        else //only 1 amount in inventory, move dragItem and set SlotUI to have no dragItem.
        {
            draggableItem.isInInventory = false;
            draggableItem.currentParent = parent;   //set new parent
            if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillGem)
            {
                draggableItem.itemDescription.quantityInInventory--;
                draggableItem.slotUI.amountText.text = draggableItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[draggableItem].ToString();
            }
            draggableItem.slotUI.inUseText.gameObject.SetActive(true);
        }
        InstantiateSkill(draggableItem);
        PItemEffectManager.CheckAllPItemCondition(PItemEffectManager.ConditionTag.ActiveSkill);
    }
    public void InstantiateSkill(DraggableItem dragItem) //Add skill to gameplay
    {
        if (dragItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
        {
            foreach (SkillController sc in itemManager.skillControllerPrefabsList) //Look for skill controller name.
            {
                if (sc.skillOrbDescription.itemName.Equals(dragItem.itemDescription.itemName)) //Get skill controller and set to draggableItem
                {
                    SkillController skill = Instantiate(sc, skillParent.transform);
                    skill.name = sc.name;
                    skill.poolParent.transform.SetParent(skillPoolParent.transform);
                    skill.player = player;
                    skill.enemyManager = enemyManager;
                    skill.gameplayManager = gameplayManager;
                    if (!itemManager.skillExpDict.ContainsKey(skill.skillOrbDescription.itemName)) //if skill isn't saved to dictionary, add and save it.
                    {
                        itemManager.skillExpDict.Add(skill.skillOrbDescription.itemName, skill.exp);
                        itemManager.skillLevelDict.Add(skill.skillOrbDescription.itemName, skill.level);
                    }
                    else //set skill's exp and level from skill's saved info.
                    {
                        skill.level = itemManager.skillLevelDict[skill.skillOrbDescription.itemName];
                        skill.exp = itemManager.skillExpDict[skill.skillOrbDescription.itemName];
                    }
                    skill.UpdateSkillStats();
                    if (skill.level > 1)
                    {
                        for (int i = 0; i < skill.level - 1; i++)
                        {
                            UpdateStats.ApplySkillUpgrades(skill.levelUpgrades, skill, i);
                        }
                    }
                    dragItem.skillController = skill;
                    activeSkillList[dragItem.activeSkillDrop.activeSlotNum].skillController = skill;
                    activeSkillUIList[dragItem.activeSkillDrop.activeSlotNum].SetSkill(activeSkillList[dragItem.activeSkillDrop.activeSlotNum].skillController);
                    return;
                }
            }
        }
    }
    public void AddCollectibleIntoInventory(string itemName)
    {
        if (itemName.Contains("Orb")) //Orb
        {
            foreach (DraggableItem dItem in itemManager.skillOrbList.Keys) //Skill gains exp if there is already exist.
            {
                if (dItem.itemDescription.itemName.Equals(itemName))
                {
                    foreach (Skill skill in activeSkillList) //if skill is in active slot, add exp to skill controller. Else add to skill save data.
                    {
                        if (skill.skillController != null)
                        {
                            if (skill.skillController.skillOrbDescription.itemName.Equals(itemName))
                            {
                                skill.skillController.GainExp(gameplayManager.expOrbBonus);
                                dItem.slotUI.levelText.text = "Lv. " + skill.skillController.level;
                                skill.activeSkillDrop.nameText.text = "Lv. " + skill.skillController.level.ToString() + " " + itemName;
                                return;
                            }
                        }
                    }
                    //Add exp to skill save data.
                    if (itemManager.skillLevelDict[itemName] < 5)
                    {
                        itemManager.skillExpDict[itemName] += gameplayManager.expOrbBonus;
                        if (itemManager.skillExpDict[itemName] >= gameplayManager.skillExpCapList[itemManager.skillLevelDict[itemName] - 1]) //check if level up
                        {
                            itemManager.skillExpDict[itemName] -= gameplayManager.skillExpCapList[itemManager.skillLevelDict[itemName] - 1];
                            itemManager.skillLevelDict[itemName]++; 
                            if (itemManager.skillLevelDict[itemName] < 5)
                            {
                                if (itemManager.skillExpDict[itemName] >= gameplayManager.skillExpCapList[itemManager.skillLevelDict[itemName] - 1]) //if skill levels up 2 times on one exp gain. level up again. Should not be able to level more than 2.
                                {
                                    itemManager.skillExpDict[itemName] -= gameplayManager.skillExpCapList[itemManager.skillLevelDict[itemName] - 1];
                                    itemManager.skillLevelDict[itemName]++;
                                }
                            }
                        }
                    }
                    dItem.slotUI.levelText.text = "Lv. " + itemManager.skillLevelDict[itemName];
                    return;
                }
            }
            if (!itemManager.skillExpDict.ContainsKey(itemName)) //if skill isn't saved to dictionary, add and save it.
            {
                Debug.Log(itemName + " Add into Dictionary");
                itemManager.skillExpDict.Add(itemName, 0);
                itemManager.skillLevelDict.Add(itemName, 1);
            }
            //Create DraggbleItem for orb.
            foreach (DraggableItem prefab in itemManager.orbPrefabList)
            {
                if (prefab.itemDescription.itemName.Equals(itemName))
                {
                    DraggableItem draggableItem = Instantiate(prefab, inventoryOrbDrop.contentParent.transform);
                    draggableItem.inventory = this;
                    //Create new slotUI
                    SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryOrbDrop.contentParent.transform);
                    draggableItem.slotUI = slotUI;
                    draggableItem.isInInventory = true;
                    draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
                    draggableItem.transform.SetParent(draggableItem.currentParent);
                    slotUI.name = itemName;
                    slotUI.nameText.text = draggableItem.itemDescription.itemName;
                    slotUI.levelText.text = "Lv. " + draggableItem.itemDescription.currentLevel.ToString();
                    slotUI.fadedImage.sprite = draggableItem.itemDescription.itemSprite;
                    itemManager.skillOrbList.Add(draggableItem, 1);
                    return;
                }
            }
        }
        else //Gem
        {
            foreach (DraggableItem dItem in itemManager.skillGemList.Keys) //if gem exist, add to quantity and gem inventory quantity
            {
                if (dItem.itemDescription.itemName.Equals(itemName)) //If the gem inventory slotUI has no more quantity left, then create and set new dragItem to slotUI. Else, add to inventory.
                {
                    if (dItem.slotUI.inUseText.gameObject.activeSelf == true)
                    {
                        foreach (DraggableItem prefab in itemManager.t1GemPrefabList) //Create new dragItem, set to slotUI
                        {
                            if (prefab.itemDescription.itemName.Equals(itemName))
                            {
                                dItem.slotUI.inUseText.gameObject.SetActive(false);
                                DraggableItem draggableItem = Instantiate(prefab, inventoryGemDrop.contentParent.transform);
                                draggableItem.inventory = this;
                                draggableItem.slotUI = dItem.slotUI;
                                draggableItem.isInInventory = true;
                                draggableItem.currentParent = dItem.slotUI.fadedImage.transform;   //set new parent
                                draggableItem.transform.SetParent(draggableItem.currentParent);
                                draggableItem.itemDescription.quantityInInventory++;
                                int numInInventory = itemManager.skillGemList[dItem];
                                itemManager.skillGemList.Remove(dItem);
                                itemManager.skillGemList.Add(draggableItem, numInInventory + 1);
                                draggableItem.slotUI.amountText.text = draggableItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[draggableItem].ToString();
                                return;
                            }
                        }
                    }
                    else
                    {
                        dItem.itemDescription.quantityInInventory++;
                        itemManager.skillGemList[dItem]++;
                        dItem.slotUI.amountText.text = dItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[dItem].ToString();
                    }
                    return;
                }
            }
            //Create DraggbleItem for gem.
            foreach (DraggableItem prefab in itemManager.t1GemPrefabList)
            {
                if (prefab.itemDescription.itemName.Equals(itemName))
                {
                    DraggableItem draggableItem = Instantiate(prefab, inventoryGemDrop.contentParent.transform);
                    draggableItem.inventory = this;
                    //Create new slotUI
                    SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryGemDrop.contentParent.transform);
                    draggableItem.slotUI = slotUI;
                    draggableItem.isInInventory = true;
                    draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
                    draggableItem.transform.SetParent(draggableItem.currentParent);
                    draggableItem.itemDescription.quantityInInventory++;
                    slotUI.name = itemName;
                    slotUI.nameText.text = draggableItem.itemDescription.itemName;
                    slotUI.levelText.text = "Tier " + draggableItem.itemDescription.currentLevel.ToString();
                    slotUI.fadedImage.sprite = draggableItem.itemDescription.itemSprite;
                    itemManager.skillGemList.Add(draggableItem, 1);
                    draggableItem.slotUI.amountText.text = draggableItem.itemDescription.quantityInInventory + "/" + itemManager.skillGemList[draggableItem].ToString();
                    return;
                }
            }
        }
    }
    public void UpdateGeneralStatistics()
    {
        for (int i = 0; i < generalStatistics.Count; i++)
        {
            switch (i)
            {
                case 0: generalStatistics[i].text = GameManager.totalTime.ToString(); break;
                case 1: generalStatistics[i].text = GameManager.totalKills.ToString(); break;
                case 2: generalStatistics[i].text = GameManager.totalExp.ToString(); break;
                case 3: generalStatistics[i].text = gameplayManager.level.ToString(); break;
                case 4: generalStatistics[i].text = (gameplayManager.waveCounter + 1).ToString(); break;
                case 5: generalStatistics[i].text = GameManager.totalCoinsCollected.ToString(); break;
                //case 6: generalStatistics[i].text = GameManager.totalClassStarsCollected.ToString(); break;
                case 7: generalStatistics[i].text = GameManager.totalPassiveItems.ToString(); break;
                case 8: generalStatistics[i].text = GameManager.totalSkillGemsCollected.ToString(); break;
                case 9: generalStatistics[i].text = GameManager.totalDashes.ToString(); break;
                case 10: generalStatistics[i].text = GameManager.totalDamageDealt.ToString(); break;
                case 11: generalStatistics[i].text = GameManager.totalPhysicalDamage.ToString(); break;
                case 12: generalStatistics[i].text = GameManager.totalFireDamage.ToString(); break;
                case 13: generalStatistics[i].text = GameManager.totalColdDamage.ToString(); break;
                case 14: generalStatistics[i].text = GameManager.totalLightningDamage.ToString(); break;
                case 15: generalStatistics[i].text = GameManager.TotalDotDamage.ToString(); break;
                case 16: generalStatistics[i].text = GameManager.totalBleedDamage.ToString(); break;
                case 17: generalStatistics[i].text = GameManager.totalBurnDamage.ToString(); break;
                case 18: generalStatistics[i].text = GameManager.totalCrits.ToString(); break;
                case 19: generalStatistics[i].text = GameManager.totalSkillsUsed.ToString(); break;
                case 20: generalStatistics[i].text = GameManager.totalDamageTaken.ToString(); break;
                case 21: generalStatistics[i].text = GameManager.totalHealing.ToString(); break;
                case 22: generalStatistics[i].text = GameManager.totalRegen.ToString(); break;
                case 23: generalStatistics[i].text = GameManager.totalDegen.ToString(); break;
                case 24: generalStatistics[i].text = GameManager.totalLifeStealProc.ToString(); break;
                case 25: generalStatistics[i].text = GameManager.totalLifeSteal.ToString(); break;
                case 26: generalStatistics[i].text = GameManager.totalBleed.ToString(); break;
                case 27: generalStatistics[i].text = GameManager.totalBurn.ToString(); break;
                case 28: generalStatistics[i].text = GameManager.totalChill.ToString(); break;
                case 29: generalStatistics[i].text = GameManager.totalShock.ToString(); break;
                //default: GameManager.DebugLog("General Stats has no switch case for " + i); break;
            }
        }
    }
    public void AutoToggle() //Set to toggle
    {
        foreach (Skill skillSlot in activeSkillList) //Set skills in start.
        {
            if (skillSlot.skillController != null && !skillSlot.skillController.automaticOnly)
            {
                skillSlot.skillController.SetAutomatic(skillSlot.autoToggle.isOn);
                skillSlot.skillController.CheckTargetless();
            }
        }
    }
    public void UpdatePassiveItemsInventory(ItemDescription item) //Create new pItem slotUI and add to inventory
    {
        if (item.quantityInInventory > 1) //if there is already the pItem here, update the UI.
        {
            item.pItemSlotUI.quanityText.text = item.quantityInInventory.ToString() + "/" + item.maxQuantity.ToString();
        }
        else
        {
            PItemSlotUI pItemUI = Instantiate(pItemSlotUIPrefab, pItemInventoryParent.transform);
            pItemUI.itemDescription = item;
            pItemUI.itemDescription.pItemSlotUI = pItemUI;
            pItemUI.image.sprite = item.itemSprite;
            pItemUI.quanityText.text = item.quantityInInventory.ToString() + "/" + item.maxQuantity.ToString();
            ToolTipTrigger tooltip = pItemUI.gameObject.GetComponent<ToolTipTrigger>();
            if (tooltip != null) tooltip.itemDesc = item;
        }
    }
}
