using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public GameObject prefab;
    public GameObject poolParent;
    public GameplayManager gameplayManager;
    public int level;
    public float baseDamage;    //base stats used in multipliers
    public float baseSpeed;
    public float baseAttackRange;
    public float baseChainRange;
    public float baseCooldown;
    public float baseKnockBack;
    public float baseCriticalChance, baseCriticalDamage;
    public float currentCooldown;
    public float despawnTime;
    public int strike = 1, projectile, pierce, chain;
    public float chillChance, burnChance, shockChance, bleedChance;
    public float chillSlow, burnDamage, shockDamage, bleedDamage;
    public float damage;
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
    public EnemyController enemyController;
    public FloatingTextController floatingTextController;
    public EnemyStats nearestEnemy;
    public Transform target;
    int counter;    //Used in spread skill
    Vector3 direction;
    float projectileSpreadAngle;
    public bool stopFiring;
    public bool useBarrage, useSpread;
    public bool autoUseSkill;

    private void Awake()
    {
        damage = baseDamage;
        speed = baseSpeed;
        attackRange = baseAttackRange;
        chainRange = baseChainRange;
        cooldown = baseCooldown;
        knockBack = baseKnockBack;
        currentCooldown = cooldown;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        PopulatePool((projectile * strike) * 4);
        InvokeRepeating(nameof(UpdateTarget), 0, 0.15f);    //Repeat looking for target
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
                    StartCoroutine(UseSkillBarrage());
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
        for (int i = 0; i < enemyController.enemyList.Count; i++)
        {
            if (enemyController.enemyList[i].isActiveAndEnabled)
            {
                distanceToEnemy = Vector3.Distance(transform.position, enemyController.enemyList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyController.enemyList[i];
                }

            }
        }
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else target = null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public IEnumerator UseSkillBarrage()       //Spawn/Activate skill. Projectiles barrages.
    {
        stopFiring = true;
        currentCooldown = cooldown;
        for (int p = 0; p < projectile; p++)    //number of projectiles
        {
            yield return new WaitForSeconds(0.1f);
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
                    poolList[i].SetStats(damage, speed, pierce, chain, despawnTime);
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
                    poolList[i].SetStats(damage, speed, pierce, chain, despawnTime);
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
            GameObject skill = Instantiate(prefab, poolParent.transform);    //Spawn, add to list, and initialize prefabs
            skill.SetActive(false);
            SkillBehavior sb = skill.GetComponent<SkillBehavior>();
            sb.skillController = this;
            sb.SetStats(damage, speed, pierce, chain, despawnTime);
            poolList.Add(sb);
        }
    }

    public void UpdateSkillStats()
    {
        damage = baseDamage * gameplayManager.damageMultiplier;
        speed = baseSpeed * gameplayManager.speedMultiplier;
        attackRange = baseAttackRange * gameplayManager.attackRangeMultiplier;
        chainRange = baseChainRange * gameplayManager.chainRangeMultiplier;
        cooldown = baseCooldown * gameplayManager.cooldownMultiplier;
        knockBack = baseKnockBack * gameplayManager.knockBackMultiplier;
    }
}
