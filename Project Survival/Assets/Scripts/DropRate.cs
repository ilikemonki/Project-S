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
        PopulatePool(collectiblesList[0].prefab, 100, transform, coinList);
        PopulatePool(collectiblesList[4].prefab, 5, collectiblesParent.transform, magnetList);
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
        rand = Random.Range(0, collectiblesList[4].chanceRange);
        if (rand <= 1) //magnet drop chance only
        {
            SpawnItem(collectiblesList[4].prefab, pos);
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
                    PopulatePool(collectiblesList[4].prefab, 5, collectiblesParent.transform, magnetList);
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
        for (int i = 0; i < coinList.Count; i++) //Merge coin if distance is close
        {
            if (Vector3.Distance(coinList[i].transform.position, pos.position) < 0.7f)
            {
                coinList[i].coinAmount += prefab.coinAmount;
                //Change sprite
                if (coinList[i].coinAmount >= 5 && coinList[i].coinAmount < 10) coinList[i].spriteRenderer.sprite = collectiblesList[2].prefab.spriteRenderer.sprite;
                else if (coinList[i].coinAmount >= 10 && coinList[i].coinAmount < 20) coinList[i].spriteRenderer.sprite = collectiblesList[1].prefab.spriteRenderer.sprite;
                else if (coinList[i].coinAmount >= 20) coinList[i].spriteRenderer.sprite = collectiblesList[0].prefab.spriteRenderer.sprite;
                return;
            }
        }
        for (int i = 0; i < coinList.Count; i++) //Spawn Coin
        {
            if (i > coinList.Count - 5)
            {
                PopulatePool(collectiblesList[0].prefab, 5, transform, coinList);
            }
            if (!coinList[i].isActiveAndEnabled)
            {
                coinList[i].coinAmount = prefab.coinAmount;
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
    }
}
