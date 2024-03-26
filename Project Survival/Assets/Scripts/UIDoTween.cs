using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIDoTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 defaultSize;
    RectTransform rect;
    public void Start()
    {
        rect = GetComponent<RectTransform>();
        defaultSize = rect.localScale;
    }
    public void OnEnable()
    {
        if (rect != null)
            rect.localScale = defaultSize;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOScale(new Vector3(defaultSize.x * 1.1f, defaultSize.y * 1.1f, 1), 0.2f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOScale(defaultSize, 0.2f).SetUpdate(true);
    }
}
