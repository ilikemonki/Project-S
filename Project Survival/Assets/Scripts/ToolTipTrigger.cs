using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemDescription itemDesc;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemDesc != null)
            ToolTipManager.ShowItemToolTip(itemDesc);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemDesc != null)
            ToolTipManager.HideToolTip();
    }
}
