using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingTextController current;
    public GameObject prefab;
    public List<TextMeshProUGUI> floatingTextList;
    public float textSize, textCritSize;
    public float rand;
    public int maxFloatingText;

    private void Start()
    {
        current = this;
        DOTween.SetTweensCapacity(400, 50);
        PopulatePool(50);
    }

    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject g = Instantiate(prefab, transform);    //Spawn, add to list, and initialize prefabs
            TextMeshProUGUI dText = g.GetComponent<TextMeshProUGUI>();
            dText.gameObject.SetActive(false);
            current.floatingTextList.Add(dText);
        }
    }

    public static void DisplayFloatingText(Transform transform, string text)
    {
        for (int i = 0; i < current.floatingTextList.Count; i++)
        {
            if (i >= current.maxFloatingText) return;
            if (!current.floatingTextList[i].isActiveAndEnabled)
            {
                if (i > current.floatingTextList.Count - 2)
                {
                    current.PopulatePool(10);
                }
                current.floatingTextList[i].fontSize = current.textSize;
                current.floatingTextList[i].text = text;
                current.floatingTextList[i].color = Color.white;
                current.floatingTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                current.floatingTextList[i].gameObject.SetActive(true);
                current.floatingTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.8f, 1.2f), 0.6f).OnComplete(() => current.floatingTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public static void DisplayFloatingCritText(Transform transform, string text)
    {
        for (int i = 0; i < current.floatingTextList.Count; i++)
        {
            if (i >= current.maxFloatingText) return;
            if (!current.floatingTextList[i].isActiveAndEnabled)
            {
                if (i > current.floatingTextList.Count - 2)
                {
                    current.PopulatePool(10);
                }
                current.floatingTextList[i].fontSize = current.textCritSize;
                current.floatingTextList[i].text = text;
                current.floatingTextList[i].color = Color.yellow;
                current.floatingTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                current.floatingTextList[i].gameObject.SetActive(true);
                current.floatingTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.8f, 1.2f), 0.6f).OnComplete(() => current.floatingTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public static void DisplayDoTText(Transform transform, string text, Color color)
    {
        for (int i = 0; i < current.floatingTextList.Count; i++)
        {
            if (i >= current.maxFloatingText) return;
            if (!current.floatingTextList[i].isActiveAndEnabled)
            {
                if (i > current.floatingTextList.Count - 2)
                {
                    current.PopulatePool(10);
                }
                current.floatingTextList[i].fontSize = current.textSize - 5;
                current.floatingTextList[i].text = text;
                current.floatingTextList[i].color = color;
                current.floatingTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                current.floatingTextList[i].gameObject.SetActive(true);
                current.floatingTextList[i].transform.DOJump(transform.position + new Vector3(0,0.3f,0), 0.3f, 1, 0.3f).SetEase(Ease.InFlash).OnComplete(() => current.floatingTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public static void DisplayPlayerText(Transform transform, string text, Color color, float displayTime)
    {
        for (int i = 0; i < current.floatingTextList.Count; i++)
        {
            if (!current.floatingTextList[i].isActiveAndEnabled)
            {
                if (i > current.floatingTextList.Count - 2)
                {
                    current.PopulatePool(10);
                }
                current.floatingTextList[i].fontSize = current.textSize;
                current.floatingTextList[i].text = text;
                current.floatingTextList[i].color = color;
                current.floatingTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                current.floatingTextList[i].gameObject.SetActive(true);
                current.floatingTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.4f, 1.6f), displayTime).OnComplete(() => current.floatingTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
}
