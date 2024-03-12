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
            if (dropped.TryGetComponent(out DraggableItem draggable))
            {
                if (!draggable.itemDescription.itemType.ToString().Equals(slotType.ToString())) return; //if dragItem is not the same slotType, return;
                draggableItem = draggable;
                if (draggableItem.isInInventory) //Moving from inventory to active skill
                {
                    draggableItem.activeSkillDrop = this; // New activeskilldrop
                    inventory.DropInActiveSkill(draggableItem, transform);

                    if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
                    {
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[num].skillGemDropList) //Apply all Gem Modifiers from new skill group
                        {
                            if (asd.draggableItem != null && inventory.activeSkillList[num].skillController != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[num].skillController, false);
                            }
                        }
                        inventory.activeSkillList[num].autoToggle.isOn = true;
                        inventory.activeSkillList[num].autoToggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (inventory.activeSkillList[num].skillController != null)
                        {
                            UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[num].skillController, false);
                        }
                    }
                }
                else //From active slot to active slot
                {
                    if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
                    {
                        if (draggableItem.activeSkillDrop.num == num) return;
                        //old active slot
                        inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController.UpdateSkillStats(); //Reset stats
                        draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.num + 1).ToString();
                        inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController = null; //null the skillController in skill list
                        inventory.activeSkillList[draggableItem.activeSkillDrop.num].autoToggle.isOn = true;
                        inventory.activeSkillList[draggableItem.activeSkillDrop.num].autoToggle.gameObject.SetActive(false);
                        //new active slot
                        draggableItem.activeSkillDrop = this; // Set New activeskilldrop
                        inventory.activeSkillList[num].skillController = draggableItem.skillController; //Set new skill Controller in skill list
                        inventory.activeSkillList[num].skillController.autoUseSkill = true;
                        inventory.activeSkillList[num].skillController.CheckTargetless();
                        inventory.activeSkillList[num].autoToggle.isOn = true;
                        inventory.activeSkillList[num].autoToggle.gameObject.SetActive(true);
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[num].skillGemDropList) //Apply Gem Modifiers from new skill group
                        {
                            if (asd.draggableItem != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[num].skillController, false);
                            }
                        }
                    }
                    else //skill gem
                    {
                        if (draggableItem.activeSkillDrop.num != num) //if gem is moved to a new active skill group, apply to new skillController.
                        {
                            if (inventory.activeSkillList[num].skillController != null) //if moved to new slot with a skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController, true);
                                }
                                //inventory.ApplyGemModifier(draggableItem.skillGem.gemModifierList, num); //apply to new skill controller
                                UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[num].skillController, false);
                            }
                            else // Moved to slot with no skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController, true);
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
                if (nameText != null) nameText.text = "Lv. " + draggableItem.skillController.level.ToString() + " " + draggableItem.itemDescription.itemName;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggableItem != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
            {
                inventory.DropInInventory(draggableItem);
            }
            else
            {
                if (inventory.activeSkillList[num].skillController != null)
                {
                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.num].skillController, true);
                }
                inventory.DropInInventory(draggableItem);
            }
            draggableItem = null; 
        }
    }
}
