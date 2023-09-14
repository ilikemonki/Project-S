using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveSkillDrop : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        SkillOrb,
        SkillGem,
    }
    public SlotType slotType;
    public InventoryManager inventory;
    public DraggableItem draggableItem;
    public TextMeshProUGUI nameText;
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
                    inventory.DropInActiveSkill(draggableItem, transform);
                }
                else //From active slot to active slot
                {
                    draggableItem.activeSkillDrop.nameText.text = "";
                    draggableItem.currentParent = transform;
                }
                draggable.activeSkillDrop = this;
                nameText.text = "Lv. " + draggable.level.ToString() + " " + draggable.itemName;
            }
        }
    }
}
