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
        nameText.text = "Active Skill " + num.ToString();
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
                    inventory.DropInActiveSkill(draggableItem, transform);
                }
                else //From active slot to active slot
                {
                    draggableItem.activeSkillDrop.nameText.text = "Active Skill " + draggableItem.activeSkillDrop.num.ToString();
                    draggableItem.currentParent = transform;
                }
                draggable.activeSkillDrop = this;
                nameText.text = "Lv. " + draggable.level.ToString() + " " + draggable.itemName;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggableItem != null)
        {
            inventory.xButton.transform.SetParent(transform.parent);
            inventory.xButtonOnDrop = this;
            RectTransform rt = GetComponent<RectTransform>();
            inventory.xButton.transform.localPosition = new Vector3(rt.sizeDelta.x / 2, -rt.sizeDelta.x / 2, 0);
            inventory.xButton.gameObject.SetActive(true);
        }
    }
}
