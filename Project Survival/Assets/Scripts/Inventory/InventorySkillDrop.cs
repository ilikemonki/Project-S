using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySkillDrop : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        SkillOrb,
        SkillGem,
    }
    public SlotType slotType;
    public InventoryManager inventory;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
        if ((draggableItem.slotType == DraggableItem.SlotType.SkillOrb && slotType == SlotType.SkillOrb) || (draggableItem.slotType == DraggableItem.SlotType.SkillGem && slotType == SlotType.SkillGem)) //Make sure the item is dropped in the right inventory.
        {
            if (draggableItem.isInInventory) return; //If dropped from inventory to inventory, do nothing.

            if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
            {
                inventory.DropInInventory(draggableItem);
                inventory.skillList[draggableItem.activeSkillDrop.num].skillController = null;
            }
            else
            {
                if (inventory.skillList[draggableItem.activeSkillDrop.num].skillController != null)
                {
                    inventory.UnapplyGemModifier(draggableItem.skillGem.gemModifierList, draggableItem.activeSkillDrop.num); //unapply to old skill controller
                }
                inventory.DropInInventory(draggableItem);
            }
        }
    }
}
