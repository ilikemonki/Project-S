using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRate : MonoBehaviour
{
    public List<ItemsToDrop> collectiblesList;  //Health Potion, Coin
    public GameplayManager gameplayManager;
    private float rand;
    [System.Serializable]
    public class ItemsToDrop
    {
        public GameObject prefab;
        public float chance;
        public int spawnAtStart;
        public GameObject parent;
        public List<ICollectibles> itemList;
    }
    private void Start()
    {
        InvokeRepeating(nameof(DeleteInactives), 10, 30f);
        foreach (ItemsToDrop item in collectiblesList)
        {
            PopulatePool(item.prefab, item.spawnAtStart, item.parent, item.itemList);  //Instantiate all items to their pools.
        }
    }

    public void DropLoot(Transform pos)
    {
        foreach (ItemsToDrop item in collectiblesList)
        {
            rand = Random.Range(1, 100);
            if (rand <= item.chance)
            {
                SpawnItem(item, pos);
                return;
            }
        }
    }

    public void SpawnItem(ItemsToDrop item, Transform pos)
    {
        for (int i = 0; i < item.itemList.Count; i++)
        {
            if(i > item.itemList.Count - 5)
            {
                PopulatePool(item.prefab, 5, item.parent, item.itemList);
            }
            if (!item.itemList[i].isActiveAndEnabled)
            {
                item.itemList[i].transform.position = pos.position;
                item.itemList[i].gameObject.SetActive(true);
                return;
            }
        }

    }

    protected virtual void PopulatePool(GameObject prefab, int spawnAmount, GameObject parent, List<ICollectibles> itemList)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject obj = Instantiate(prefab, parent.transform);    //Spawn, add to list, and initialize prefabs
            obj.SetActive(false);
            ICollectibles c = obj.GetComponent<ICollectibles>();
            c.gameplayManager = gameplayManager;
            itemList.Add(c);
        }
    }

    public void DeleteInactives()
    {
        int inactive;
        int destroyed;
        foreach (ItemsToDrop item in collectiblesList)
        {
            inactive = 0;
            destroyed = 0;
            for (int i = 0; i < item.itemList.Count; i++)
            {
                if (!item.itemList[i].isActiveAndEnabled)
                {
                    inactive++;
                }
            }
            if(inactive >= Mathf.Round(item.itemList.Count * 0.3f) && item.itemList.Count >= 500)     //if there are at least 30% inactives, destroy them.
            {
                for (int i = 0; i < item.itemList.Count; i++)
                {
                    if (inactive == 0) break;
                    if (!item.itemList[i].isActiveAndEnabled && inactive >= 0)
                    {
                        Destroy(item.itemList[i].gameObject);
                        item.itemList.RemoveAt(i);
                        inactive--;
                        destroyed++;
                    }
                }
            }
        }
    }
}
