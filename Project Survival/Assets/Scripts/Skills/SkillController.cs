using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class SkillController : MonoBehaviour
{
    public SkillBehavior prefab;
    public GameObject poolParent;
    public GameplayManager gameplayManager;
    public int level;
    public List<float> baseDamages; //[0]physical,[1]fire,[2]cold,[3]lightning
    public List<float> baseAilmentsChance;
    public List<float> baseAilmentsEffect;
    public float baseSpeed;
    public float baseAttackRange;
    public float baseChainRange;
    public float baseCooldown;
    public float baseKnockBack;
    public float baseCriticalChance, baseCriticalDamage;
    public float currentCooldown;
    public float despawnTime;
    public int strike, projectile, pierce, chain;
    public List<float> damages;
    public List<float> ailmentsChance;
    public List<float> ailmentsEffect;
    public float speed;
    public float attackRange;
    public float chainRange;
    public float cooldown;
    public float knockBack;
    public float criticalChance, criticalDamage;
    public float chainSpeed;
    public List<SkillBehavior> poolList = new();
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    //Transform target;
    int counter;    //Used in spread skill
    Vector3 direction;
    float projectileSpreadAngle;
    public bool stopFiring;
    public bool useBarrage, useSpread, useMelee;
    public bool autoUseSkill, skillSpawnsOnEnemy;
    public int highestDamageType;
    private void Awake()
    {
        damages = baseDamages;
        ailmentsChance = baseAilmentsChance;
        ailmentsEffect = baseAilmentsEffect;
        speed = baseSpeed;
        attackRange = baseAttackRange;
        chainRange = baseChainRange;
        cooldown = baseCooldown;
        currentCooldown = cooldown;
        knockBack = baseKnockBack;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateSkillStats();
        PopulatePool(10);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!stopFiring)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                if (enemyDistances.closestEnemyList.Count <= 0 || enemyDistances.updatingInProgress) return; //if no enemies alive, return.
                if (autoUseSkill)
                {
                    direction = enemyDistances.closestEnemyList[0].transform.position - transform.position;
                    if (direction.magnitude > attackRange)
                    {
                        return;
                    }
                }
                stopFiring = true;
                currentCooldown = cooldown;
                if (useBarrage)
                {
                    Timing.RunCoroutine(UseSkillBarrage());
                }
                else if (useSpread)
                {
                    UseSkillSpread();
                }
                else if (useMelee)
                {
                    UseSkillMelee();
                }
            }
        }
    }
    public IEnumerator<float> UseSkillBarrage()       //Spawn/Activate skill. Projectiles barrages.
    {
        for (int p = 0; p < projectile; p++)    //number of projectiles
        {
            yield return Timing.WaitForSeconds(0.1f);
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 5)
                {
                    PopulatePool(projectile);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (autoUseSkill) direction = enemyDistances.closestEnemyList[0].transform.position - transform.position;
                    else direction = gameplayManager.mousePos - transform.position;

                    poolList[i].transform.position = transform.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void UseSkillSpread()       //Spawn/Activate skill. Projectiles spread.
    {
        counter = 0;
        projectileSpreadAngle = 90 / projectile;
        if (autoUseSkill) direction = enemyDistances.closestEnemyList[0].transform.position - transform.position;
        else direction = gameplayManager.mousePos - transform.position;
        for (int p = 0; p < projectile; p++)    //number of projectiles
        {
            if (p != 0) counter++;
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 5)
                {
                    PopulatePool(projectile);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].transform.position = transform.position;    //set starting position on player
                    if (p == 0)
                    {
                        poolList[i].SetDirection((direction).normalized);   //Set direction
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                    }
                    else if(p % 2 != 0) //shoots up angle
                    {
                        counter--;
                        poolList[i].SetDirection((Quaternion.AngleAxis(projectileSpreadAngle * (p - counter), Vector3.forward) * direction).normalized);
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + projectileSpreadAngle * p);
                    }
                    else //shoots down angle
                    {
                        poolList[i].SetDirection((Quaternion.AngleAxis(-projectileSpreadAngle * (p - counter), Vector3.forward) * direction).normalized);
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -projectileSpreadAngle * (p - 1));
                    }
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void UseSkillMelee()       //Spawn/Activate skill. Melee.
    {
        //counter = 0;
        //projectileSpreadAngle = 90 / strike;
        //if (autoUseSkill) direction = target.position - transform.position;
        //else direction = gameplayManager.mousePos - transform.position;
        //for (int p = 0; p < strike; p++)    //number of strikes
        //{
        //    if (p != 0) counter++;
        //    for (int i = 0; i < poolList.Count; i++)
        //    {
        //        if (i > poolList.Count - 5)
        //        {
        //            PopulatePool(strike);
        //        }
        //        if (!poolList[i].isActiveAndEnabled)
        //        {
        //            poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
        //            if (autoUseSkill) poolList[i].transform.position = target.position;    //set starting position on player
        //            else
        //            {
        //                if (direction.magnitude < attackRange)
        //                {
        //                    poolList[i].transform.position = transform.position + (direction.normalized * direction.magnitude);
        //                }
        //                else poolList[i].transform.position = transform.position + (direction.normalized * attackRange);
        //            }
        //            if (p == 0)
        //            {
        //                poolList[i].SetDirection((direction).normalized);   //Set direction
        //                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
        //            }
        //            else if (p % 2 != 0) //shoots up angle
        //            {
        //                counter--;
        //                poolList[i].SetDirection((Quaternion.AngleAxis(projectileSpreadAngle * (p - counter), Vector3.forward) * direction).normalized);
        //                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + projectileSpreadAngle * p);
        //            }
        //            else //shoots down angle
        //            {
        //                poolList[i].SetDirection((Quaternion.AngleAxis(-projectileSpreadAngle * (p - counter), Vector3.forward) * direction).normalized);
        //                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -projectileSpreadAngle * (p - 1));
        //            }
        //            poolList[i].gameObject.SetActive(true);
        //            break;
        //        }
        //    }
        //}
        //stopFiring = false;

        if (skillSpawnsOnEnemy)
        {
            for (int t = 0; t < strike; t++)
            {
                if (t >= enemyDistances.closestEnemyList.Count) break;
                for (int i = 0; i < poolList.Count; i++)
                {
                    if (i > poolList.Count - 5)
                    {
                        PopulatePool(strike);
                    }
                    if (!poolList[i].isActiveAndEnabled)
                    {
                        direction = enemyDistances.closestEnemyList[t].transform.position - transform.position;
                        if (direction.magnitude <= attackRange)
                            poolList[i].transform.position = enemyDistances.closestEnemyList[t].transform.position;
                        else
                        {
                            stopFiring = false;
                            return;
                        }
                        poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                        poolList[i].SetDirection((direction).normalized);   //Set direction
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                        poolList[i].gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        stopFiring = false;
    }

    public virtual void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SkillBehavior skill = Instantiate(prefab, poolParent.transform);    //Spawn, add to list, and initialize prefabs
            skill.gameObject.SetActive(false);
            skill.skillController = this;
            skill.SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
            poolList.Add(skill);
        }
    }

    public void UpdateSkillStats()
    {
        for (int i = 0; i < damages.Count; i++)
        {
            damages[i] = baseDamages[i] * (1 - gameplayManager.resistances[i] / 100);
        }
        for (int i = 0; i < ailmentsChance.Count; i++)
        {
            ailmentsChance[i] = baseAilmentsChance[i] + gameplayManager.ailmentsChanceAdditive[i];
        }
        for (int i = 0; i < ailmentsEffect.Count; i++)
        {
            ailmentsEffect[i] = baseAilmentsEffect[i] + gameplayManager.ailmentsEffectAdditive[i];
        }
        speed = baseSpeed * (1 + gameplayManager.speedMultiplier / 100);
        attackRange = baseAttackRange * (1 + gameplayManager.attackRangeMultiplier / 100);
        chainRange = baseChainRange * (1 + gameplayManager.chainRangeMultiplier / 100);
        cooldown = baseCooldown * (1 + gameplayManager.cooldownMultiplier / 100);
        //knockBack = baseKnockBack * (1 + gameplayManager.knockBackMultiplier / 100);
        criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive;
        criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive;
        highestDamageType = damages.IndexOf(Mathf.Max(damages.ToArray()));  //Find highest damage type.
    }
}
