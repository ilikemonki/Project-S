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
            if ((draggableItem.slotType == DraggableItem.SlotType.SkillOrb && slotType == SlotType.SkillOrb) || (draggableItem.slotType == DraggableItem.SlotType.SkillGem && slotType == SlotType.SkillGem))
            {
                if (draggableItem.isInInventory) //Moving from inventory to active skill
                {
                    draggable.activeSkillDrop = this;
                    inventory.DropInActiveSkill(draggableItem, transform);
                }
                else //From active slot to active slot
                {
                    inventory.skillList[draggable.activeSkillDrop.num].skillController = null; //old activeskilldrop
                    if (draggableItem.activeSkillDrop.nameText != null) draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.num + 1).ToString();
                    draggableItem.currentParent = transform;
                    draggable.activeSkillDrop = this; // New activeskilldrop
                    inventory.skillList[draggable.activeSkillDrop.num].skillController = draggable.skillController;
                }
                if (nameText != null) nameText.text = "Lv. " + draggable.level.ToString() + " " + draggable.itemName;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggableItem != null && eventData.clickCount == 2)
        {
            Debug.Log("Remove " + draggableItem.itemName);
            if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
            {
                inventory.DropInInventory(draggableItem);
            }
            else
            {
                inventory.DropInInventory(draggableItem);
            }
            draggableItem = null;
            inventory.skillList[num].skillController = null;
        }
    }
}
