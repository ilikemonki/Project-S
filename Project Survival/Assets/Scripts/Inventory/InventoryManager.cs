using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<SkillController> skillControllerPrefabList = new();
    public List<DraggableItem> skillOrbList = new();
    public List<DraggableItem> skillGemList = new();
    public List<TextMeshProUGUI> generalStats = new(); //List must be in order of the GameManager.totalIntStats and totalFloatStats list.
    public List<ActiveSkillDrop> activeSkillDropList;
    public SkillSlotUI uiPrefab;
    public DraggableItem newItemPrefab;
    public GameObject skillParent;
    public GameObject skillPoolParent;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public GameplayManager gameplayManager;
    private void Start()
    {
        foreach(ActiveSkillDrop skillDrop in activeSkillDropList) //Set skills in start.
        {
            if (skillDrop.draggableItem != null)
            {
                InstantiateSkill(skillDrop.draggableItem);
            }
        }
    }
    public void DropInInventory(DraggableItem draggableItem, Transform parent)
    {
        if(draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
        {
            foreach (DraggableItem dItem in skillOrbList)  //if there are already an newItem in inventory, add to that instead.
            {
                if (dItem.name.Equals(draggableItem.name))
                {
                    dItem.amount++;
                    dItem.slotUI.amountText.text = dItem.amount.ToString();
                    Destroy(draggableItem.gameObject, 0);   //Destroy the dragged newItem.
                    return;
                }
            }
            //Create new slotUI if there isn't one in inventory.
            SkillSlotUI slotUI = Instantiate(uiPrefab, parent);
            draggableItem.slotUI = slotUI;
            draggableItem.amount = 1;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
            slotUI.nameText.text = draggableItem.name;
            slotUI.fadedImage.sprite = draggableItem.image.sprite;
            skillOrbList.Add(draggableItem);
        }
        else
        {
            foreach (DraggableItem dItem in skillGemList)  //if there are already an newItem in inventory, add to that instead.
            {
                if (dItem.name.Equals(draggableItem.name))
                {
                    dItem.amount++;
                    dItem.slotUI.amountText.text = dItem.amount.ToString();
                    Destroy(draggableItem.gameObject, 0);   //Destroy the dragged newItem.
                    return;
                }
            }
            //Create new slotUI if there isn't one in inventory.
            SkillSlotUI slotUI = Instantiate(uiPrefab, parent);
            draggableItem.slotUI = slotUI;
            draggableItem.amount = 1;
            draggableItem.isInInventory = true;
            draggableItem.currentParent = slotUI.fadedImage.transform;   //set new parent
            slotUI.nameText.text = draggableItem.name;
            slotUI.fadedImage.sprite = draggableItem.image.sprite;
            skillGemList.Add(draggableItem);
        }
    }
    public void DropInActiveSkill(DraggableItem draggableItem, Transform parent)
    {
        if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
        {
            if (draggableItem.amount > 1) //if there is more than 1 amount in inventory
            {
                DraggableItem newItem = Instantiate(newItemPrefab, draggableItem.slotUI.fadedImage.transform); //Create new item back into inventory
                newItem.inventory = this;
                newItem.slotUI = draggableItem.slotUI;
                newItem.amount = draggableItem.amount - 1;
                newItem.name = draggableItem.name;
                newItem.image.sprite = draggableItem.image.sprite;
                skillOrbList.Remove(draggableItem);
                skillOrbList.Add(newItem);
                if (newItem.amount > 1) newItem.slotUI.amountText.text = newItem.amount.ToString();
                else newItem.slotUI.amountText.text = "";
                newItem.isInInventory = true;
                newItem.currentParent = draggableItem.currentParent;   //set new parent
                draggableItem.slotUI = null;
                draggableItem.amount = 1;
                draggableItem.isInInventory = false;
                draggableItem.currentParent = parent;   //set new parent
            }
            else
            {
                skillOrbList.Remove(draggableItem);
                draggableItem.amount = 1;
                draggableItem.isInInventory = false;
                draggableItem.currentParent = parent;   //set new parent
                Destroy(draggableItem.slotUI.gameObject, 0);
                draggableItem.slotUI = null;
            }
            InstantiateSkill(draggableItem);
        }
    }
    public void InstantiateSkill(DraggableItem dragItem)
    {
        foreach (SkillController sc in skillControllerPrefabList) //Add skill controller into game.
        {
            if (sc.name.Contains(dragItem.name)) //Name on drag item and controller must contain same name
            {
                SkillController skill = Instantiate(sc, skillParent.transform);
                skill.player = player;
                skill.enemyManager = enemyManager;
                skill.enemyDistances = enemyDistances;
                skill.gameplayManager = gameplayManager;
                skill.poolParent.transform.SetParent(skillPoolParent.transform);
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
            }
        }
    }
    public void OnEnable()
    {
        UpdateGeneralStats();
    }
}
