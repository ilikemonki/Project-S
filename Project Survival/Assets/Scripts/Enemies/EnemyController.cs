using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemySkillController enemyProjectilePool;
    float distanceToPlayer;
    private void FixedUpdate()
    {
        MoveAndAttack(enemyManager.enemyList);
    }

    public void MoveAndAttack(List<EnemyStats> enemyList)
    {
        for (int i = 0; i < enemyList.Count; ++i)   //Move all enemies
        {
            if (enemyList[i].isActiveAndEnabled)
            {
                distanceToPlayer = Vector3.Distance(enemyList[i].transform.position, enemyManager.player.transform.position);
                if (!enemyList[i].knockedBack)   //knockedback
                {
                    if (enemyList[i].canAttack && (distanceToPlayer <= enemyList[i].attackRange))   //stop and attack
                    {
                        enemyList[i].rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        enemyList[i].enemyMovement.MoveEnemy();    //Move
                    }
                }
                if (enemyList[i].canAttack)    //Attack
                {
                    enemyList[i].attackTimer -= Time.deltaTime;
                    if (enemyList[i].attackTimer <= 0f)
                    {
                        if (distanceToPlayer <= enemyList[i].attackRange)
                        {
                            if (enemyList[i].spreadAttack)
                            {
                                enemyProjectilePool.SpreadBehavior(enemyList[i], enemyManager.player.transform);
                            }
                            else if (enemyList[i].circleAttack)
                            {
                                enemyProjectilePool.CircleBehavior(enemyList[i]);
                            }
                            else if (enemyList[i].burstAttack)
                            {
                                enemyProjectilePool.BurstBehavior(enemyList[i], enemyManager.player.transform);
                            }
                            enemyList[i].attackTimer = enemyList[i].attackCooldown;
                        }
                    }
                }
            }
        }
    }
}
