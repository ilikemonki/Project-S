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
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, asd.frameImage, false);
                            }
                        }
                        if (inventory.activeSkillList[activeSlotNum].skillController.automaticOnly)
                            inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(false);
                        else
                            inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (inventory.activeSkillList[activeSlotNum].skillController != null)
                        {
                            UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, frameImage, false);
                        }
                    }
                }
                else //From active slot to active slot
                {
                    if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
                    {
                        if (draggableItem.activeSkillDrop.activeSlotNum == activeSlotNum) return;
                        //old active slot
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillGemDropList) //Unapply all Gem Modifiers from old skill group
                        {
                            if (asd.draggableItem != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, asd.frameImage, true);
                            }
                        }
                        draggableItem.activeSkillDrop.nameText.text = "Active Skill " + (draggableItem.activeSkillDrop.activeSlotNum + 1).ToString();
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController = null; //null the skillController in skill list
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].autoToggle.isOn = true;
                        inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].autoToggle.gameObject.SetActive(false);
                        inventory.activeSkillUIList[draggableItem.activeSkillDrop.activeSlotNum].RemoveSkill();
                        //new active slot
                        draggableItem.activeSkillDrop = this; // Set New activeskilldrop
                        inventory.activeSkillList[activeSlotNum].skillController = draggableItem.skillController; //Set new skill Controller in skill list
                        inventory.activeSkillList[activeSlotNum].skillController.autoUseSkill = true;
                        inventory.activeSkillList[activeSlotNum].skillController.CheckTargetless();
                        inventory.activeSkillList[activeSlotNum].autoToggle.isOn = true;
                        if (inventory.activeSkillList[activeSlotNum].skillController.automaticOnly)
                            inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(false);
                        else
                            inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(true);
                        foreach (ActiveSkillDrop asd in inventory.activeSkillList[activeSlotNum].skillGemDropList) //Apply Gem Modifiers from new skill group
                        {
                            if (asd.draggableItem != null)
                            {
                                UpdateStats.ApplyGemUpgrades(asd.draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, asd.frameImage, false);
                            }
                        }
                        inventory.activeSkillUIList[activeSlotNum].SetSkill(inventory.activeSkillList[activeSlotNum].skillController);
                    }
                    else //skill gem
                    {
                        if (draggableItem.activeSkillDrop.activeSlotNum != activeSlotNum) //if gem is moved to a new active skill group, apply to new skillController.
                        {
                            if (inventory.activeSkillList[activeSlotNum].skillController != null) //if moved to new slot with a skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, frameImage, true);
                                }
                                //inventory.ApplyGemModifier(draggableItem.skillGem.gemModifierList, num); //apply to new skill controller
                                UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[activeSlotNum].skillController, frameImage, false);
                            }
                            else // Moved to slot with no skill controller
                            {
                                if (inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController != null)
                                {
                                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, frameImage, true);
                                }
                            }
                            draggableItem.activeSkillDrop.draggableItem = null; //set old activeskilldrop's dragItem to null
                            draggableItem.activeSkillDrop.frameImage.color = Color.white;
                            draggableItem.activeSkillDrop = this; // New activeskilldrop
                        }
                        else  //if moved to same skill group, do nothing.
                        {
                            if (draggableItem.activeSkillDrop != this) //if moved within the same skill group
                            {
                                draggableItem.activeSkillDrop.draggableItem = null; //set old activeskilldrop's dragItem to null
                                frameImage.color = draggableItem.activeSkillDrop.frameImage.color;
                                draggableItem.activeSkillDrop.frameImage.color = Color.white;
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
        //Right click to send item to inventory from active skill slots
        if (draggableItem != null && eventData.button == PointerEventData.InputButton.Right && !Input.GetMouseButton(0))
        {
            if (draggableItem.itemDescription.itemType == ItemDescription.ItemType.SkillOrb)
            {
                for(int i = 0; i < inventory.activeSkillList[activeSlotNum].skillGemDropList.Count; i++)
                {
                    inventory.activeSkillList[activeSlotNum].skillGemDropList[i].frameImage.color = Color.white;
                }
                inventory.activeSkillList[activeSlotNum].autoToggle.isOn = true;
                inventory.activeSkillList[activeSlotNum].autoToggle.gameObject.SetActive(false);
                inventory.DropInInventory(draggableItem);
            }
            else // gem
            {
                if (inventory.activeSkillList[activeSlotNum].skillController != null)
                {
                    UpdateStats.ApplyGemUpgrades(draggableItem.itemDescription.upgrade, inventory.activeSkillList[draggableItem.activeSkillDrop.activeSlotNum].skillController, frameImage, true);
                }
                inventory.DropInInventory(draggableItem);
            }
            draggableItem = null;
            ToolTipManager.HideToolTip();
        }
    }
}
