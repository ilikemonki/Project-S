using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextController : MonoBehaviour
{
    public GameObject prefab;
    public List<TextMeshProUGUI> damageTextList;
    public float textSize, textCritSize;
    public float rand;
    private void Start()
    {
        DOTween.SetTweensCapacity(400, 50);
        PopulatePool(30);
    }

    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject g = Instantiate(prefab, transform);    //Spawn, add to list, and initialize prefabs
            TextMeshProUGUI dText = g.GetComponent<TextMeshProUGUI>();
            dText.gameObject.SetActive(false);
            damageTextList.Add(dText);
        }
    }

    public void DisplayFloatingText(Transform transform, float dmg, Color color)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (i >= 400) return;
            if (!damageTextList[i].isActiveAndEnabled)
            {
                if (i > damageTextList.Count - 5)
                {
                    PopulatePool(10);
                }
                damageTextList[i].fontSize = textSize;
                damageTextList[i].text = (dmg).ToString();
                damageTextList[i].color = color;
                damageTextList[i].transform.position = transform.position;
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + 1f, 0.60f).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public void DisplayFloatingCritText(Transform transform, float dmg, Color color)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (i >= 400) return;
            if (!damageTextList[i].isActiveAndEnabled)
            {
                if (i > damageTextList.Count - 5)
                {
                    PopulatePool(10);
                }
                damageTextList[i].fontSize = textCritSize;
                damageTextList[i].text = "*" + (dmg).ToString() + "*";
                damageTextList[i].color = color;
                damageTextList[i].transform.position = transform.position;
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + 1f, 0.60f).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
}
