using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRate : MonoBehaviour
{
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    public List<CurrenciesToDrop> collectiblesList;  //Coin
    public GameObject collectiblesParent;   //parent for collectibles except coins
    public List<ICollectibles> itemList;    //pool list
    public List<ICollectibles> magnetList;    //magnet pool list
    [Header("Orbs")]
    public List<ICollectibles> orbPrefabs;  //skill orb prefabs
    public List<ICollectibles> orbList;  //skill orb list
    public List<string> normalOrbNames, rareOrbNames;
    public float baseOrbChanceRange, orbChanceRange;
    [System.Serializable]
    public class CurrenciesToDrop    //plus magnet
    {
        public ICollectibles prefab;
        public float baseChanceRange;
        public float chanceRange;
    }
    private void Start()
    {
        InvokeRepeating(nameof(DeleteInactives), 10, 30f);
        PopulatePool(collectiblesList[0].prefab, 100, transform, itemList);
        PopulatePool(collectiblesList[5].prefab, 5, collectiblesParent.transform, magnetList);
        PopulatePool(orbPrefabs[0], 5, collectiblesParent.transform, orbList);
        UpdateDropChance(); 
    }

    public void DropLoot(Transform pos)
    {
        float rand;
        foreach (CurrenciesToDrop item in collectiblesList)  //coin drop chance only. 1 changeRange means 100% chance to drop.
        {
            rand = Random.Range(0, item.chanceRange);
            if (rand <= 1)
            {
                SpawnItem(item.prefab, pos);
                break;
            }
        }
        rand = Random.Range(0, collectiblesList[5].chanceRange);
        if (rand <= 1) //magnet drop chance only
        {
            SpawnItem(collectiblesList[5].prefab, pos);
        }
        //drop orb/gem
        rand = Random.Range(0, orbChanceRange);
        if (rand <= 1)
        {
            SpawnItem(orbPrefabs[Random.Range(0, orbPrefabs.Count)], pos); //random base orb
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
                    PopulatePool(collectiblesList[5].prefab, 5, collectiblesParent.transform, magnetList);
                }
                if (!magnetList[i].isActiveAndEnabled)
                {
                    magnetList[i].transform.position = new Vector3((float)(pos.position.x - 0.4), pos.position.y, pos.position.z);
                    magnetList[i].gameObject.SetActive(true);
                    magnetList[i].MagnetDuration();
                    return;
                }
            }
        }
        if (prefab.CompareTag("Skill Orb"))
        {
            for (int i = 0; i < orbList.Count; i++)
            {
                if (i > orbList.Count - 2)
                {
                    PopulatePool(orbPrefabs[0], 5, collectiblesParent.transform, orbList);
                }
                if (!orbList[i].isActiveAndEnabled)
                {
                    orbList[i].transform.position = new Vector3((float)(pos.position.x + 0.4), pos.position.y, pos.position.z);
                    orbList[i].tag = prefab.tag;
                    orbList[i].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
                    RandomizeOrb(orbList[i]);
                    orbList[i].gameObject.SetActive(true);
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
        foreach (CurrenciesToDrop item in collectiblesList)
        {
            item.chanceRange = Mathf.Round(item.baseChanceRange * ((100 - gameplayManager.dropChanceMultiplier) / 100));
        }
        orbChanceRange = Mathf.Round(baseOrbChanceRange * ((100 - gameplayManager.dropChanceMultiplier) / 100));
    }
    public void RandomizeOrb(ICollectibles orb)
    {
        int r = Random.Range(0, 100);
        if (r < 90) //Chance for normal orbs
        {
            r = Random.Range(0, normalOrbNames.Count);
            orb.name = normalOrbNames[r];
        }
        else //rare orbs
        {
            //r = Random.Range(0, rareOrbNames.Count);
            //orb.name = rareOrbNames[r];
        }
    }
}
