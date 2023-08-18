using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
                else
                {
                    draggableItem.currentParent = transform;
                }
            }
        }
    }
}
