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
    public int strike = 1, projectile, pierce, chain;
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
    float shortestDistance, distanceToEnemy;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyStats nearestEnemy;
    public Transform target;
    int counter;    //Used in spread skill
    Vector3 direction;
    float projectileSpreadAngle;
    public bool stopFiring;
    public bool useBarrage, useSpread;
    public bool autoUseSkill;
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
        knockBack = baseKnockBack;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
        currentCooldown = cooldown;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateSkillStats();
        PopulatePool((projectile * strike) * 4);
        InvokeRepeating(nameof(UpdateTarget), 0, 0.10f);    //Repeat looking for target
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!stopFiring)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                if (target == null && autoUseSkill) return;
                if (useBarrage)
                {
                    Timing.RunCoroutine(UseSkillBarrage());
                }
                else if (useSpread)
                {
                    UseSkillSpread();
                }
            }
        }
    }
    //Check for closest enemy to target
    protected virtual void UpdateTarget()
    {
        if (stopFiring) return;
        shortestDistance = Mathf.Infinity;
        nearestEnemy = null;
        for (int i = 0; i < enemyManager.enemyList.Count; i++)
        {
            if (enemyManager.enemyList[i].isActiveAndEnabled)
            {
                distanceToEnemy = Vector3.Distance(transform.position, enemyManager.enemyList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyManager.enemyList[i];
                }

            }
        }
        for (int i = 0; i < enemyManager.rareEnemyList.Count; i++)
        {
            if (enemyManager.rareEnemyList[i].isActiveAndEnabled)
            {
                distanceToEnemy = Vector3.Distance(transform.position, enemyManager.rareEnemyList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyManager.rareEnemyList[i];
                }

            }
        }
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else target = null;
    }

    public IEnumerator<float> UseSkillBarrage()       //Spawn/Activate skill. Projectiles barrages.
    {
        stopFiring = true;
        currentCooldown = cooldown;
        for (int p = 0; p < projectile; p++)    //number of projectiles
        {
            yield return Timing.WaitForSeconds(0.1f);
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 5)
                {
                    PopulatePool(5);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (autoUseSkill) direction = target.position - transform.position;
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
        stopFiring = true;
        currentCooldown = cooldown;
        projectileSpreadAngle = 90 / projectile;
        if (autoUseSkill) direction = target.position - transform.position;
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

    protected virtual void PopulatePool(int spawnAmount)
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
