using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActiveSkillDrop : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum SlotType
    {
        SkillOrb,
        SkillGem,
    }
    public SlotType slotType;
    public int activeSlotNum;
    public InventoryManager inventory;
    public DraggableItem draggableItem;
    public TextMeshProUGUI nameText;
    public Image frameImage;
    void Awake()
    {
        if (nameText != null && draggableItem == null)
            nameText.text = "Active Skill " + (activeSlotNum + 1).ToString();
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
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[activeSlotNum].skillGemDropList) //Apply all Gem Modifiers from new skill group
                        {
                            if (asd.draggableItem != null && inventory.activeSkillList[activeSlotNum].skillController != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, false);
                            }
                        }
                        inventory.activeSkillList[activeSlotNum].autoToggle.isOn = true;
                        inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (inventory.activeSkillList[activeSlotNum].skillController != null)
                        {
                            UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, false);
                        }
                    }
                }
                else //From active slot to active slot
                {
                    if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
                    {
                        if (draggableItem.activeSkillDrop.activeSlotNum == activeSlotNum) return;
                        //old active slot
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController.UpdateSkillStats(); //Reset stats
                        draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.activeSlotNum + 1).ToString();
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController = null; //null the skillController in skill list
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].autoToggle.isOn = true;
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].autoToggle.gameObject.SetActive(false);
                        //new active slot
                        draggableItem.activeSkillDrop = this; // Set New activeskilldrop
                        inventory.activeSkillList[activeSlotNum].skillController = draggableItem.skillController; //Set new skill Controller in skill list
                        inventory.activeSkillList[activeSlotNum].skillController.autoUseSkill = true;
                        inventory.activeSkillList[activeSlotNum].skillController.CheckTargetless();
                        inventory.activeSkillList[activeSlotNum].autoToggle.isOn = true;
                        inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(true);
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[activeSlotNum].skillGemDropList) //Apply Gem Modifiers from new skill group
                        {
                            if (asd.draggableItem != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, false);
                            }
                        }
                    }
                    else //skill gem
                    {
                        if (draggableItem.activeSkillDrop.activeSlotNum != activeSlotNum) //if gem is moved to a new active skill group, apply to new skillController.
                        {
                            if (inventory.activeSkillList[activeSlotNum].skillController != null) //if moved to new slot with a skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, true);
                                }
                                //inventory.ApplyGemModifier(draggableItem.skillGem.gemModifierList, num); //apply to new skill controller
                                UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, false);
                            }
                            else // Moved to slot with no skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, true);
                                }
                            }
                            draggableItem.activeSkillDrop.draggableItem = null; //set old activeskilldrop's dragItem to null
                            draggableItem.activeSkillDrop = this; // New activeskilldrop
                        }
                        else  //if moved to same skill group, do nothing.
                        {
                            if (draggableItem.activeSkillDrop != this) //if moved within the same skill group
                            {
                                draggableItem.activeSkillDrop.draggableItem = null; //set old activeskilldrop's dragItem to null
                                draggableItem.activeSkillDrop = this; // New activeskilldrop
                            }
                        }
                    }
                    draggableItem.currentParent = transform;
                }
                if (nameText != null) nameText.text = "Lv. " + draggableItem.skillController.level.ToString() + " " + draggableItem.itemDescription.itemName;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggableItem != null && eventData.button == PointerEventData.InputButton.Right && !Input.GetMouseButton(0))
        {
            if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
            {
                inventory.DropInInventory(draggableItem);
            }
            else
            {
                if (inventory.activeSkillList[activeSlotNum].skillController != null)
                {
                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, true);
                }
                inventory.DropInInventory(draggableItem);
            }
            draggableItem = null;
            ToolTipManager.HideToolTip();
        }
    }
}
