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
    public void CheckEnemyStatus(List<Enemy> enemyList)
    {
        //for (int i = 0; i < enemyList.Count; ++i)
        //{
        //    enemyList[i].UpdateStatusEffect();
        //}
    }
    public void MoveAndAttack(List<Enemy> enemyList)
    {
        for (int i = 0; i < enemyList.Count; i++)   //Move all enemies
        {
            if (enemyList[i].gameObject.activeSelf)
            {
                distanceToPlayer = Vector3.Distance(enemyList[i].transform.position, enemyManager.player.transform.position);
                if (!enemyList[i].isAttacking)
                {
                    if (enemyList[i].enemyStats.canAttack && (distanceToPlayer <= enemyList[i].enemyStats.attackRange))   //stop to attack
                    {
                        enemyList[i].rb.velocity = Vector2.zero;
                        enemyList[i].isAttacking = true;
                        enemyList[i].attackImage.SetActive(false);
                    }
                    else
                    {
                        if (Vector3.Distance(enemyList[i].transform.position, enemyManager.player.transform.position) >= stopDistance)
                        {
                            enemyList[i].enemyStats.enemyMovement.MoveEnemy(enemyList[i]);    //Move
                        }
                    }
                }
                if (enemyList[i].enemyStats.canAttack && enemyList[i].isAttacking)    //Attack
                {
                    if (!enemyList[i].knockedBack) enemyList[i].rb.velocity = Vector2.zero;
                    enemyList[i].attackTimer -= Time.deltaTime;
                    if (enemyList[i].attackTimer <= 0.5f && enemyList[i].attackImage.activeSelf == false)
                    {
                        if (enemyList[i].enemyStats.barrageAttack && enemyList[i].barrageCounter <= 0) 
                            enemyList[i].attackImage.SetActive(true);
                        else
                            enemyList[i].attackImage.SetActive(true);
                    }
                    if (enemyList[i].attackTimer <= 0f)
                    {
                        if (enemyList[i].enemyStats.spreadAttack)
                        {
                            enemyProjectilePool.SpreadBehavior(enemyList[i], enemyManager.player.transform);
                            enemyList[i].attackTimer = enemyList[i].enemyStats.attackCooldown;
                            enemyList[i].attackImage.SetActive(false);
                            enemyList[i].isAttacking = false;
                        }
                        else if (enemyList[i].enemyStats.circleAttack)
                        {
                            enemyProjectilePool.CircleBehavior(enemyList[i], enemyManager.player.transform);
                            enemyList[i].attackTimer = enemyList[i].enemyStats.attackCooldown;
                            enemyList[i].attackImage.SetActive(false);
                            enemyList[i].isAttacking = false;
                        }
                        else if (enemyList[i].enemyStats.burstAttack)
                        {
                            enemyProjectilePool.BurstBehavior(enemyList[i], enemyManager.player.transform);
                            enemyList[i].attackTimer = enemyList[i].enemyStats.attackCooldown;
                            enemyList[i].attackImage.SetActive(false);
                            enemyList[i].isAttacking = false;
                        }
                        else if (enemyList[i].enemyStats.barrageAttack)
                        {
                            if (enemyList[i].barrageCounter < enemyList[i].enemyStats.projectile) //fires the amount of attacks
                            {
                                enemyList[i].barrageTimer += Time.deltaTime;
                                if (enemyList[i].barrageTimer >= 0.15f) //firing interval here.
                                {
                                    if (enemyList[i].enemyStats.barrageAttack)
                                        enemyProjectilePool.BarrageBehavior(enemyList[i], enemyManager.player.transform);
                                    enemyList[i].barrageCounter++;
                                    enemyList[i].barrageTimer = 0;
                                }
                            }
                            else if (enemyList[i].barrageCounter >= enemyList[i].enemyStats.projectile)
                            {
                                enemyList[i].barrageCounter = 0;
                            enemyList[i].attackTimer = enemyList[i].enemyStats.attackCooldown;
                            enemyList[i].attackImage.SetActive(false);
                            enemyList[i].isAttacking = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
