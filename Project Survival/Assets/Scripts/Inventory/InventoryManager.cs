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
    public SkillSlotUI uiPrefab;
    public GameObject skillParent;
    public GameObject skillPoolParent;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public GameplayManager gameplayManager;
    public InventorySkillDrop inventoryOrbDrop, inventoryGemDrop;
    public ItemManager itemManager;
    private void Start()
    {
        foreach(Skill skill in activeSkillList) //Set current skills at start of game.
        {
            if (skill.activeSkillDrop.draggableItem != null)
            {
                InstantiateSkill(skill.activeSkillDrop.draggableItem);
                skill.activeSkillDrop.nameText.text = "Lv. " + skill.skillController.level.ToString() + " " + skill.activeSkillDrop.draggableItem.itemName;
                SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryOrbDrop.contentParent.transform);
                skill.activeSkillDrop.draggableItem.slotUI = slotUI;
                slotUI.inUseText.gameObject.SetActive(true);
                slotUI.name = uiPrefab.name;
                slotUI.nameText.text = skill.activeSkillDrop.draggableItem.itemName;
                slotUI.levelText.text = "Lv. " + skill.skillController.level.ToString();
                slotUI.fadedImage.sprite = skill.activeSkillDrop.draggableItem.image.sprite;
                itemManager.skillOrbList.Add(skill.activeSkillDrop.draggableItem, 1);
                skill.autoToggle.gameObject.SetActive(true);
            }
            else
            {
                skill.autoToggle.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < activeSkillList.Count; i++) //Apply gem mods to active skills
        {
            foreach (ActiveSkillDrop asd in activeSkillList[i].skillGemDropList)
            {
                if (asd.draggableItem != null && activeSkillList[i].skillController != null)
                {
                    gameplayManager.updateStats.ApplyGemUpgrades(asd.draggableItem.gemUpgrades, activeSkillList[i].skillController);
                }
            }
        }
    }
    public void DropInInventory(DraggableItem draggableItem)
    {
        if(draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
        {
            draggableItem.slotUI.inUseText.gameObject.SetActive(false);
            activeSkillList[draggableItem.activeSkillDrop.num].skillController = null;
            draggableItem.activeSkillDrop.draggableItem = null;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = draggableItem.slotUI.fadedImage.transform;   //set new parent
            draggableItem.transform.SetParent(draggableItem.currentParent);
            draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.num + 1).ToString();
            draggableItem.activeSkillDrop = null;
            //Destroy Skill Controller when moving orb to inventory.
            draggableItem.skillController.enabled = false;
            Destroy(draggableItem.skillController.poolParent);
            Destroy(draggableItem.skillController.orbitParent);
            Destroy(draggableItem.skillController.gameObject);
            draggableItem.skillController = null;
        }
        else
        {
            foreach (DraggableItem dItem in itemManager.skillGemList.Keys) //if theres 1+ skill gem in inventory, add to dictionary value
            {
                if (draggableItem.itemName.Equals(dItem.itemName))
                {
                    itemManager.skillGemList[dItem]++;
                    dItem.slotUI.amountText.text = itemManager.skillGemList[dItem].ToString();
                    draggableItem.activeSkillDrop.draggableItem = null;
                    Destroy(draggableItem.gameObject, 0);   //Destroy the dragged newItem.
                    return;
                }
            }
            //Create new slotUI if there isn't one in inventory.
            SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryGemDrop.contentParent.transform);
            draggableItem.slotUI = slotUI;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
            draggableItem.transform.SetParent(draggableItem.currentParent);
            slotUI.name = uiPrefab.name;
            slotUI.nameText.text = draggableItem.itemName;
            slotUI.levelText.text = "Tier " + draggableItem.tier.ToString();
            slotUI.fadedImage.sprite = draggableItem.image.sprite;
            itemManager.skillGemList.Add(draggableItem, 1);
            draggableItem.activeSkillDrop = null;
        }
    }
    public void DropInActiveSkill(DraggableItem draggableItem, Transform parent) //From Inventory to Active Skill slot
    {
        bool moreThanOne = false;
        if (draggableItem.slotType == DraggableItem.SlotType.SkillGem) //if theres 1+ skill gem in inventory, set bool to true.
        {
            foreach (DraggableItem dItem in itemManager.skillGemList.Keys) 
            {
                if (draggableItem.itemName.Equals(dItem.itemName))
                {
                    if (itemManager.skillGemList[dItem] > 1)
                    {
                        moreThanOne = true;
                        break;
                    }
                }
            }
        }
        if (moreThanOne) //if there is more than 1 amount in inventory, spawn new dragItem and put it back in inventory
        {
            foreach (DraggableItem prefab in itemManager.t1GemPrefabList)
            {
                if (draggableItem.itemName.Equals(prefab.itemName))
                {
                    DraggableItem newItem = Instantiate(prefab, draggableItem.slotUI.fadedImage.transform); //Create new item back into inventory
                    newItem.inventory = this;
                    newItem.slotUI = draggableItem.slotUI;
                    newItem.name = draggableItem.name;
                    newItem.itemName = draggableItem.itemName;
                    newItem.image.sprite = draggableItem.image.sprite;
                    newItem.gemUpgrades = draggableItem.gemUpgrades;
                    newItem.slotType = draggableItem.slotType;
                    itemManager.skillGemList[draggableItem]--;
                    itemManager.skillGemList.Add(newItem, itemManager.skillGemList[draggableItem]);
                    itemManager.skillGemList.Remove(draggableItem);
                    if (itemManager.skillGemList[newItem] > 1) newItem.slotUI.amountText.text = itemManager.skillGemList[newItem].ToString();
                    else newItem.slotUI.amountText.text = "";
                    newItem.isInInventory = true;
                    newItem.currentParent = draggableItem.currentParent;   //set new parent
                                                                           //draggableItem gets dropped in active skill slot
                    draggableItem.slotUI = null;
                    draggableItem.isInInventory = false;
                    draggableItem.currentParent = parent;   //set new parent}
                }
            }
        }
        else //only 1 amount in inventory, destroy slotUI
        {
            draggableItem.isInInventory = false;
            draggableItem.currentParent = parent;   //set new parent
            if (draggableItem.slotType == DraggableItem.SlotType.SkillGem)
            {
                itemManager.skillGemList.Remove(draggableItem);
                Destroy(draggableItem.slotUI.gameObject, 0);
                draggableItem.slotUI = null;
            }
            else
            {
                draggableItem.slotUI.inUseText.gameObject.SetActive(true);
            }
        }
        InstantiateSkill(draggableItem);
    }
    public void InstantiateSkill(DraggableItem dragItem) //Add skill to gameplay
    {
        if (dragItem.slotType == DraggableItem.SlotType.SkillOrb)
        {
            foreach (SkillController sc in itemManager.skillControllerPrefabsList) //Look for skill controller name.
            {
                if (sc.skillOrbName.Equals(dragItem.itemName)) //Get skill controller and set to draggableItem
                {
                    SkillController skill = Instantiate(sc, skillParent.transform);
                    skill.name = sc.name;
                    skill.poolParent.transform.SetParent(skillPoolParent.transform);
                    skill.player = player;
                    skill.enemyManager = enemyManager;
                    skill.enemyDistances = enemyDistances;
                    skill.gameplayManager = gameplayManager;
                    if (!itemManager.skillExpDict.ContainsKey(skill.skillOrbName)) //if skill isn't saved to dictionary, add and save it.
                    {
                        itemManager.skillExpDict.Add(skill.skillOrbName, skill.exp);
                        itemManager.skillLevelDict.Add(skill.skillOrbName, skill.level);
                    }
                    else //set skill's exp and level to saved skill's info.
                    {
                        skill.level = itemManager.skillLevelDict[skill.skillOrbName];
                        skill.exp = itemManager.skillExpDict[skill.skillOrbName];
                    }
                    skill.UpdateSkillStats();
                    if (skill.level > 1)
                    {
                        for (int i = 0; i < skill.level - 1; i++)
                        {
                            gameplayManager.updateStats.ApplySkillUpgrades(skill.upgrade, skill, i);
                        }
                    }
                    dragItem.skillController = skill;
                    activeSkillList[dragItem.activeSkillDrop.num].skillController = skill;
                    return;
                }
            }
        }
    }
    public void AddCollectibleIntoInventory(string itemName)
    {
        if (itemName.Contains("Orb"))
        {
            foreach (DraggableItem dItem in itemManager.skillOrbList.Keys)
            {
                if (dItem.itemName.Equals(itemName))
                {
                    itemManager.skillOrbList[dItem]++;
                    dItem.slotUI.amountText.text = (itemManager.skillOrbList[dItem] - 1).ToString();
                    return;
                }
            }
            //Create DraggbleItem for orb.
            foreach (DraggableItem prefab in itemManager.orbPrefabList)
            {
                if (prefab.itemName.Equals(itemName))
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
                    slotUI.nameText.text = draggableItem.itemName;
                    slotUI.levelText.text = "Lv. " + draggableItem.skillController.level.ToString();
                    slotUI.fadedImage.sprite = draggableItem.image.sprite;
                    itemManager.skillOrbList.Add(draggableItem, 1);
                    return;
                }
            }
        }
        else
        {
            foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
            {
                if (dItem.itemName.Equals(itemName))
                {
                    itemManager.skillGemList[dItem]++;
                    dItem.slotUI.amountText.text = itemManager.skillGemList[dItem].ToString();
                    return;
                }
            }
            //Create DraggbleItem for gem.
            foreach (DraggableItem prefab in itemManager.t1GemPrefabList)
            {
                if (prefab.itemName.Equals(itemName))
                {
                    DraggableItem draggableItem = Instantiate(prefab, inventoryGemDrop.contentParent.transform);
                    draggableItem.inventory = this;
                    //Create new slotUI
                    SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryGemDrop.contentParent.transform);
                    draggableItem.slotUI = slotUI;
                    draggableItem.isInInventory = true;
                    draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
                    draggableItem.transform.SetParent(draggableItem.currentParent);
                    slotUI.name = itemName;
                    slotUI.nameText.text = draggableItem.itemName;
                    slotUI.levelText.text = "Tier " + draggableItem.tier.ToString();
                    slotUI.fadedImage.sprite = draggableItem.image.sprite;
                    itemManager.skillGemList.Add(draggableItem, 1);
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
            if (skillSlot.skillController != null)
            {
                if (skillSlot.autoToggle.isOn)
                {
                    skillSlot.skillController.autoUseSkill = true;
                }
                else
                {
                    skillSlot.skillController.autoUseSkill = false;
                }
                skillSlot.skillController.CheckTargetless();
            }
        }
    }
}
