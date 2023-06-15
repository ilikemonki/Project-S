using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRate : MonoBehaviour
{
    public List<ItemsToDrop> collectiblesList;  //Health Potion, Coin
    private float rand;
    public List<ICollectibles> itemList;    //pool list
    [System.Serializable]
    public class ItemsToDrop
    {
        public ICollectibles prefab;
        public float chance;
        public float chanceOver;
    }
    private void Start()
    {
        InvokeRepeating(nameof(DeleteInactives), 10, 30f);
        PopulatePool(collectiblesList[^1], 100);
    }

    public void DropLoot(Transform pos)
    {
        foreach (ItemsToDrop item in collectiblesList)
        {
            rand = Random.Range(1, item.chanceOver + 1);
            if (rand <= item.chance)
            {
                SpawnItem(item, pos);
                return;
            }
        }
    }

    public void SpawnItem(ItemsToDrop item, Transform pos)
    {
        if (item.prefab.CompareTag("Magnet"))
        {
            ICollectibles obj = Instantiate(item.prefab, transform);    //Spawn, add to list, and initialize prefabs
            obj.transform.position = pos.position;
            obj.gameObject.SetActive(true);
            obj.MagnetDuration();
            return;
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            if (i > itemList.Count - 5)
            {
                PopulatePool(collectiblesList[^1], 5);
            }
            if (!itemList[i].isActiveAndEnabled)
            {
                itemList[i].transform.position = pos.position;
                itemList[i].tag = item.prefab.tag;
                itemList[i].spriteRenderer.sprite = item.prefab.spriteRenderer.sprite;
            itemList[i].gameObject.SetActive(true);
                return;
            }
        }
    }

    protected virtual void PopulatePool(ItemsToDrop item, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            ICollectibles obj = Instantiate(collectiblesList[^1].prefab, transform);    //Spawn, add to list, and initialize prefabs
            obj.gameObject.SetActive(false);
            //ICollectibles c = obj.GetComponent<ICollectibles>();
            itemList.Add(obj);
        }
    }

    public void DeleteInactives()
    {
        int inactive = 0;
        int destroyed = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (!itemList[i].isActiveAndEnabled)
            {
                inactive++;
            }
        }
        if(inactive >= Mathf.Round(itemList.Count * 0.3f) && itemList.Count >= 500)     //if there are at least 30% inactives, destroy them.
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (inactive == 0) break;
                if (!itemList[i].isActiveAndEnabled && inactive >= 0)
                {
                    Destroy(itemList[i].gameObject);
                    itemList.RemoveAt(i);
                    inactive--;
                    destroyed++;
                }
            }
        }
    }
}
