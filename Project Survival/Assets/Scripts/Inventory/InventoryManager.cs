using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<SkillController> skillControllerPrefabList = new();
    public List<DraggableItem> skillOrbList = new();
    public List<DraggableItem> skillGemList = new();
    public SkillSlotUI uiPrefab;
    public DraggableItem newItemPrefab;
    public GameObject skillParent;
    public GameObject skillPoolParent;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public GameplayManager gameplayManager;
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
            foreach (SkillController sc in skillControllerPrefabList) //Add skill controller into game.
            {
                if (sc.name.Contains(draggableItem.name)) //Name on drag item and controller must contain same name
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
    }
    public void ReturnButton()
    {
        gameObject.SetActive(false);
    }
}
