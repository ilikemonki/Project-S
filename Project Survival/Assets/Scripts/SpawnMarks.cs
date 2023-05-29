using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMarks : MonoBehaviour
{
    public GameObject prefab;
    int inactive;
    public List<GameObject> spawnMarkList;
    // Start is called before the first frame update
    void Start()
    {
        PopulatePool(20);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CheckPoolAmount()
    {
        inactive = 0;
        for (int i = 0; i < spawnMarkList.Count; i++)
        {
            if (!spawnMarkList[i].activeSelf) inactive++;    //Calculate how many inactives there are
        }
        if (inactive <= 10) //if inactives are close to the number of prefabs needed, spawn more.
        {
            PopulatePool(10);
        }
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
    public int Spawn(Vector2 spawnPos)
    {
        CheckPoolAmount();
        for (int i = 0; i < spawnMarkList.Count; i++)
        {
            if (!spawnMarkList[i].activeSelf)   //find inactive mark to spawn
            {
                spawnMarkList[i].transform.position = spawnPos;
                spawnMarkList[i].SetActive(true);
                return i;
            }
        }
        return 0;
        
    }

    public void Despawn(int indexToDespawn)
    {
        spawnMarkList[indexToDespawn].SetActive(false);
    }
}
