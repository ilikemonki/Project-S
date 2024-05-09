using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//When dropped into Inventory.
public class InventorySkillDrop : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        SkillOrb,
        SkillGem,
    }
    public SlotType slotType;
    public InventoryManager inventory;
    public GameObject contentParent;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped.TryGetComponent(out DraggableItem draggableItem))
        {
            if ((draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb && slotType == SlotType.SkillOrb) || (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillGem && slotType == SlotType.SkillGem)) //Make sure the item is dropped in the right inventory.
            {
                if (draggableItem.isInInventory) return; //If dropped from inventory to inventory, do nothing.

                if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
                {
                    for (int i = 0; i < inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillGemDropList.Count; i++)
                    {
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillGemDropList[i].frameImage.color = Color.white;
                    }
                    inventory.DropInInventory(draggableItem);
                }
                else //gem
                {
                    if (inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController != null)
                    {
                        UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, draggableItem.activeSkillDrop.frameImage, true);
                    }
                    inventory.DropInInventory(draggableItem);
                }
            }
        }
    }
}
