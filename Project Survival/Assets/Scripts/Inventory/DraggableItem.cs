using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillController skillController;
    public ActiveSkillDrop activeSkillDrop;
    public InventoryManager inventory;
    public SkillSlotUI slotUI;
    public Image image;
    public Transform currentParent;
    public bool isInInventory;
    public ItemDescription itemDescription;
    public void OnBeginDrag(PointerEventData eventData)
    {
        currentParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) //Calls after Drop
    {
        transform.SetParent(currentParent);    //Set parent back to where it came from if it didn't move
        image.raycastTarget = true;
    }

}
