using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemyProjectilePool enemyProjectilePool;
    float distanceToPlayer;
    private void FixedUpdate()
    {
        for (int i = 0; i < enemyManager.enemyList.Count; ++i)   //Move all enemies
        {
            if (enemyManager.enemyList[i].isActiveAndEnabled)
            {
                distanceToPlayer = Vector3.Distance(enemyManager.enemyList[i].transform.position, enemyManager.player.transform.position);
                if (!enemyManager.enemyList[i].knockedBack)   //knockedback
                {
                    if (enemyManager.enemyList[i].canAttack && (distanceToPlayer <= enemyManager.enemyList[i].attackRange))   //stop and attack
                    {
                        enemyManager.enemyList[i].rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        enemyManager.enemyList[i].enemyMovement.MoveEnemy();    //Move
                    }
                }
                if (enemyManager.enemyList[i].canAttack)    //Attack
                {
                    enemyManager.enemyList[i].attackTimer -= Time.deltaTime;
                    if (enemyManager.enemyList[i].attackTimer <= 0f)
                    {
                        //distanceToPlayer = Vector3.Distance(enemyManager.enemyList[i].transform.position, enemyManager.player.transform.position);
                        if (distanceToPlayer <= enemyManager.enemyList[i].attackRange)
                        {
                            enemyProjectilePool.SpawnProjectile(enemyManager.enemyList[i], enemyManager.player.transform);
                            enemyManager.enemyList[i].attackTimer = enemyManager.enemyList[i].attackCooldown;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < enemyManager.rareEnemyList.Count; ++i)   //Move all enemies
        {
            if (enemyManager.rareEnemyList[i].isActiveAndEnabled)
            {
                if (!enemyManager.rareEnemyList[i].knockedBack)   //knockedback
                {
                    enemyManager.rareEnemyList[i].enemyMovement.MoveEnemy();    //Move
                }
                if (enemyManager.rareEnemyList[i].canAttack)    //Attack
                {
                    enemyManager.rareEnemyList[i].attackTimer -= Time.deltaTime;
                    if (enemyManager.rareEnemyList[i].attackTimer <= 0f)
                    {
                        distanceToPlayer = Vector3.Distance(enemyManager.rareEnemyList[i].transform.position, enemyManager.player.transform.position);
                        if (distanceToPlayer <= enemyManager.rareEnemyList[i].attackRange)
                        {
                            enemyProjectilePool.SpawnProjectile(enemyManager.rareEnemyList[i], enemyManager.player.transform);
                            enemyManager.rareEnemyList[i].attackTimer = enemyManager.rareEnemyList[i].attackCooldown;
                        }
                    }
                }
            }
        }
    }

}
