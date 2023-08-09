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
    public float speed;
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
    public bool isOrbitSkill, rotateSkill, returnSkill, isThrowWeapon;

    public void SetStats(List<float> damages, float speed, int pierce, int chain, float despawnTime, List<float> ailmentsChance, List<float> ailmentsEffect)
    {
        this.damages = damages;
        this.speed = speed;
        this.pierce = pierce;
        this.chain = chain;
        this.despawnTime = despawnTime;
        this.ailmentsChance = ailmentsChance;
        this.ailmentsEffect = ailmentsEffect;
        if (isThrowWeapon)
        {
            this.speed = 8;
            this.despawnTime = 1;
        }
    }

    protected virtual void Update()
    {
        if (rotateSkill && speed > 0)
        {
            transform.Rotate(new Vector3(0, 0, 160 + (speed * 8)) * Time.deltaTime);
        }
        if (target != null && target.gameObject.activeSelf)     //Have skill keep following target and change its angle.
        {
            direction = (target.position - transform.position).normalized;
            if (!rotateSkill)
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        if (!isOrbitSkill)
        {
            if (!returnSkill)
            {
                despawnTime -= Time.deltaTime;
                if (despawnTime <= 0f)
                {
                    if (skillController.useReturnDirection)
                    {
                        returnSkill = true;
                        target = skillController.player.transform;
                        speed = skillController.chainSpeed;
                    }
                    else
                        gameObject.SetActive(false);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (speed > 0 && !isOrbitSkill || !skillController.useCloseCombat && !isOrbitSkill)
        {
            rb.MovePosition(transform.position + (speed * Time.fixedDeltaTime * direction));
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
            skillController.player.Heal(skillController.lifeSteal);
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
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (hitOnceOnly) return;
        hitOnceOnly = true;
        if (col.CompareTag("Enemy") || col.CompareTag("Rare Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (isThrowWeapon) //The skills used here cannot be manual.
            {
                if (skillController.useBarrage)
                {
                    speed = 0;
                    despawnTime = 10;
                    Timing.RunCoroutine(skillController.BarrageBehavior(skillController.strike, enemy.transform, transform, this));    //spawn skill on enemy.
                }
                else if (skillController.useScatter)
                {
                    speed = 0;
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
                else if (skillController.useCloseCombat)
                {
                    gameObject.SetActive(false);
                    skillController.CloseCombatMelee(skillController.strike, transform, null);
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
                    if (skillController.useCloseCombat)
                        skillController.SpreadBehavior(1, 0, enemy.transform, transform, false);
                        //Timing.RunCoroutine(skillController.BarrageBehavior(1, enemy.transform, enemy.transform, null));    //spawn skill on enemy.
                    else
                        skillController.SpreadBehavior(1, 0, enemy.transform, transform, false);
                        //Timing.RunCoroutine(skillController.BarrageBehavior(1, enemy.transform, transform, null));    //spawn skill on weapon.
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
        hitOnceOnly = false;
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
                speed = skillController.chainSpeed;
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
        speed = skillController.chainSpeed;
        target = FindTarget(true);
        if (target != null)
        {
            despawnTime = skillController.despawnTime;
            SetDirection((target.position - transform.position).normalized); //if target is found, set direction
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else
        {
            if (skillController.useReturnDirection)
            {
                returnSkill = true;
                target = skillController.player.transform;
                speed = skillController.chainSpeed;
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
                        if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.chainRange)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = skillController.enemyManager.enemyList[i];
                        }
                    }
                    else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.chainRange)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyManager.enemyList[i];
                    }
                }
            }
        }
        for (int i = 0; i < skillController.enemyManager.rareEnemyList.Count; i++)  //find target in rare enemy list
        {
            bool dontChain = false;
            if (skillController.enemyManager.rareEnemyList[i].isActiveAndEnabled)
            {
                for (int j = 0; j < enemyChainList.Count; j++)
                {
                    if (enemyChainList.Contains(skillController.enemyManager.rareEnemyList[i]))
                    {
                        dontChain = true;
                        break;
                    }
                }
                if (!dontChain)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyManager.rareEnemyList[i].transform.position); 
                    if (!closest)
                    {
                        if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.chainRange)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = skillController.enemyManager.rareEnemyList[i];
                        }
                    }
                    else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.chainRange)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyManager.rareEnemyList[i];
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

}
