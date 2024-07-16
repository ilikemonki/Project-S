using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    public Collider2D hitboxCollider;
    public SkillController skillController;
    public Vector3 direction;
    public List<float> damageTypes;
    public List<bool> applyAilment;
    public float travelSpeed;
    public int pierce, chain;
    protected EnemyStats nearestEnemy;
    public Transform target;
    protected float shortestDistance, distanceToEnemy;
    public float totalDamage;
    public bool applyCrit, applyLifeSteal, hitOnceOnly;
    public bool hasHitEnemy;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;
    public List<EnemyStats> rememberEnemyList = new(); //remember the enemies hit, will not hit the same enemy again. Cannot chain at same target
    public bool stayUpRightOnly;
    public bool isOrbitSkill, rotateSkill, returnSkill, isHoming;
    public Vector3 startingPos;
    public float currentTravelRange; //travel
    public float currentDuration;
    public float currentDespawnTime;
    public float currenthitboxColliderTimer;
    public float currentDamageCooldownTimer;
    public float currentHomingTimer; //After a certain time, isHoming is activated.

    public virtual void SetStats(float physical, float fire, float cold, float lightning, float travelSpeed, int pierce, int chain, float aoe)
    {
        if (skillController.useHoming && !skillController.useMultiTarget) target = null;
        if (stayUpRightOnly)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        if (!this.enabled) this.enabled = true;
        if (!hitboxCollider.enabled) hitboxCollider.enabled = true;
        if (spriteRend != null) spriteRend.enabled = true;
        currenthitboxColliderTimer = 0;
        currentDamageCooldownTimer = 0;
        currentDespawnTime = 0;
        currentDuration = 0;
        currentTravelRange = 0;
        currentHomingTimer = 0;
        this.damageTypes[0] = physical;
        this.damageTypes[1] = fire;
        this.damageTypes[2] = cold;
        this.damageTypes[3] = lightning;
        this.travelSpeed = travelSpeed;
        this.pierce = pierce;
        this.chain = chain;
        CheckChances();
        if (skillController.isAoe)
            transform.localScale = new Vector3(skillController.prefabBehavior.transform.localScale.x * (1 + aoe / 100), skillController.prefabBehavior.transform.localScale.y * (1 + aoe / 100), 1);
    }
    public void SetDirection(Vector3 dir) //set where the skill will go.
    {
        direction = dir;
        if (!stayUpRightOnly && (direction.normalized.x < 0 && transform.localScale.y > 0) || (direction.normalized.x > 0 && transform.localScale.y < 0))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
        }
    }

    public virtual void Update()
    {
        if (stayUpRightOnly && target != null) //FlipX only if stayUpRight is true
        {
            if (target.transform.position.x > transform.position.x)
            {
                spriteRend.flipX = false;
            }
            else spriteRend.flipX = true;
        }
        if (isHoming && target != null && !skillController.isMelee) //If homing and no target, find new target.
        {
            if (!target.gameObject.activeSelf)
            {
                target = FindTarget(true);
            }
        }
        if (skillController.useHoming && !isHoming && !skillController.isMelee) //Activate homing
        {
            currentHomingTimer += Time.deltaTime;
            if (currentHomingTimer >= (100 / travelSpeed) * 0.01)
            {
                target = FindTarget(true);
                isHoming = true; 
                hitboxCollider.enabled = true;
            }
        }
        if (rememberEnemyList.Count > 0 && skillController.damageCooldown > 0) //For damage cooldown
        {
            currentDamageCooldownTimer += Time.deltaTime;
            if (currentDamageCooldownTimer >= skillController.damageCooldown)
            {
                rememberEnemyList.Clear();
                hitboxCollider.enabled = false;
                hitboxCollider.enabled = true;
                currentDamageCooldownTimer = 0;
            }
        }
        if (skillController.hitboxColliderDuration > 0 && hitboxCollider.enabled) //hitbox collider
        {
            currenthitboxColliderTimer += Time.deltaTime;
            if (currenthitboxColliderTimer >= skillController.hitboxColliderDuration)
            {
                if (skillController.resetHitBoxCollider)
                {
                    hitboxCollider.enabled = true;
                    currenthitboxColliderTimer = 0;
                }
                else
                    hitboxCollider.enabled = false;
            }
        }
        if (rotateSkill) //Rotate skill object
        {
            if (travelSpeed > 0)
                transform.Rotate(new Vector3(0, 0, 160 + (travelSpeed * 8)) * Time.deltaTime);
            else
                transform.Rotate(new Vector3(0, 0, 160 + (8)) * Time.deltaTime);
        }
        if (target != null && isHoming && !skillController.isMelee)     //Home on target.
        {
            if (target.gameObject.activeSelf)
            {
                direction = (target.position - transform.position).normalized;
                if (!stayUpRightOnly && !rotateSkill)
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
        }
        if (!isOrbitSkill)
        {
            if (!returnSkill)
            {
                if (skillController.despawnTime > 0) //skills with despawn timer
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
                else if (skillController.travelRange > 0) //skills that travel range will check the distance traveled.
                {
                    currentTravelRange = Vector3.Distance(transform.position, startingPos);
                    if (currentTravelRange >= skillController.travelRange) //reached max range
                    {
                        if (skillController.useReturn) //At end of travel, return projectile or despawn.
                        {
                            SetReturn();
                        }
                        else
                        {
                            gameObject.SetActive(false);
                        }
                    }
                }
            }
            else if (returnSkill && !skillController.useHomingReturn)
            {
                if (Vector3.Distance(transform.position, startingPos) >= currentTravelRange)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (travelSpeed > 0 && !isOrbitSkill || !skillController.useOnTarget && !isOrbitSkill)
        {
            rb.MovePosition(transform.position + (travelSpeed * Time.fixedDeltaTime * direction));
            if (isHoming) travelSpeed += 0.01f;
        }
    }


    public virtual void DoDamage(EnemyStats enemy, float damageEffectiveness)
    {
        totalDamage = damageTypes.Sum() * (damageEffectiveness / 100);
        if (applyCrit)  //Crit damage
        {
            totalDamage *= (skillController.criticalDamage / 100);
        }
        if (applyLifeSteal)  //Life Steal
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
        if (applyAilment[1])    //fire, burn
        {
            enemy.ApplyBurn((damageTypes[1] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[1] / 100));
        }
        else if (applyAilment[2])   //cold, chill
        {
            enemy.ApplyChill(skillController.ailmentsEffect[2]);
        }
        else if (applyAilment[3])   //lightning, shock
        {
            enemy.ApplyShock(skillController.ailmentsEffect[3]);
        }
        else if (applyAilment[0]) //physical, bleed
        {
            enemy.ApplyBleed((damageTypes[0] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[0] / 100));
        }
        enemy.TakeDamage(totalDamage, applyCrit);
        if ((!isOrbitSkill && skillController.damageCooldown <= 0) || skillController.damageCooldown > 0)
        {
            if (!rememberEnemyList.Contains(enemy))
            {
                rememberEnemyList.Add(enemy);
            }
        }
            if (skillController.knockBack > 0 && !enemy.knockedBack && !enemy.knockBackImmune)  //Apply knockback
        {
            enemy.KnockBack((enemy.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
        }
        if (skillController.isMelee && skillController.combo > 1 && !hasHitEnemy && skillController.comboCounter < skillController.combo)
        {
            skillController.comboCounter++;
        }
        hasHitEnemy = true;
        if (applyCrit)
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

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (hitOnceOnly) return;
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponentInParent<EnemyStats>(); if (enemy == null || !enemy.gameObject.activeSelf) return;
            if (rememberEnemyList.Count > 0)
            {
                if (rememberEnemyList.Contains(enemy)) return;
            }
            if (returnSkill)
            {
                DoDamage(enemy, 100);
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
            DoDamage(enemy, 100);
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
    public virtual void ProjectileBehavior()
    {
        if (skillController.continuous || skillController.pierceAll || skillController.cannotChain || skillController.cannotPierce) return;
        if (pierce <= 0 && chain <= 0)
        {
            if (skillController.useReturn)
            {
                SetReturn();
            }
            else
            {
                gameObject.SetActive(false);
            }
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
                    skillController.SpawnChainProjectile(rememberEnemyList, target, transform);
                }
                gameObject.SetActive(false);
                return;
            }
            chain--;
            ChainToEnemy();
        }
    }
    //Find new enemy to target.
    public void  ChainToEnemy()
    {
        target = FindTarget(true);
        if (target != null)
        {
            startingPos = transform.position;
            SetDirection((target.position - transform.position).normalized); //if target is found, set direction
            if(!stayUpRightOnly)
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else
        {
            if (skillController.useReturn)
            {
                SetReturn();
            }
            else
            {
                gameObject.SetActive(false);    //despawn if no targets found
            }
        }
    }

    public Transform FindTarget(bool closest) //if false, find furthest
    {
        if (closest) shortestDistance = Mathf.Infinity;
        else shortestDistance = 0;
        nearestEnemy = null;
        for (int i = 0; i < skillController.enemyManager.enemyList.Count; i++)  //find target in normal enemy list
        {
            bool dontChain = false;
            if (skillController.enemyManager.enemyList[i].gameObject.activeSelf)
            {
                for (int j = 0; j < rememberEnemyList.Count; j++)
                {
                    //if (rememberEnemyList.Contains(skillController.enemyManager.enemyList[i]))
                    //{
                    //    dontChain = true;
                    //    break;
                    //}
                }
                if (!dontChain)
                {
                    //distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyManager.enemyList[i].transform.position);
                    //if (!closest)
                    //{
                    //    if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.travelRange * 0.5)
                    //    {
                    //        shortestDistance = distanceToEnemy;
                    //        nearestEnemy = skillController.enemyManager.enemyList[i];
                    //    }
                    //}
                    //else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.travelRange * 0.5)
                    //{
                    //    shortestDistance = distanceToEnemy;
                    //    nearestEnemy = skillController.enemyManager.enemyList[i];
                    //}
                }
            }
        }
        if (nearestEnemy != null)
        {
            return nearestEnemy.transform;
        }
        else return null;
    }
    public void SetReturn()
    {
        if (!skillController.useHomingReturn)
        {
            isHoming = false;
        }
        rememberEnemyList.Clear();
        target = skillController.player.transform;
        SetDirection((target.position - transform.position).normalized);
        travelSpeed *= 1.5f;
        currentDuration = 0;
        currentDespawnTime = 0;
        startingPos = transform.position;
        returnSkill = true;
        currentTravelRange = Vector3.Distance(skillController.player.transform.position, startingPos);
        if (!stayUpRightOnly)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (returnSkill)
        {
            if (!target.Equals(skillController.player.transform)) SetReturn();
            if (collision.CompareTag("Player"))
            {
                gameObject.SetActive(false);
            }
        }
    }
    public virtual void OnDisable()
    {
        rememberEnemyList.Clear();
        hasHitEnemy = false;
        target = null;
        returnSkill = false;
        hitOnceOnly = false;
    }
    private void OnEnable()
    {
        startingPos = transform.position;
    }
    public void CheckChances()
    {
        applyCrit = false;
        applyLifeSteal = false;
        applyAilment[0] = false;
        applyAilment[1] = false;
        applyAilment[2] = false;
        applyAilment[3] = false;
        if (skillController.criticalChance > 0)
        {
            if (Random.Range(1, 101) <= skillController.criticalChance)  //Crit damage
            {
                applyCrit = true;
            }
        }
        if (skillController.lifeStealChance > 0)
        {
            if (Random.Range(1, 101) <= skillController.lifeStealChance)  //Life Steal
            {
                applyLifeSteal = true;
            }
        }
        if (skillController.highestDamageType.Equals(1))    //fire, burn
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[1])
            {
                applyAilment[1] = true;
            }
        }
        else if (skillController.highestDamageType.Equals(2))   //cold, chill
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[2])
            {
                applyAilment[2] = true;
            }
        }
        else if (skillController.highestDamageType.Equals(3))   //lightning, shock
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[3])
            {
                applyAilment[3] = true;
            }
        }
        else //physical, bleed
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[0])
            {
                applyAilment[0] = true;
            }
        }
    }

}
