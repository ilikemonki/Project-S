using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextController : MonoBehaviour
{
    public GameObject prefab;
    public List<TextMeshProUGUI> damageTextList;

    private void Start()
    {
        PopulatePool(30);
    }

    public void CheckPoolAmount()
    {
        int inactive = 0;
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (!damageTextList[i].isActiveAndEnabled) inactive++;    //Calculate how many inactives there are
        }
        if (inactive <= 20) //if inactives are less than the number, spawn more. Change number if not enough.
        {
            PopulatePool(10);
        }
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

    public void DisplayDamageText(Transform transform, float dmg)
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (!damageTextList[i].isActiveAndEnabled)
            {
                damageTextList[i].text = (dmg).ToString();
                damageTextList[i].transform.position = transform.position;
                damageTextList[i].gameObject.SetActive(true);
                damageTextList[i].transform.DOMoveY(transform.position.y + 1f, 0.5f).OnComplete(() => damageTextList[i].gameObject.SetActive(false));
                return;
            }
        }
    }
}
