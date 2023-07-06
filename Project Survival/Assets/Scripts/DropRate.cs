using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRate : MonoBehaviour
{
    public GameplayManager gameplayManager;
    public List<CoinsToDrop> collectiblesList;  //Coin
    private float rand;
    public GameObject magnetParent;
    public List<ICollectibles> itemList;    //pool list
    public List<ICollectibles> magnetList;    //magnet pool list
    [System.Serializable]
    public class CoinsToDrop    //plus magnet
    {
        public ICollectibles prefab;
        public float chance;
        public float baseChanceRange;
        public float chanceRange;
    }
    [System.Serializable]
    public class SkillsToDrop
    {
        public ICollectibles prefab;
        public float chance;
        public float baseChanceRange;
        public float chanceRange;
    }
    private void Start()
    {
        InvokeRepeating(nameof(DeleteInactives), 10, 30f);
        PopulatePool(collectiblesList[0].prefab, 100, transform, itemList);
        PopulatePool(collectiblesList[5].prefab, 5, magnetParent.transform, magnetList);
        UpdateDropChance(); 
    }

    public void DropLoot(Transform pos)
    {
        foreach (CoinsToDrop item in collectiblesList)  //coin drop chance only
        {
            rand = Random.Range(1, item.chanceRange + 1);
            if (rand <= item.chance)
            {
                SpawnItem(item.prefab, pos);
                break;
            }
        }
        rand = Random.Range(1, collectiblesList[5].chanceRange + 1);
        if (rand <= collectiblesList[5].chance) //magnet drop chance only
        {
            SpawnItem(collectiblesList[5].prefab, pos);
        }
    }

    public void SpawnItem(ICollectibles prefab, Transform pos)
    {
        if (prefab.CompareTag("Magnet"))
        {
            for (int i = 0; i < magnetList.Count; i++)
            {
                if (i > magnetList.Count - 2)
                {
                    PopulatePool(collectiblesList[5].prefab, 5, magnetParent.transform, magnetList);
                }
                if (!magnetList[i].isActiveAndEnabled)
                {
                    magnetList[i].transform.position = pos.position;
                    magnetList[i].gameObject.SetActive(true);
                    magnetList[i].MagnetDuration();
                    return;
                }
            }
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            if (i > itemList.Count - 5)
            {
                PopulatePool(collectiblesList[0].prefab, 5, transform, itemList);
            }
            if (!itemList[i].isActiveAndEnabled)
            {
                itemList[i].transform.position = pos.position;
                itemList[i].tag = prefab.tag;
                itemList[i].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
            itemList[i].gameObject.SetActive(true);
                return;
            }
        }
    }

    protected virtual void PopulatePool(ICollectibles prefab, int numToSpawn, Transform parent, List<ICollectibles> poolList)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            ICollectibles obj = Instantiate(prefab, parent);    //Spawn, add to list, and initialize prefabs
            obj.gameObject.SetActive(false);
            poolList.Add(obj);
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

    public void UpdateDropChance()
    {
        foreach (CoinsToDrop item in collectiblesList)
        {
            item.chanceRange = Mathf.Round(item.baseChanceRange * ((100 - gameplayManager.dropChanceMultiplier) / 100));
        }
    }
}
