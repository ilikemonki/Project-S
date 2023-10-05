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
        public List<ActiveSkillDrop> skillGemDropList;
    }
    public List<Skill> skillList = new();
    public List<SkillController> skillControllerPrefabList = new();
    public Dictionary<DraggableItem, int> skillOrbList = new();
    public Dictionary<DraggableItem, int> skillGemList = new();
    public List<DraggableItem> orbPrefabList;
    public List<DraggableItem> t1GemPrefabList, t2GemPrefabList, t3GemPrefabList;
    public List<TextMeshProUGUI> generalStats = new(); //Statistics. List must be in order of the GameManager.totalIntStats and totalFloatStats list.
    public SkillSlotUI uiPrefab;
    public GameObject skillParent;
    public GameObject skillPoolParent;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public GameplayManager gameplayManager;
    public InventorySkillDrop inventoryOrbDrop, inventoryGemDrop;
    private void Start()
    {
        foreach(Skill skill in skillList) //Set skills in start.
        {
            if (skill.activeSkillDrop.draggableItem != null)
            {
                InstantiateSkill(skill.activeSkillDrop.draggableItem);
                skill.activeSkillDrop.nameText.text = "Lv. " + skill.activeSkillDrop.draggableItem.level.ToString() + " " + skill.activeSkillDrop.draggableItem.itemName;
                SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryOrbDrop.transform);
                skill.activeSkillDrop.draggableItem.slotUI = slotUI;
                slotUI.inUseText.gameObject.SetActive(true);
                slotUI.name = uiPrefab.name;
                slotUI.nameText.text = skill.activeSkillDrop.draggableItem.itemName;
                slotUI.levelText.text = "Lv. " + skill.activeSkillDrop.draggableItem.level.ToString();
                slotUI.fadedImage.sprite = skill.activeSkillDrop.draggableItem.image.sprite;
                skillOrbList.Add(skill.activeSkillDrop.draggableItem, 1);
            }
        }
        for (int i = 0; i < skillList.Count; i++) //Apply gem mods to active skills
        {
            foreach (ActiveSkillDrop asd in skillList[i].skillGemDropList)
            {
                if (asd.draggableItem != null && skillList[i].skillController != null)
                {
                    ApplyGemModifier(asd.draggableItem.skillGem.gemModifierList, i);
                }
            }
        }
    }
    public void DropInInventory(DraggableItem draggableItem)
    {
        if(draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
        {
            //draggableItem.slotUI.amountText.text = skillOrbList[draggableItem].ToString();
            draggableItem.slotUI.inUseText.gameObject.SetActive(false);
            skillList[draggableItem.activeSkillDrop.num].skillController = null;
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
            foreach (DraggableItem dItem in skillGemList.Keys) //if theres 1+ skill gem in inventory, add to dictionary value
            {
                if (draggableItem.itemName.Equals(dItem.itemName))
                {
                    skillGemList[dItem]++;
                    dItem.slotUI.amountText.text = skillGemList[dItem].ToString();
                    draggableItem.activeSkillDrop.draggableItem = null;
                    Destroy(draggableItem.gameObject, 0);   //Destroy the dragged newItem.
                    return;
                }
            }
            //Create new slotUI if there isn't one in inventory.
            SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryGemDrop.transform);
            draggableItem.slotUI = slotUI;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
            draggableItem.transform.SetParent(draggableItem.currentParent);
            slotUI.name = uiPrefab.name;
            slotUI.nameText.text = draggableItem.itemName;
            slotUI.levelText.text = "Tier " + draggableItem.level.ToString();
            slotUI.fadedImage.sprite = draggableItem.image.sprite;
            skillGemList.Add(draggableItem, 1);
            draggableItem.activeSkillDrop = null;
        }
    }
    public void DropInActiveSkill(DraggableItem draggableItem, Transform parent) //From Inventory to Active Skill slot
    {
        bool moreThanOne = false;
        if (draggableItem.slotType == DraggableItem.SlotType.SkillGem) //if theres 1+ skill gem in inventory, set bool to true.
        {
            foreach (DraggableItem dItem in skillGemList.Keys) 
            {
                if (draggableItem.itemName.Equals(dItem.itemName))
                {
                    if (skillGemList[dItem] > 1)
                    {
                        moreThanOne = true;
                        break;
                    }
                }
            }
        }
        if (moreThanOne) //if there is more than 1 amount in inventory, spawn new dragItem and put it back in inventory
        {
            foreach (DraggableItem prefab in t1GemPrefabList)
            {
                if (draggableItem.itemName.Equals(prefab.itemName))
                {
                    DraggableItem newItem = Instantiate(prefab, draggableItem.slotUI.fadedImage.transform); //Create new item back into inventory
                    newItem.inventory = this;
                    newItem.slotUI = draggableItem.slotUI;
                    newItem.name = draggableItem.name;
                    newItem.itemName = draggableItem.itemName;
                    newItem.image.sprite = draggableItem.image.sprite;
                    newItem.skillGem = draggableItem.skillGem;
                    newItem.slotType = draggableItem.slotType;
                    skillGemList[draggableItem]--;
                    skillGemList.Add(newItem, skillGemList[draggableItem]);
                    skillGemList.Remove(draggableItem);
                    if (skillGemList[newItem] > 1) newItem.slotUI.amountText.text = skillGemList[newItem].ToString();
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
                skillGemList.Remove(draggableItem);
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
            foreach (SkillController sc in skillControllerPrefabList) //Look for skill controller name.
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
                    skill.UpdateSkillStats();
                    skill.UpdateSize();
                    dragItem.skillController = skill;
                    skillList[dragItem.activeSkillDrop.num].skillController = skill;
                }
            }
        }
    }
    public void AddCollectibleIntoInventory(string itemName)
    {
        if (itemName.Contains("Orb"))
        {
            foreach (DraggableItem dItem in skillOrbList.Keys)
            {
                if (dItem.itemName.Equals(itemName))
                {
                    skillOrbList[dItem]++;
                    dItem.slotUI.amountText.text = (skillOrbList[dItem] - 1).ToString();
                    return;
                }
            }
            //Create DraggbleItem for orb.
            foreach (DraggableItem prefab in orbPrefabList)
            {
                if (prefab.itemName.Equals(itemName))
                {
                    DraggableItem draggableItem = Instantiate(prefab, inventoryOrbDrop.transform);
                    draggableItem.inventory = this;
                    //Create new slotUI
                    SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryOrbDrop.transform);
                    draggableItem.slotUI = slotUI;
                    draggableItem.isInInventory = true;
                    draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
                    draggableItem.transform.SetParent(draggableItem.currentParent);
                    slotUI.name = itemName;
                    slotUI.nameText.text = draggableItem.itemName;
                    slotUI.levelText.text = "Lv. " + draggableItem.level.ToString();
                    slotUI.fadedImage.sprite = draggableItem.image.sprite;
                    skillOrbList.Add(draggableItem, 1);
                    return;
                }
            }
        }
        else
        {
            foreach (DraggableItem dItem in skillGemList.Keys)
            {
                if (dItem.itemName.Equals(itemName))
                {
                    skillGemList[dItem]++;
                    dItem.slotUI.amountText.text = skillGemList[dItem].ToString();
                    return;
                }
            }
            //Create DraggbleItem for gem.
            foreach (DraggableItem prefab in t1GemPrefabList)
            {
                if (prefab.itemName.Equals(itemName))
                {
                    DraggableItem draggableItem = Instantiate(prefab, inventoryGemDrop.transform);
                    draggableItem.inventory = this;
                    //Create new slotUI
                    SkillSlotUI slotUI = Instantiate(uiPrefab, inventoryGemDrop.transform);
                    draggableItem.slotUI = slotUI;
                    draggableItem.isInInventory = true;
                    draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
                    draggableItem.transform.SetParent(draggableItem.currentParent);
                    slotUI.name = itemName;
                    slotUI.nameText.text = draggableItem.itemName;
                    slotUI.levelText.text = "Lv. " + draggableItem.level.ToString();
                    slotUI.fadedImage.sprite = draggableItem.image.sprite;
                    skillGemList.Add(draggableItem, 1);
                    return;
                }
            }
        }
    }
    public void UpdateGeneralStats()
    {
        for (int i = 0; i < generalStats.Count; i++)
        {
            switch (i)
            {
                case 0: generalStats[i].text = GameManager.totalTime.ToString(); break;
                case 1: generalStats[i].text = GameManager.totalKills.ToString(); break;
                case 2: generalStats[i].text = GameManager.totalExp.ToString(); break;
                case 3: generalStats[i].text = gameplayManager.level.ToString(); break;
                case 4: generalStats[i].text = (gameplayManager.waveCounter + 1).ToString(); break;
                case 5: generalStats[i].text = GameManager.totalCoinsCollected.ToString(); break;
                case 6: generalStats[i].text = GameManager.totalClassStarsCollected.ToString(); break;
                case 7: generalStats[i].text = GameManager.totalPassiveItems.ToString(); break;
                case 8: generalStats[i].text = GameManager.totalSkillGemsCollected.ToString(); break;
                case 9: generalStats[i].text = GameManager.totalDashes.ToString(); break;
                case 10: generalStats[i].text = GameManager.totalDamageDealt.ToString(); break;
                case 11: generalStats[i].text = GameManager.totalPhysicalDamage.ToString(); break;
                case 12: generalStats[i].text = GameManager.totalFireDamage.ToString(); break;
                case 13: generalStats[i].text = GameManager.totalColdDamage.ToString(); break;
                case 14: generalStats[i].text = GameManager.totalLightningDamage.ToString(); break;
                case 15: generalStats[i].text = GameManager.TotalDotDamage.ToString(); break;
                case 16: generalStats[i].text = GameManager.totalBleedDamage.ToString(); break;
                case 17: generalStats[i].text = GameManager.totalBurnDamage.ToString(); break;
                case 18: generalStats[i].text = GameManager.totalCrits.ToString(); break;
                case 19: generalStats[i].text = GameManager.totalSkillsUsed.ToString(); break;
                case 20: generalStats[i].text = GameManager.totalDamageTaken.ToString(); break;
                case 21: generalStats[i].text = GameManager.totalHealing.ToString(); break;
                case 22: generalStats[i].text = GameManager.totalRegen.ToString(); break;
                case 23: generalStats[i].text = GameManager.totalDegen.ToString(); break;
                case 24: generalStats[i].text = GameManager.totalLifeStealProc.ToString(); break;
                case 25: generalStats[i].text = GameManager.totalLifeSteal.ToString(); break;
                case 26: generalStats[i].text = GameManager.totalBleed.ToString(); break;
                case 27: generalStats[i].text = GameManager.totalBurn.ToString(); break;
                case 28: generalStats[i].text = GameManager.totalChill.ToString(); break;
                case 29: generalStats[i].text = GameManager.totalShock.ToString(); break;
                default: GameManager.DebugLog("General Stats has no switch case for " + i); break;
            }
        }
    }
    public void ApplyGemModifier(List<SkillGem.GemModifier> modList, int skillIndex)
    {
        for (int i = 0; i < modList.Count; i++)
        {
            switch (modList[i].modifier)
            {
                case SkillGem.GemModifier.Modifier.Damage: skillList[skillIndex].skillController.damage += modList[i].amt; break;
                case SkillGem.GemModifier.Modifier.Projectile: skillList[skillIndex].skillController.projectile += ((int)modList[i].amt); break;
                default: GameManager.DebugLog("ApplyGemMod has no switch case for " + modList[i].modifier); break;
            }
            GameManager.DebugLog("apply mod: " + modList[i].modifier + " " + modList[i].amt);
        }
    }
    public void UnapplyGemModifier(List<SkillGem.GemModifier> modList, int skillIndex)
    {
        for (int i = 0; i < modList.Count; i++)
        {
            switch (modList[i].modifier)
            {
                case SkillGem.GemModifier.Modifier.Damage: skillList[skillIndex].skillController.damage -= modList[i].amt; break;
                case SkillGem.GemModifier.Modifier.Projectile: skillList[skillIndex].skillController.projectile -= ((int)modList[i].amt); break;
                default: GameManager.DebugLog("UnapplyGemMod has no switch case for " + modList[i].modifier); break;
            }
            GameManager.DebugLog("Unapply mod: " + modList[i].modifier + " " + modList[i].amt);
        }
    }

    public void OnEnable()
    {
        UpdateGeneralStats();
    }
}
