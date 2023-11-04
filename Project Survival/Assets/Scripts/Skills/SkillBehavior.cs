using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    public SkillController skillController;
    public Vector3 direction;
    public List<float> damages;
    public float travelSpeed;
    public int pierce, chain;
    public float despawnTime;
    public List<float> ailmentsChance;
    public List<float> ailmentsEffect;
    protected EnemyStats nearestEnemy;
    public Transform target;
    protected float shortestDistance, distanceToEnemy;
    float totalDamage;
    bool isCrit, hitOnceOnly;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;
    public List<EnemyStats> enemyChainList;    //remember the index of enemies hit by chain, will not hit the same enemy again.
    public bool isOrbitSkill, rotateSkill, returnSkill, isThrowWeapon, isHoming;
    public Vector3 startingPos;
    public float currentRange;

    public void SetStats(List<float> damages, float travelSpeed, int pierce, int chain, float despawnTime, List<float> ailmentsChance, List<float> ailmentsEffect)
    {
        this.damages = damages;
        this.travelSpeed = travelSpeed;
        this.pierce = pierce;
        this.chain = chain;
        this.despawnTime = despawnTime;
        this.ailmentsChance = ailmentsChance;
        this.ailmentsEffect = ailmentsEffect;
        if (isThrowWeapon)
        {
            this.travelSpeed = 8;
        }
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
                if (travelSpeed <= 0) //skills that doesn't travel will despawn
                {
                    despawnTime -= Time.deltaTime;
                    if (despawnTime <= 0f)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else //skills that travel will check the distance traveled.
                {
                    currentRange = Vector3.Distance(transform.position, startingPos);
                    if (currentRange >= skillController.travelRange)
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

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
        if ((direction.normalized.x < 0 && transform.localScale.y > 0) || (direction.normalized.x > 0 && transform.localScale.y < 0))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
        }
    }

    public void DoDamage(EnemyStats enemy, float damagePercent)
    {
        totalDamage = damages.Sum() * (damagePercent / 100);
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
                skillController.player.Heal(skillController.lifeSteal);
                GameManager.totalLifeStealProc++;
                if (skillController.lifeSteal + skillController.player.currentHealth > skillController.player.maxHealth)
                    GameManager.totalLifeSteal += skillController.player.maxHealth - skillController.player.currentHealth;
                else
                    GameManager.totalLifeSteal += skillController.lifeSteal;
            }
        }
        if (skillController.highestDamageType.Equals(1))    //fire, burn
        {
            if (Random.Range(1, 101) <= ailmentsChance[1])
            {
                if (isCrit)
                    enemy.ApplyBurn(damages[1] * (damagePercent / 100) * (skillController.criticalDamage / 100) * (ailmentsEffect[1] / 100));
                else
                    enemy.ApplyBurn(damages[1] * (damagePercent / 100) * (ailmentsEffect[1] / 100));
            }
        }
        else if (skillController.highestDamageType.Equals(2))   //cold, chill
        {
            if (Random.Range(1, 101) <= ailmentsChance[2])
            {
                enemy.ApplyChill(ailmentsEffect[2]);
            }
        }
        else if (skillController.highestDamageType.Equals(3))   //lightning, shock
        {
            if (Random.Range(1, 101) <= ailmentsChance[3])
            {
                enemy.ApplyShock(ailmentsEffect[3]);
            }
        }
        else //physical, bleed
        {
            if (Random.Range(1, 101) <= ailmentsChance[0])
            {
                if (isCrit)
                    enemy.ApplyBleed(damages[0] * (damagePercent / 100) * (skillController.criticalDamage / 100) * (ailmentsEffect[0] / 100));
                else
                    enemy.ApplyBleed(damages[0] * (damagePercent / 100) * (ailmentsEffect[0] / 100));
            }
        }
        enemy.TakeDamage(totalDamage, isCrit); 
        if (isCrit)
        {
            foreach (InventoryManager.Skill sc in skillController.player.gameplayManager.inventory.skillSlotList) //Check crit trigger skill condition
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
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (isThrowWeapon) //The skills used here cannot be manual.
            {
                if (skillController.useBarrage)
                {
                    hitOnceOnly = true;
                    travelSpeed = 0;
                    despawnTime = 10;
                    Timing.RunCoroutine(skillController.BarrageBehavior(skillController.strike, enemy.transform, transform, this));    //spawn skill on enemy.
                }
                else if (skillController.useScatter)
                {
                    hitOnceOnly = true;
                    travelSpeed = 0;
                    despawnTime = 10;
                    Timing.RunCoroutine(skillController.ScatterBehavior(skillController.strike, enemy.transform, transform, this));
                }
                else if (skillController.useBurst)
                {
                    gameObject.SetActive(false);
                    skillController.BurstBehavior(skillController.strike, enemy.transform, transform);
                }
                else if (skillController.useSpread)
                {
                    gameObject.SetActive(false); 
                    if (skillController.useRandomDirection)
                        skillController.SpreadBehavior(skillController.strike, skillController.maxSpreadAngle, null, transform, false);
                    else if (skillController.useBackwardsDirection)
                    {
                        skillController.SpreadBehavior(skillController.strike - (skillController.strike / 2), skillController.maxSpreadAngle, enemy.transform, transform, false);
                        skillController.SpreadBehavior(skillController.strike / 2, skillController.maxSpreadAngle, enemy.transform, transform, true);
                    }
                    else
                    {
                        skillController.SpreadBehavior(skillController.strike, skillController.maxSpreadAngle, enemy.transform, transform, false);
                    }
                }
                else if (skillController.useLateral)
                {
                    gameObject.SetActive(false);
                    if (skillController.useRandomDirection)
                        skillController.LateralBehavior(skillController.strike, transform.localScale.x - skillController.lateralOffset, null, transform, false);
                    else if (skillController.useBackwardsDirection)
                    {
                        skillController.LateralBehavior(skillController.strike - (skillController.strike / 2), transform.localScale.x - skillController.lateralOffset, enemy.transform, transform, false);
                        skillController.LateralBehavior(skillController.strike / 2, transform.localScale.x - skillController.lateralOffset, enemy.transform, transform, true);
                    }
                    else
                    {
                        skillController.LateralBehavior(skillController.strike, transform.localScale.x - skillController.lateralOffset, enemy.transform, transform, false);
                    }
                }
                else if (skillController.useCircular)
                {
                    gameObject.SetActive(false);
                    skillController.CircularBehavior(skillController.strike, transform);
                }
                else if (skillController.useOnTarget)
                {
                    gameObject.SetActive(false);
                    skillController.OnTargetBehavior(skillController.strike, transform, null);
                }
                return;
            }
            if (returnSkill)
            {
                DoDamage(enemy, 50);
                return;
            }
            if (skillController.useOrbit && isOrbitSkill && gameObject.activeSelf)   //Object is a weapon and using orbit behavior
            {
                despawnTime = skillController.cooldown;
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
        target = null;
        returnSkill = false;
        hitOnceOnly = false;
    }
    private void OnEnable()
    {
        startingPos = transform.position;
    }

}
