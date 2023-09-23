using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveSkillDrop : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum SlotType
    {
        SkillOrb,
        SkillGem,
    }
    public SlotType slotType;
    public int num;
    public InventoryManager inventory;
    public DraggableItem draggableItem;
    public TextMeshProUGUI nameText;
    void Awake()
    {
        if (nameText != null)
            nameText.text = "Active Skill " + (num + 1).ToString();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggable = dropped.GetComponent<DraggableItem>();
            draggableItem = draggable;
            if (draggableItem.isInInventory) //Moving from inventory to active skill
            {
                draggableItem.activeSkillDrop = this; // New activeskilldrop
                inventory.DropInActiveSkill(draggableItem, transform);

                if (draggableItem.slotType == DraggableItem.SlotType.SkillGem)
                {
                    if (inventory.skillList[num].skillController != null)
                    {
                        inventory.ApplyGemModifier(draggableItem.skillGem.gemModifierList, num);
                    }
                }
            }
            else //From active slot to active slot
            {
                if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
                {
                    inventory.skillList[draggableItem.activeSkillDrop.num].skillController = null; //null the old activeskilldrop
                    draggableItem.activeSkillDrop = this; // New activeskilldrop
                    inventory.skillList[draggableItem.activeSkillDrop.num].skillController = draggableItem.skillController;
                }
                if (draggableItem.activeSkillDrop.nameText != null) draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.num + 1).ToString();
                draggableItem.currentParent = transform;
            }
            if (nameText != null) nameText.text = "Lv. " + draggableItem.level.ToString() + " " + draggableItem.itemName;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggableItem != null && eventData.clickCount == 2)
        {
            GameManager.DebugLog("Remove " + draggableItem.itemName);
            if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
            {
                inventory.DropInInventory(draggableItem);
                inventory.skillList[num].skillController = null;
            }
            else
            {
                inventory.DropInInventory(draggableItem);
            }
            draggableItem = null; 
        }
    }
}
