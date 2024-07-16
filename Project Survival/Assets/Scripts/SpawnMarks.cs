using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnMarks : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> spawnMarkList;
    public EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        PopulatePool(20);
    }

    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject mark = Instantiate(prefab, gameObject.transform);    //Spawn, add to list, and initialize prefabs
            mark.SetActive(false);
            spawnMarkList.Add(mark);
        }
    }

    //spawn mark
    public void Spawn(Vector2 spawnPos, Enemy enemy)
    {
        for (int i = 0; i < spawnMarkList.Count; i++)
        {
            if (i > spawnMarkList.Count - 2)    //Check pool, add more if neccessary
            {
                PopulatePool(5);
            }
            if (!spawnMarkList[i].activeSelf)   //find inactive mark to spawn
            {
                spawnMarkList[i].transform.localScale = Vector3.one;
                spawnMarkList[i].transform.localPosition = spawnPos;
                spawnMarkList[i].SetActive(true);
                spawnMarkList[i].transform.DOScale(Vector3.one * 0.5f, 2f).SetEase(Ease.InBounce).OnComplete(() =>
                {
                    spawnMarkList[i].SetActive(false);
                    enemy.gameObject.SetActive(true);
                    enemy.isSpawning = false;
                    enemyManager.enemiesAlive++;
                });
                return;
            }
        }
    }
}
