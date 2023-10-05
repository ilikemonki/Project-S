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
        if (nameText != null && draggableItem == null)
            nameText.text = "Active Skill " + (num + 1).ToString();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggable = dropped.GetComponent<DraggableItem>();
            if (!draggable.slotType.ToString().Equals(slotType.ToString())) return; //if dragItem is not the same slotType, return;
            draggableItem = draggable;
            if (draggableItem.isInInventory) //Moving from inventory to active skill
            {
                draggableItem.activeSkillDrop = this; // New activeskilldrop
                inventory.DropInActiveSkill(draggableItem, transform);

                if (draggableItem.slotType == DraggableItem.SlotType.SkillOrb)
                {
                    foreach (ActiveSkillDrop asd in inventory.skillList[num].skillGemDropList) //Apply Gem Modifiers from new skill group
                    {
                        if (asd.draggableItem != null && inventory.skillList[num].skillController != null)
                        {
                            inventory.ApplyGemModifier(asd.draggableItem.skillGem.gemModifierList, num);
                        }
                    }
                }
                else
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
                    if (draggableItem.activeSkillDrop.num == num) return;
                    inventory.skillList[draggableItem.activeSkillDrop.num].skillController.UpdateSkillStats(); //Reset stats
                    draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.num + 1).ToString();
                    inventory.skillList[draggableItem.activeSkillDrop.num].skillController = null; //null the skillController in skill list
                    draggableItem.activeSkillDrop = this; // Set New activeskilldrop
                    inventory.skillList[num].skillController = draggableItem.skillController; //Set new skill Controller in skill list
                    foreach (ActiveSkillDrop asd in inventory.skillList[num].skillGemDropList) //Apply Gem Modifiers from new skill group
                    {
                        if (asd.draggableItem != null)
                        {
                            inventory.ApplyGemModifier(asd.draggableItem.skillGem.gemModifierList, num);
                        }
                    }
                }
                else //skill gem
                {
                    if (draggableItem.activeSkillDrop.num != num) //if gem is moved to a new active skill group, apply to new skillController.
                    {
                        if (inventory.skillList[num].skillController != null) //if moved to new slot with a skill controller
                        {
                            if (inventory.skillList[draggableItem.activeSkillDrop.num].skillController != null)
                            {
                                inventory.UnapplyGemModifier(draggableItem.skillGem.gemModifierList, draggableItem.activeSkillDrop.num); //unapply to old skill controller
                            }
                            inventory.ApplyGemModifier(draggableItem.skillGem.gemModifierList, num); //apply to new skill controller
                        }
                        else // Moved to slot with no skill controller
                        {
                            if (inventory.skillList[draggableItem.activeSkillDrop.num].skillController != null)
                            {
                                inventory.UnapplyGemModifier(draggableItem.skillGem.gemModifierList, draggableItem.activeSkillDrop.num); //unapply to old skill controller
                            }
                        }
                    }
                    else //if moved to same skill group, do nothing.
                    {

                    }
                    draggableItem.activeSkillDrop.draggableItem = null; //set old activeskilldrop's dragItem to null
                    draggableItem.activeSkillDrop = this; // New activeskilldrop
                }
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
            }
            else
            {
                if (inventory.skillList[draggableItem.activeSkillDrop.num].skillController != null)
                {
                    inventory.UnapplyGemModifier(draggableItem.skillGem.gemModifierList, draggableItem.activeSkillDrop.num); //unapply to old skill controller
                }
                inventory.DropInInventory(draggableItem); 
            }
            draggableItem = null; 
        }
    }
}
