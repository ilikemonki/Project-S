using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRate : MonoBehaviour
{
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    public List<CurrenciesToDrop> collectiblesList;  //Coin
    public GameObject collectiblesParent;   //parent for collectibles except coins
    public List<ICollectibles> coinList;    //pool list
    public List<ICollectibles> magnetList;    //magnet pool list
    [Header("Orbs")]
    public List<ICollectibles> orbPrefabs;  //skill orb prefabs
    public List<ICollectibles> orbList;  //skill orb list
    public List<string> greenOrbNames, redOrbNames, blueOrbNames, yellowOrbNames;
    public float baseOrbChanceRange, orbChanceRange;
    [Header("Gems")]
    public List<ICollectibles> gemPrefabs;  //skill orb prefabs
    public List<ICollectibles> gemList;  //skill orb list
    public List<string> redGemNames, blueGemNames, yellowGemNames;
    public float baseGemChanceRange, gemChanceRange;
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
        PopulatePool(collectiblesList[1].prefab, 100, transform, coinList);
        PopulatePool(collectiblesList[5].prefab, 5, collectiblesParent.transform, magnetList);
        PopulatePool(orbPrefabs[0], 5, collectiblesParent.transform, orbList);
        PopulatePool(gemPrefabs[0], 5, collectiblesParent.transform, gemList);
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
        rand = Random.Range(0, gemChanceRange);
        if (rand <= 1)
        {
            SpawnItem(gemPrefabs[Random.Range(0, gemPrefabs.Count)], pos); //random base gem
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
                    orbList[i].transform.position = new Vector3(pos.position.x, (float)(pos.position.y + 0.4), pos.position.z);
                    orbList[i].tag = prefab.tag;
                    orbList[i].name = prefab.name;
                    orbList[i].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
                    RandomizeOrb(orbList[i]);
                    orbList[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
        if (prefab.CompareTag("Skill Gem"))
        {
            for (int i = 0; i < gemList.Count; i++)
            {
                if (i > gemList.Count - 2)
                {
                    PopulatePool(orbPrefabs[0], 5, collectiblesParent.transform, gemList);
                }
                if (!gemList[i].isActiveAndEnabled)
                {
                    gemList[i].transform.position = new Vector3((float)(pos.position.x + 0.4), pos.position.y, pos.position.z);
                    gemList[i].tag = prefab.tag;
                    gemList[i].name = prefab.name;
                    gemList[i].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
                    RandomizeGem(gemList[i]);
                    gemList[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
        for (int i = 0; i < coinList.Count; i++)
        {
            if (Vector3.Distance(coinList[i].transform.position, pos.position) < 0.6f)
            {
                if (prefab.tag.Contains("1")) coinList[i].coinAmount += 1;
                else if (prefab.tag.Contains("5")) coinList[i].coinAmount += 5;
                else if (prefab.tag.Contains("10")) coinList[i].coinAmount += 10;
                else if (prefab.tag.Contains("20")) coinList[i].coinAmount += 20;
                return;
            }
        }
        for (int i = 0; i < coinList.Count; i++) //Spawn Coin
        {
            if (i > coinList.Count - 5)
            {
                PopulatePool(collectiblesList[1].prefab, 5, transform, coinList);
            }
            if (!coinList[i].isActiveAndEnabled)
            {
                if (prefab.tag.Contains("1")) coinList[i].coinAmount = 1;
                else if (prefab.tag.Contains("5")) coinList[i].coinAmount = 5;
                else if (prefab.tag.Contains("10")) coinList[i].coinAmount = 10;
                else if (prefab.tag.Contains("20")) coinList[i].coinAmount = 20;
                coinList[i].transform.position = pos.position;
                coinList[i].tag = prefab.tag;
                coinList[i].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
                coinList[i].gameObject.SetActive(true);
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
        for (int i = 0; i < coinList.Count; i++)
        {
            if (!coinList[i].isActiveAndEnabled)
            {
                inactive++;
            }
        }
        if(inactive >= Mathf.Round(coinList.Count * 0.3f) && coinList.Count >= 500)     //if there are at least 30% inactives, destroy them.
        {
            for (int i = 0; i < coinList.Count; i++)
            {
                if (inactive == 0) break;
                if (!coinList[i].isActiveAndEnabled && inactive >= 0)
                {
                    Destroy(coinList[i].gameObject);
                    coinList.RemoveAt(i);
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
        gemChanceRange = Mathf.Round(baseGemChanceRange * ((100 - gameplayManager.dropChanceMultiplier) / 100));
    }
    public void RandomizeOrb(ICollectibles orb)
    {
        int r;
        if (orb.name.Contains("Green"))
        {
            r = Random.Range(0, greenOrbNames.Count);
            orb.name = greenOrbNames[r];
        }
        else if (orb.name.Contains("Red"))
        {
            r = Random.Range(0, redOrbNames.Count);
            orb.name = redOrbNames[r];
        }
        else if (orb.name.Contains("Blue"))
        {
            r = Random.Range(0, blueOrbNames.Count);
            orb.name = blueOrbNames[r];
        }
        else
        {
            r = Random.Range(0, yellowOrbNames.Count);
            orb.name = yellowOrbNames[r];
        }
    }
    public void RandomizeGem(ICollectibles gem)
    {
        int r;
        if (gem.name.Contains("Red"))
        {
            r = Random.Range(0, redGemNames.Count);
            gem.name = redGemNames[r];
        }
        else if (gem.name.Contains("Blue"))
        {
            r = Random.Range(0, blueGemNames.Count);
            gem.name = blueGemNames[r];
        }
        else
        {
            r = Random.Range(0, yellowGemNames.Count);
            gem.name = yellowGemNames[r];
        }
    }
}
