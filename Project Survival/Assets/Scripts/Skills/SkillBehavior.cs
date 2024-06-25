using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    public SkillController skillController;
    public Vector3 direction;
    public List<float> damageTypes;
    public float travelSpeed;
    public int pierce, chain;
    protected EnemyStats nearestEnemy;
    public Transform target;
    protected float shortestDistance, distanceToEnemy;
    float totalDamage;
    bool isCrit, hitOnceOnly;
    bool hasHitEnemy;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;
    public List<EnemyStats> enemyChainList;    //remember the index of enemies hit by chain, will not hit the same enemy again.
    public bool isOrbitSkill, rotateSkill, returnSkill, isHoming;
    public Vector3 startingPos;
    public float currentTravelRange; //travel
    public float currentDuration;
    public float currentDespawnTime;

    public void SetStats(float physical, float fire, float cold, float lightning, float travelSpeed, int pierce, int chain, float size)
    {
        currentDespawnTime = 0;
        currentDuration = 0;
        currentTravelRange = 0;
        this.damageTypes[0] = physical;
        this.damageTypes[1] = fire;
        this.damageTypes[2] = cold;
        this.damageTypes[3] = lightning;
        this.travelSpeed = travelSpeed;
        this.pierce = pierce;
        this.chain = chain;
        transform.localScale = new Vector3(skillController.prefabBehavior.transform.localScale.x + (size / 100), skillController.prefabBehavior.transform.localScale.y + (size / 100), 1);
    }

    protected virtual void Update()
    {
        if (rotateSkill && travelSpeed > 0)
        {
            transform.Rotate(new Vector3(0, 0, 160 + (travelSpeed * 8)) * Time.deltaTime);
        }
        if (target != null && target.gameObject.activeSelf && isHoming)     //Home on enemy.
        {
            direction = (target.position - transform.position).normalized;
            if (!rotateSkill)
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        if (!isOrbitSkill)
        {
            if (!returnSkill)
            {
                if (travelSpeed <= 0 && skillController.duration <= 0) //skills that doesn't travel or have duration will despawn
                {
                    currentDespawnTime += Time.deltaTime;
                    if (currentDespawnTime >= skillController.despawnTime)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else if (skillController.duration > 0) //Skills with duration (can have travelspeed) will use duration
                {
                    currentDuration += Time.deltaTime;
                    if (currentDuration >= skillController.duration)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else if (travelSpeed > 0) //skills that travel range will check the distance traveled.
                {
                    currentTravelRange = Vector3.Distance(transform.position, startingPos);
                    if (currentTravelRange >= skillController.travelRange)
                    {
                        if (skillController.useReturnDirection && !skillController.isMelee) //At end of travel, return projectile.
                        {
                            returnSkill = true;
                            target = skillController.player.transform;
                        }
                        else
                            gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (travelSpeed > 0 && !isOrbitSkill || !skillController.useOnTarget && !isOrbitSkill)
        {
            rb.MovePosition(transform.position + (travelSpeed * Time.fixedDeltaTime * direction));
        }
    }

    public void SetDirection(Vector3 dir) //set where the skill will go.
    {
        direction = dir;
        if ((direction.normalized.x < 0 && transform.localScale.y > 0) || (direction.normalized.x > 0 && transform.localScale.y < 0))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
        }
    }

    public void DoDamage(EnemyStats enemy, float damageEffectiveness)
    {
        totalDamage = damageTypes.Sum() * (damageEffectiveness / 100);
        isCrit = false;
        if (Random.Range(1, 101) <= skillController.criticalChance)  //Crit damage
        {
            isCrit = true;
            totalDamage *= (skillController.criticalDamage / 100);
        }
        if (Random.Range(1, 101) <= skillController.lifeStealChance)  //Life Steal
        {
            if (skillController.player.currentHealth < skillController.player.maxHealth)
            {
                skillController.player.Heal(skillController.lifeSteal, true);
                GameManager.totalLifeStealProc++;
                if (skillController.lifeSteal + skillController.player.currentHealth > skillController.player.maxHealth)
                    GameManager.totalLifeSteal += skillController.player.maxHealth - skillController.player.currentHealth;
                else
                    GameManager.totalLifeSteal += skillController.lifeSteal;
            }
        }
        if (skillController.highestDamageType.Equals(1))    //fire, burn
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[1])
            {
                enemy.ApplyBurn((damageTypes[1] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[1] / 100));
            }
        }
        else if (skillController.highestDamageType.Equals(2))   //cold, chill
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[2])
            {
                enemy.ApplyChill(skillController.ailmentsEffect[2]);
            }
        }
        else if (skillController.highestDamageType.Equals(3))   //lightning, shock
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[3])
            {
                enemy.ApplyShock(skillController.ailmentsEffect[3]);
            }
        }
        else //physical, bleed
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[0])
            {
                enemy.ApplyBleed((damageTypes[0] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[0] / 100));
            }
        }
        enemy.TakeDamage(totalDamage, isCrit);
        if (skillController.isMelee && skillController.combo > 1 && !hasHitEnemy && skillController.comboCounter < skillController.combo)
        {
            hasHitEnemy = true;
            skillController.comboCounter++;
        }
        if (isCrit)
        {
            foreach (InventoryManager.Skill sc in skillController.player.gameplayManager.inventory.activeSkillList) //Check crit trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useCritTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++; 
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (hitOnceOnly) return;
        if (col.CompareTag("Enemy") || col.CompareTag("Rare Enemy"))
        {
            EnemyStats enemy = col.GetComponentInParent<EnemyStats>(); if (enemy == null) return;
            if (returnSkill)
            {
                DoDamage(enemy, 50);
                return;
            }
            if (skillController.useOrbit && isOrbitSkill && gameObject.activeSelf)   //Object is a weapon and using orbit behavior
            {
                currentDespawnTime = 0;
                if (skillController.isMelee)
                {
                    gameObject.SetActive(false);
                    if (skillController.useOnTarget)
                        skillController.SpreadBehavior(1, 0, enemy.transform, transform, false);
                    else
                        skillController.SpreadBehavior(1, 0, enemy.transform, transform, false);
                    return;
                }
            }

            if ((skillController.isMelee || pierce <= 0) && enemyChainList.Contains(enemy)) //Melee and chain will hit enemies only once
            {
                return;
            }
            DoDamage(enemy, 100);
            if (skillController.knockBack > 0 && !enemy.knockedBack && !enemy.knockBackImmune)  //Apply knockback
            {
                enemy.KnockBack((col.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
            }
            if ((pierce <= 0 && chain > 0 || skillController.isMelee)) //check if there are chains, add enemy to list to not chain again.
            {
                if (!enemyChainList.Contains(enemy))    //if enemy is not in list, add it.
                {
                    enemyChainList.Add(enemy);
                }
            }
            if (!skillController.isMelee)   //For projectiles only
            {
                ProjectileBehavior();
            }
        }
        if (col.CompareTag("Player") && returnSkill)
        {
            gameObject.SetActive(false);
        }
    }

    //How projectile act after hitting enemy
    void ProjectileBehavior()
    {
        if (skillController.continuous) return;
        if (pierce <= 0 && chain <= 0)
        {
            if (skillController.useReturnDirection)
            {
                returnSkill = true;
                target = skillController.player.transform;
            }
            else
                gameObject.SetActive(false);
            return;
        }
        if (pierce > 0)  //behavior for pierce
        {
            pierce--;
        }
        else if (chain > 0)   //behavior for chain
        {
            if (isOrbitSkill) //if projectile is an orbit object, despawn it and spawn in skill from poolList
            {
                target = FindTarget(true);
                if (target != null)
                {
                    skillController.SpawnChainProjectile(enemyChainList, target, transform);
                }
                gameObject.SetActive(false);
                return;
            }
            chain--;
            ChainToEnemy();
        }
    }
    //Find new enemy to target.
    void  ChainToEnemy()
    {
        target = FindTarget(true);
        if (target != null)
        {
            startingPos = transform.position;
            SetDirection((target.position - transform.position).normalized); //if target is found, set direction
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else
        {
            if (skillController.useReturnDirection)
            {
                returnSkill = true;
                target = skillController.player.transform;
            }
            else
                gameObject.SetActive(false);    //despawn if no targets found
        }
    }

    public Transform FindTarget(bool closest)
    {
        if (closest) shortestDistance = Mathf.Infinity;
        else shortestDistance = 0;
        nearestEnemy = null;
        for (int i = 0; i < skillController.enemyManager.enemyList.Count; i++)  //find target in normal enemy list
        {
            bool dontChain = false;
            if (skillController.enemyManager.enemyList[i].isActiveAndEnabled)
            {
                for (int j = 0; j < enemyChainList.Count; j++)
                {
                    if (enemyChainList.Contains(skillController.enemyManager.enemyList[i]))
                    {
                        dontChain = true;
                        break;
                    }
                }
                if (!dontChain)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyManager.enemyList[i].transform.position);
                    if (!closest)
                    {
                        if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.travelRange)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = skillController.enemyManager.enemyList[i];
                        }
                    }
                    else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.travelRange)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyManager.enemyList[i];
                    }
                }
            }
        }
        if (nearestEnemy != null)
        {
            return nearestEnemy.transform;
        }
        else return null;
    }

    private void OnDisable()
    {
        enemyChainList.Clear();
        hasHitEnemy = false;
        target = null;
        returnSkill = false;
        hitOnceOnly = false;
    }
    private void OnEnable()
    {
        startingPos = transform.position;
    }

}
