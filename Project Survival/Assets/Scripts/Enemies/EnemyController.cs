using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controlls all updates for every enemy
public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemySkillController enemyProjectilePool;
    float distanceToPlayer;
    public float stopDistance;
    public void Update()
    {
        CheckEnemyStatus(enemyManager.enemyList);
    }
    private void FixedUpdate()
    {
        MoveAndAttack(enemyManager.enemyList);
    }
    public void CheckEnemyStatus(List<EnemyStats> enemyList)
    {
        for (int i = 0; i < enemyList.Count; ++i)
        {
            enemyList[i].UpdateStatusEffect();
        }
    }
    public void MoveAndAttack(List<EnemyStats> enemyList)
    {
        for (int i = 0; i < enemyList.Count; ++i)   //Move all enemies
        {
            if (enemyList[i].isActiveAndEnabled)
            {
                distanceToPlayer = Vector3.Distance(enemyList[i].transform.position, enemyManager.player.transform.position);
                if (!enemyList[i].isAttacking)
                {
                    if (enemyList[i].canAttack && (distanceToPlayer <= enemyList[i].attackRange))   //stop to attack
                    {
                        enemyList[i].rb.velocity = Vector2.zero;
                        enemyList[i].isAttacking = true;
                        enemyList[i].attackImage.SetActive(false);
                    }
                    else
                    {
                        if (Vector3.Distance(enemyList[i].transform.position, enemyManager.player.transform.position) >= stopDistance)
                        {
                            enemyList[i].enemyMovement.MoveEnemy();    //Move
                        }
                    }
                }
                if (enemyList[i].canAttack && enemyList[i].isAttacking)    //Attack
                {
                    if (!enemyList[i].knockedBack) enemyList[i].rb.velocity = Vector2.zero;
                    enemyList[i].attackTimer -= Time.deltaTime;
                    if (enemyList[i].attackTimer <= 0.5f && enemyList[i].attackImage.activeSelf == false)
                    {
                        enemyList[i].attackImage.SetActive(true);
                    }
                    if (enemyList[i].attackTimer <= 0f)
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
                        enemyList[i].attackImage.SetActive(false); 
                        enemyList[i].isAttacking = false;
                    }
                }
            }
        }
    }
}
