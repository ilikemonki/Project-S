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
        PopulatePool(50);
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

    public void DisplayFloatingText(Transform transform, string text)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (i >= 400) return;
            if (!damageTextList[i].isActiveAndEnabled)
            {
                if (i > damageTextList.Count - 2)
                {
                    PopulatePool(10);
                }
                damageTextList[i].fontSize = textSize;
                damageTextList[i].text = text;
                damageTextList[i].color = Color.white;
                damageTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.8f, 1.2f), 0.6f).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public void DisplayFloatingCritText(Transform transform, string text)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (i >= 400) return;
            if (!damageTextList[i].isActiveAndEnabled)
            {
                if (i > damageTextList.Count - 2)
                {
                    PopulatePool(10);
                }
                damageTextList[i].fontSize = textCritSize;
                damageTextList[i].text = text;
                damageTextList[i].color = Color.yellow;
                damageTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.8f, 1.2f), 0.6f).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
    public void DisplayPlayerText(Transform transform, string text, Color color, float displayTime)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (!damageTextList[i].isActiveAndEnabled)
            {
                if (i > damageTextList.Count - 2)
                {
                    PopulatePool(10);
                }
                damageTextList[i].fontSize = textSize;
                damageTextList[i].text = text;
                damageTextList[i].color = color;
                damageTextList[i].transform.position = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + Random.Range(0.4f, 1.6f), displayTime).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
}
