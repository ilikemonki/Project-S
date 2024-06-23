using System.Collections.Generic;
using UnityEngine;

public class EnemyDistances : MonoBehaviour
{
    public EnemyManager enemyManager;
    public List<EnemyStats> tempClosestEnemyList, closestEnemyList;
    public int maxAmountToCheck;
    private float shortestDistance, distanceToEnemy;
    private EnemyStats nearestEnemy;
    public bool updatingInProgress;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateClosestEnemyList), 0f, 0.2f);    //Repeat looking for closest targets
    }


    public void UpdateClosestEnemyList()
    {
        updatingInProgress = true;
        tempClosestEnemyList.Clear();
        for (int x = 0; x < maxAmountToCheck; x++) //amount of mobs to add into target list
        {
            shortestDistance = Mathf.Infinity;
            nearestEnemy = null;
            for (int i = 0; i < enemyManager.enemyList.Count; i++) //search all mobs for nearest distance
            {
                if (enemyManager.enemyList[i].isActiveAndEnabled && !tempClosestEnemyList.Contains(enemyManager.enemyList[i]))
                {
                    distanceToEnemy = Vector3.Distance(enemyManager.player.transform.position, enemyManager.enemyList[i].transform.position);
                    if (distanceToEnemy < shortestDistance && enemyManager.gameplayManager.furthestAttackRange >= distanceToEnemy)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemyManager.enemyList[i];
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