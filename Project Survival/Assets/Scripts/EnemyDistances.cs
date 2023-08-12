using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDistances : MonoBehaviour
{
    public EnemyManager enemyManager;
    public List<EnemyStats> tempClosestEnemyList, closestEnemyList;
    public int maxAmountToCheck;
    float shortestDistance, distanceToEnemy;
    EnemyStats nearestEnemy;
    public bool updatingInProgress;
    void Start()
    {
        InvokeRepeating(nameof(UpdateClosestEnemyList), 0f, 0.2f);    //Repeat looking for closest targets
    }

    public void UpdateClosestEnemyList()
    {
        updatingInProgress = true;
        tempClosestEnemyList.Clear();
        for (int x = 0; x < enemyManager.enemyList.Count; x++)
        {
            shortestDistance = Mathf.Infinity;
            nearestEnemy = null;
            for (int i = 0; i < enemyManager.enemyList.Count; i++)
            {
                if (enemyManager.enemyList[i].isActiveAndEnabled && !tempClosestEnemyList.Contains(enemyManager.enemyList[i]))
                {
                    distanceToEnemy = Vector3.Distance(enemyManager.player.transform.position, enemyManager.enemyList[i].transform.position);
                    if (distanceToEnemy < shortestDistance && enemyManager.gameplayManager.maxAttackRange >= distanceToEnemy)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemyManager.enemyList[i];
                    }
                }
                if (i < enemyManager.rareEnemyList.Count)
                {
                    if (enemyManager.rareEnemyList[i].isActiveAndEnabled && !tempClosestEnemyList.Contains(enemyManager.rareEnemyList[i]))
                    {
                        distanceToEnemy = Vector3.Distance(enemyManager.player.transform.position, enemyManager.rareEnemyList[i].transform.position);
                        if (distanceToEnemy < shortestDistance && enemyManager.gameplayManager.maxAttackRange >= distanceToEnemy)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = enemyManager.rareEnemyList[i];
                        }
                    }
                }
            }
            if (nearestEnemy != null)
            {
                tempClosestEnemyList.Add(nearestEnemy);
            }
            else 
            {
                break;
            }
        }
        closestEnemyList = tempClosestEnemyList;
        updatingInProgress = false;
    }
}
