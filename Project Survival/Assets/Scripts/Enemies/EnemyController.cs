using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    private void FixedUpdate()
    {
        for (int i = 0; i < enemyManager.enemyList.Count; ++i)   //Move all enemies
        {
            if (enemyManager.enemyList[i].isActiveAndEnabled)
            {
                if (!enemyManager.enemyList[i].knockedBack)   //knockedback
                {
                    enemyManager.enemyList[i].enemyMovement.MoveEnemy();
                }
            }
        }
        for (int i = 0; i < enemyManager.rareEnemyList.Count; ++i)   //Move all enemies
        {
            if (enemyManager.rareEnemyList[i].isActiveAndEnabled)
            {
                if (!enemyManager.rareEnemyList[i].knockedBack)   //knockedback
                {
                    enemyManager.rareEnemyList[i].enemyMovement.MoveEnemy();
                }
            }
        }
    }

}
