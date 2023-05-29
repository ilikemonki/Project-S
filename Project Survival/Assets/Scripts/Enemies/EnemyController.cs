using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public List<EnemyStats> enemyList = new();

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < enemyList.Count; ++i)   //Move all enemies
        {
            if (enemyList[i].isActiveAndEnabled)
            {
                enemyList[i].enemyMovement.MoveEnemy();
            }
        }
    }
    public void AddToList(EnemyStats e)
    {
        enemyList.Add(e);
    }

}
