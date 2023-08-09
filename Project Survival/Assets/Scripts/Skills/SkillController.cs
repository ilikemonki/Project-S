using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class SkillController : MonoBehaviour
{
    public SkillBehavior prefab;
    public SkillBehavior meleeWeaponPrefab;
    public GameObject poolParent, orbitParent;
    public GameplayManager gameplayManager;
    public List<SkillBehavior> poolList = new();
    public List<SkillBehavior> orbitPoolList = new();
    List<EnemyStats> rememberEnemiesList = new();
    public List<EnemyStats> enemiesInRange = new();
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public int level;
    EnemyStats nearestEnemy;
    float shortestDistance, distanceToEnemy;
    [Header("Base Stats")]
    public List<float> baseDamages; //[0]physical,[1]fire,[2]cold,[3]lightning
    public List<float> addedAilmentsChance;
    public List<float> addedAilmentsEffect;
    public float baseSpeed;
    public float baseAttackRange;
    public float baseChainRange;
    public float baseCooldown;
    public float baseKnockBack;
    public float baseCriticalChance, baseCriticalDamage;
    public float baseLifeStealChance, baseLifeSteal;
    public int baseStrike, baseProjectile, basePierce, baseChain;
    [Header("Current Stats")]
    public List<float> damages;
    public List<float> ailmentsChance;
    public List<float> ailmentsEffect;
    public float speed;
    public float attackRange;
    public float chainRange;
    public float cooldown;
    public float knockBack;
    public float criticalChance, criticalDamage;
    public float size;
    public float lifeStealChance, lifeSteal;
    public int strike, projectile, pierce, chain;
    [Header("Other Stats")]
    public float chainSpeed;
    public float currentCooldown;
    public float despawnTime;
    int counter;    //Used in spread skill
    Vector3 direction;
    float spreadAngle;
    public bool stopFiring;
    public int highestDamageType;
    public float maxSpreadAngle, lateralOffset; 
    //Behaviors 
    [Header("Behaviors")]
    public bool autoUseSkill;
    public bool isMelee;
    public bool useCloseCombat;    //close Combat spawns on enemies, is always automatic unless behavior is changed.
    public bool useBarrage, useSpread;
    public bool useOrbit; //targetless
    public bool useCircular; //targetless
    public bool useScatter;
    public bool useLateral;
    public bool useBurst;
    public bool useThrowWeapon;
    public bool useRandomDirection; //targetless, is automatic, cannot be manual. Turn off autoUseSkill.
    public bool useBackwardsDirection; //Shoots from behind
    public bool useReturnDirection; //projectiles only.
    public bool useRandomTargeting;
    public bool targetless; //Keeps using it's skill regardless of enemies.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.white;
    }
    private void Awake()
    {
        damages.AddRange(baseDamages);
        ailmentsChance.AddRange(addedAilmentsChance);
        ailmentsEffect.AddRange(addedAilmentsEffect);
        if (useOrbit || useRandomDirection || (useCircular && !useThrowWeapon) || !autoUseSkill)
            targetless = true;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateSkillStats();
        if (isMelee && useOrbit) //orbit melee, spawn orbiting weapons
        {
            PopulatePool(baseStrike + gameplayManager.strikeAdditive, meleeWeaponPrefab, orbitParent, orbitPoolList);
            OrbitBehavior(baseStrike + gameplayManager.strikeAdditive, orbitParent.transform, orbitPoolList);
        } 
        else if (!isMelee && useOrbit) //orbiting projectile
        {
            PopulatePool(baseProjectile + gameplayManager.projectileAdditive, prefab, orbitParent, orbitPoolList);
            OrbitBehavior(baseProjectile + gameplayManager.projectileAdditive, orbitParent.transform, orbitPoolList);
        }
        else if (useThrowWeapon)
        {
            orbitParent.transform.SetParent(player.transform.parent);
            PopulatePool(baseStrike + gameplayManager.strikeAdditive, meleeWeaponPrefab, orbitParent, orbitPoolList);
        }
        PopulatePool(baseProjectile + gameplayManager.projectileAdditive + baseStrike + gameplayManager.strikeAdditive, prefab, poolParent, poolList);
        UpdateSize();

    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (useOrbit)
        {
            UpdateOrbit();
        }
        else if (!stopFiring && !useOrbit)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                if ((enemyDistances.closestEnemyList.Count <= 0 || enemyDistances.updatingInProgress) && !targetless) return; //if no enemies alive, return.
                if (!targetless)   //Use skill when in attackRange.
                {
                    if (Vector3.Distance(transform.position, enemyDistances.closestEnemyList[0].transform.position) > attackRange)
                    {
                        return;
                    }
                }
                if (useRandomTargeting)
                {
                    GetEnemiesInRangeUnsorted(transform);
                    nearestEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
                }
                else
                {
                    nearestEnemy = enemyDistances.closestEnemyList[0];
                }
                stopFiring = true;
                currentCooldown = cooldown;
                //use skills
                if (useThrowWeapon)
                {
                    if (targetless)
                        ThrowWeaponBehavior(null, transform);
                    else
                        ThrowWeaponBehavior(nearestEnemy.transform, transform);
                }
                else if (useBarrage)
                {
                    if (targetless)
                        Timing.RunCoroutine(BarrageBehavior(strike + projectile, null, transform, null));
                    else
                        Timing.RunCoroutine(BarrageBehavior(strike + projectile, nearestEnemy.transform, transform, null));
                }
                else if (useScatter)
                {
                    if (targetless)
                        Timing.RunCoroutine(ScatterBehavior(strike + projectile, null, transform, null));
                    else
                        Timing.RunCoroutine(ScatterBehavior(strike + projectile, nearestEnemy.transform, transform, null));
                }
                else if (useBurst)
                {
                    if (targetless)
                        BurstBehavior(strike + projectile, null, transform);
                    else
                        BurstBehavior(strike + projectile, nearestEnemy.transform, transform);
                }
                else if (useSpread)
                {
                    if (useRandomDirection)
                        SpreadBehavior(strike + projectile, maxSpreadAngle, null, transform, false);
                    else if (useBackwardsDirection)
                    {
                        if (targetless)
                        {
                            SpreadBehavior((strike + projectile) - ((strike + projectile) / 2), maxSpreadAngle, null, transform, false);
                            SpreadBehavior((strike + projectile) / 2, maxSpreadAngle, null, transform, true);
                        }
                        else
                        {
                            SpreadBehavior((strike + projectile) - ((strike + projectile) / 2), maxSpreadAngle, nearestEnemy.transform, transform, false);
                            SpreadBehavior((strike + projectile) / 2, maxSpreadAngle, nearestEnemy.transform, transform, true);
                        }
                    }
                    else
                    {
                        if (targetless)
                            SpreadBehavior(strike + projectile, maxSpreadAngle, null, transform, false);
                        else
                            SpreadBehavior(strike + projectile, maxSpreadAngle, nearestEnemy.transform, transform, false);
                    }
                }
                else if (useLateral)
                {
                    if (useRandomDirection)
                        LateralBehavior(strike + projectile, poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
                    else if (useBackwardsDirection)
                    {
                        if (targetless)
                        {
                            LateralBehavior((strike + projectile) - ((strike + projectile) / 2), poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
                            LateralBehavior((strike + projectile) / 2, poolList[0].transform.localScale.x - lateralOffset, null, transform, true);
                        }
                        else
                        {
                            LateralBehavior((strike + projectile) - ((strike + projectile) / 2), poolList[0].transform.localScale.x - lateralOffset, nearestEnemy.transform, transform, false);
                            LateralBehavior((strike + projectile) / 2, poolList[0].transform.localScale.x - lateralOffset, nearestEnemy.transform, transform, true);
                        }
                    }
                    else
                    {
                        if (targetless)
                            LateralBehavior(strike + projectile, poolList[0].transform.localScale.x * (1 - lateralOffset), null, transform, false);
                        else
                            LateralBehavior(strike + projectile, poolList[0].transform.localScale.x * (1 - lateralOffset), nearestEnemy.transform, transform, false);
                    }
                }
                else if (useCircular)
                {
                    CircularBehavior(strike + projectile, transform);
                }
                else if (useCloseCombat)
                {
                    CloseCombatMelee(strike, transform, enemyDistances.closestEnemyList);
                }
            }
        }
    }
    public IEnumerator<float> BarrageBehavior(int numOfAttacks, Transform target, Transform spawnPos, SkillBehavior objectToDespawn)       //Spawn/Activate skill. Projectiles barrages.
    {
        Vector3 dir;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useRandomDirection) dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    else if (autoUseSkill || useThrowWeapon) dir = target.position - spawnPos.position;
                    else dir = gameplayManager.mousePos - spawnPos.position;
                    if (useCloseCombat)
                    {
                        if (autoUseSkill || useThrowWeapon)
                            poolList[i].transform.position = target.position;    //set starting position on target
                        else
                        {
                            if (dir.magnitude < attackRange)
                                poolList[i].transform.position = gameplayManager.mousePos;
                            else poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange; ;
                        }
                    }
                    else
                        poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((dir).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg); //set angle
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-dir).normalized);   //Set direction
                    }
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
            if (useBackwardsDirection && p % 2 == 0)
            {
                yield return Timing.WaitForSeconds(0);
            }
            else
                yield return Timing.WaitForSeconds(0.1f);
        }
        stopFiring = false;
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false);
    }
    public IEnumerator<float> ScatterBehavior(int numOfAttacks, Transform target, Transform spawnPos, SkillBehavior objectToDespawn)       //Spawn/Activate skill. Projectiles barrages.
    {
        Vector3 dir;
        if (useRandomDirection) dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill || useThrowWeapon) dir = target.position - spawnPos.position;
        else dir = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useCloseCombat)
                    {
                        if (autoUseSkill || useThrowWeapon)
                            poolList[i].transform.position = target.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);    //set starting position on target
                        else
                        {
                            if (dir.magnitude < attackRange)
                                poolList[i].transform.position = gameplayManager.mousePos + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                            else poolList[i].transform.position = (spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange) + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                        }
                    }
                    else
                        poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.forward) * dir).normalized);
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                    }
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
            if (useBackwardsDirection && p % 2 == 0)
            {
                yield return Timing.WaitForSeconds(0);
            }
            else
                yield return Timing.WaitForSeconds(0.05f);
        }
        stopFiring = false;
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false);
    }
    public void BurstBehavior(int numOfAttacks, Transform target, Transform spawnPos)
    {
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill || useThrowWeapon) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useCloseCombat)
                    {
                        if (autoUseSkill || useThrowWeapon)
                            poolList[i].transform.position = target.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);    //set starting position on target
                        else
                        {
                            if (direction.magnitude < attackRange)
                                poolList[i].transform.position = gameplayManager.mousePos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                            else poolList[i].transform.position = (spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        }
                    }
                    else
                        poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.forward) * direction).normalized);
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                    }
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void UpdateOrbit()
    {
        if (autoUseSkill)
        {
            orbitParent.transform.Rotate(new Vector3(0, 0, -(60 + (speed * 10)) * Time.deltaTime));
        }
        else
        {
            direction = gameplayManager.mousePos - orbitParent.transform.position;
            orbitParent.transform.rotation = Quaternion.RotateTowards(orbitParent.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), (60 + (speed * 10)) * Time.deltaTime);
        }
        for (int i = 0; i < orbitPoolList.Count; i++)
        {
            if (!orbitPoolList[i].isActiveAndEnabled)
            {
                orbitPoolList[i].despawnTime -= Time.deltaTime;
                if (orbitPoolList[i].despawnTime <= 0)
                {
                    orbitPoolList[i].speed = speed;
                    orbitPoolList[i].pierce = pierce;
                    orbitPoolList[i].chain = chain;
                    orbitPoolList[i].gameObject.SetActive(true);
                }
            }
        }
    }
    public void OrbitBehavior(int numOfAttacks, Transform spawnPos, List<SkillBehavior> pList)
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/strikes
        {
            for (int i = 0; i < pList.Count; i++)
            {
                if (!pList[i].isActiveAndEnabled)
                {
                    pList[i].isOrbitSkill = true;
                    pList[i].transform.position = spawnPos.position + Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right * attackRange;
                    pList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    pList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void CircularBehavior(int numOfAttacks, Transform spawnPos)
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/strikes
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useCloseCombat) 
                    {
                        if (autoUseSkill || useThrowWeapon)
                        {
                            direction = transform.position - spawnPos.position; 
                            poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * direction).normalized);
                            poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * p, Vector3.forward) * Vector3.right * attackRange;
                        }
                        else
                        {
                            direction = gameplayManager.mousePos - spawnPos.position; 
                            poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * direction).normalized);
                            poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + (spreadAngle * p), Vector3.forward) * Vector3.right * attackRange;
                        }
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spreadAngle * p);
                    }
                    else
                    {
                        poolList[i].transform.position = spawnPos.position;
                        if (!autoUseSkill)
                        {
                            direction = gameplayManager.mousePos - spawnPos.position;
                            poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * direction).normalized);
                            poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spreadAngle * p);
                        }
                        else
                        {
                            poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right).normalized);   //Set direction
                            poolList[i].transform.eulerAngles = new Vector3(0, 0, spreadAngle * p);
                        }
                    } 
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void SpreadBehavior(int numOfAttacks, float maxAngle, Transform target, Transform spawnPos, bool useBackwards)       //Spawn/Activate skill. Projectiles spread.
    {
        counter = 0;
        spreadAngle = maxAngle / numOfAttacks;
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if(autoUseSkill || useThrowWeapon) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        if (useBackwards)
        {
            direction = -direction;   //Set direction
        }
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles
        {
            if (p % 2 == 1) counter++;
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useCloseCombat && !useOrbit)
                    {
                        if (autoUseSkill || useThrowWeapon)
                        {
                            if (p % 2 == 1)
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * counter, Vector3.forward) * direction).normalized);
                                if (direction.magnitude < 3)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * counter, Vector3.forward) * Vector3.right * 3;
                                else
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * counter, Vector3.forward) * Vector3.right * direction.magnitude;
                                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spreadAngle * counter);
                            }
                            else
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(-spreadAngle * counter, Vector3.forward) * direction).normalized);
                                if (direction.magnitude < 3)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * counter, Vector3.forward) * Vector3.right * 3;
                                else
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * counter, Vector3.forward) * Vector3.right * direction.magnitude;
                                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -spreadAngle * counter);
                            }
                        }
                        else
                        {
                            if (p % 2 == 1)
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * counter, Vector3.forward) * direction).normalized);
                                if (direction.magnitude < 3)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * counter, Vector3.forward) * Vector3.right * 3;
                                else if (direction.magnitude < attackRange)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * counter, Vector3.forward) * Vector3.right * direction.magnitude;
                                else
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * counter, Vector3.forward) * Vector3.right * attackRange;
                                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spreadAngle * counter);
                            }
                            else
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(-spreadAngle * counter, Vector3.forward) * direction).normalized);
                                if (direction.magnitude < 3)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * counter, Vector3.forward) * Vector3.right * 3;
                                else if (direction.magnitude < attackRange)
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * counter, Vector3.forward) * Vector3.right * direction.magnitude;
                                else
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * counter, Vector3.forward) * Vector3.right * attackRange;

                                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -spreadAngle * counter);
                            }
                        }
                    }
                    else
                    {
                        if (useCloseCombat && useOrbit)
                        {
                            direction = target.position - spawnPos.position;
                            poolList[i].transform.position = target.position;    //set starting position on player
                            poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                        }
                        else
                        {
                            if (p == 0)
                            {
                                poolList[i].SetDirection((direction).normalized);   //Set direction
                            }
                            else if (p % 2 == 1) //shoots up angle
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * counter, Vector3.forward) * direction).normalized);
                            }
                            else //shoots down angle
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(-spreadAngle * counter, Vector3.forward) * direction).normalized);
                            }
                            poolList[i].transform.position = spawnPos.position;    //set starting position on player
                            poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
                        }
                    }
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void LateralBehavior(int numOfAttacks, float distanceApart, Transform target, Transform spawnPos, bool useBackwards)       //Spawn/Activate skill. Projectiles spread.
    {
        counter = 0;
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill || useThrowWeapon) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        if (useBackwards)
        {
            direction = -direction;   //Set direction
        }
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles
        {
            if (p % 2 == 1) counter++;
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useCloseCombat)
                    {
                        if (autoUseSkill || useThrowWeapon)
                        {
                            if (p == 0)
                            {
                                poolList[i].transform.position = target.position;
                            }
                            else if (p % 2 != 0) //shoots up angle
                            {
                                poolList[i].transform.position = new Vector3(target.position.x + (direction.normalized.y * (counter * distanceApart)), target.position.y - (direction.normalized.x * (counter * distanceApart)), 0);
                            }
                            else //shoots down angle
                            {
                                poolList[i].transform.position = new Vector3(target.position.x - (direction.normalized.y * (counter * distanceApart)), target.position.y + (direction.normalized.x * (counter * distanceApart)), 0);
                            }
                        }
                        else
                        {
                            if (direction.magnitude < attackRange) //mouse within attack range
                            {
                                if (p == 0)
                                {
                                    poolList[i].transform.position = gameplayManager.mousePos;
                                }
                                else if (p % 2 != 0) //shoots up angle
                                {
                                    poolList[i].transform.position = new Vector3(gameplayManager.mousePos.x + (direction.normalized.y * (counter * distanceApart)), gameplayManager.mousePos.y - (direction.normalized.x * (counter * distanceApart)), 0);
                                }
                                else //shoots down angle
                                {
                                    poolList[i].transform.position = new Vector3(gameplayManager.mousePos.x - (direction.normalized.y * (counter * distanceApart)), gameplayManager.mousePos.y + (direction.normalized.x * (counter * distanceApart)), 0);
                                }
                            }
                            else
                            {
                                if (p == 0)
                                {
                                    poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange;
                                }
                                else if (p % 2 != 0) //shoots up angle
                                {
                                    poolList[i].transform.position = new Vector3((spawnPos.position.x + (Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange).x) + (direction.normalized.y * (counter * distanceApart)),
                                       (spawnPos.position.y + (Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange).y) - (direction.normalized.x * (counter * distanceApart)), 0);
                                }
                                else //shoots down angle
                                {
                                    poolList[i].transform.position = new Vector3((spawnPos.position.x + (Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange).x) - (direction.normalized.y * (counter * distanceApart)),
                                        (spawnPos.position.y + (Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange).y) + (direction.normalized.x * (counter * distanceApart)), 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (p == 0)
                        {
                            poolList[i].transform.position = spawnPos.position;
                        }
                        else if (p % 2 != 0) //shoots up angle
                        {
                            poolList[i].transform.position = new Vector3(spawnPos.position.x + (direction.normalized.y * (counter * distanceApart)), spawnPos.position.y - (direction.normalized.x * (counter * distanceApart)), 0);
                        }
                        else //shoots down angle
                        {
                            poolList[i].transform.position = new Vector3(spawnPos.position.x - (direction.normalized.y * (counter * distanceApart)), spawnPos.position.y + (direction.normalized.x * (counter * distanceApart)), 0);
                        }
                    }
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void CloseCombatMelee(int numOfAttacks, Transform spawnPos, List<EnemyStats> closestEnemyList)       //Spawn on closest enemies. Melee. Null closestEnemyList == doesn't spawn from player
    {
        rememberEnemiesList.Clear();
        if (useRandomTargeting) GetEnemiesInRangeUnsorted(spawnPos);
        for (int p = 0; p < numOfAttacks; p++)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useRandomDirection && !useThrowWeapon)
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        poolList[i].transform.position = new Vector3(spawnPos.position.x + Random.Range(-attackRange, attackRange), spawnPos.position.y + Random.Range(-attackRange, attackRange), 0);
                    } 
                    else if (!autoUseSkill && p == 0 && !useThrowWeapon) //manual and first strike hits at mouse pos.
                    {
                        counter = 1;
                        direction = gameplayManager.mousePos - spawnPos.position;
                        if (direction.magnitude < attackRange)
                            poolList[i].transform.position = gameplayManager.mousePos;
                        else poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange;
                    }
                    else if (useRandomTargeting) //get random enemy from spawnPos
                    {
                        if (enemiesInRange.Count > 0)
                        {
                            nearestEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
                            enemiesInRange.Remove(nearestEnemy);
                            direction = nearestEnemy.transform.position - spawnPos.position;
                            poolList[i].transform.position = nearestEnemy.transform.position;
                        }
                        else
                        {
                            stopFiring = false;
                            return;
                        }
                    }
                    else //target enemy
                    {
                        if (closestEnemyList == null)   //skill used from other than player
                        {
                            nearestEnemy = FindNearestEnemy(spawnPos);
                            if (nearestEnemy != null)
                            {
                                rememberEnemiesList.Add(nearestEnemy);
                                direction = nearestEnemy.transform.position - spawnPos.position;
                                poolList[i].transform.position = nearestEnemy.transform.position;
                            }
                            else
                            {
                                stopFiring = false;
                                return;
                            }
                        }
                        else //skill used from player
                        {
                            if (closestEnemyList.Count <= 0)
                            {
                                stopFiring = false;
                                return;
                            }
                            direction = closestEnemyList[p - counter].transform.position - spawnPos.position;
                            if (direction.magnitude <= attackRange)
                                poolList[i].transform.position = closestEnemyList[p - counter].transform.position;
                            else
                            {
                                stopFiring = false;
                                return;
                            }
                        }
                    }
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void ThrowWeaponBehavior(Transform target, Transform spawnPos)       //Spawn/Activate skill. Projectiles barrages.
    {
        for (int i = 0; i < orbitPoolList.Count; i++)
        {
            if (i > orbitPoolList.Count - 2)
            {
                PopulatePool(2, meleeWeaponPrefab, orbitParent, orbitPoolList);
            }
            if (!orbitPoolList[i].isActiveAndEnabled)
            {
                if (autoUseSkill) direction = target.position - spawnPos.position;
                else direction = gameplayManager.mousePos - spawnPos.position;
                orbitPoolList[i].transform.position = spawnPos.position;    //set starting position on player
                orbitPoolList[i].isThrowWeapon = true;
                orbitPoolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                orbitPoolList[i].SetDirection((direction).normalized);   //Set direction
                orbitPoolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                orbitPoolList[i].gameObject.SetActive(true);
                break;
            }
        }
        stopFiring = false;
    }
    public void SpawnChainProjectile(List<EnemyStats> chainList, Transform target, Transform spawnPos)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(projectile, prefab, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                direction = target.position - spawnPos.position; 
                poolList[i].transform.position = spawnPos.position;
                poolList[i].enemyChainList.AddRange(chainList);
                poolList[i].target = target;
                poolList[i].SetStats(damages, chainSpeed, 0, chain - 1, despawnTime, ailmentsChance, ailmentsEffect);
                poolList[i].SetDirection((direction).normalized);   //Set direction
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                poolList[i].gameObject.SetActive(true);
                return;
            }
        }
    }
    public EnemyStats FindNearestEnemy(Transform spawnPos)
    {
        nearestEnemy = null;
        shortestDistance = Mathf.Infinity;
        for (int e = 0; e < enemyManager.enemyList.Count; e++)  //find target in normal enemy list
        {
            if (enemyManager.enemyList[e].isActiveAndEnabled && !rememberEnemiesList.Contains(enemyManager.enemyList[e]))
            {
                distanceToEnemy = Vector3.Distance(spawnPos.position, enemyManager.enemyList[e].transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyManager.enemyList[e];
                }
            }
        }
        for (int e = 0; e < enemyManager.rareEnemyList.Count; e++)
        {
            if (enemyManager.rareEnemyList[e].isActiveAndEnabled && !rememberEnemiesList.Contains(enemyManager.rareEnemyList[e]))
            {
                distanceToEnemy = Vector3.Distance(spawnPos.position, enemyManager.rareEnemyList[e].transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy <= attackRange)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyManager.rareEnemyList[e];
                }
            }
        }
        return nearestEnemy;
    }
    public void GetEnemiesInRangeUnsorted(Transform spawnPos)
    {
        enemiesInRange.Clear();
        for (int e = 0; e < enemyManager.enemyList.Count; e++)  //find target in normal enemy list
        {
            if (enemyManager.enemyList[e].isActiveAndEnabled && !enemiesInRange.Contains(enemyManager.enemyList[e]))
            {
                distanceToEnemy = Vector3.Distance(spawnPos.position, enemyManager.enemyList[e].transform.position);
                if (distanceToEnemy <= attackRange)
                {
                    enemiesInRange.Add(enemyManager.enemyList[e]);
                }
            }
        }
        for (int e = 0; e < enemyManager.rareEnemyList.Count; e++)
        {
            if (enemyManager.rareEnemyList[e].isActiveAndEnabled && !enemiesInRange.Contains(enemyManager.rareEnemyList[e]))
            {
                distanceToEnemy = Vector3.Distance(spawnPos.position, enemyManager.rareEnemyList[e].transform.position);
                if (distanceToEnemy <= attackRange)
                {
                    enemiesInRange.Add(enemyManager.rareEnemyList[e]);
                }
            }
        }
    }
    public virtual void PopulatePool(int spawnAmount, SkillBehavior prefab, GameObject parent, List<SkillBehavior> poolList)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SkillBehavior skill = Instantiate(prefab, parent.transform);    //Spawn, add to list, and initialize prefabs
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
            if (baseDamages[i] > 0)
            {
                if (isMelee)
                    damages[i] = (baseDamages[i] * (1 + (gameplayManager.damageMultiplier + gameplayManager.damageTypeMultiplier[i] + gameplayManager.meleeDamageMultiplier) / 100)) * (1 - gameplayManager.resistances[i] / 100);
                else
                    damages[i] = (baseDamages[i] * (1 + (gameplayManager.damageMultiplier + gameplayManager.damageTypeMultiplier[i] + gameplayManager.projectileDamageMultiplier) / 100)) * (1 - gameplayManager.resistances[i] / 100);
            }
        }

        for (int i = 0; i < ailmentsChance.Count; i++)
        {
            ailmentsChance[i] = addedAilmentsChance[i] + gameplayManager.ailmentsChanceAdditive[i];
        }
        for (int i = 0; i < ailmentsEffect.Count; i++)
        {
            ailmentsEffect[i] = addedAilmentsEffect[i] + gameplayManager.ailmentsEffectAdditive[i];
        }
        if (isMelee)   //is melee
        {
            speed = baseSpeed;
            strike = baseStrike + gameplayManager.strikeAdditive;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.meleeAttackRangeMultiplier) / 100);
            cooldown = baseCooldown * (1 + (gameplayManager.cooldownMultiplier + gameplayManager.meleeCooldownMultiplier) / 100);
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.meleeCriticalChanceAdditive;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.meleeCriticalDamageAdditive;
            size = gameplayManager.sizeAdditive + gameplayManager.meleeSizeAdditive;
        }
        else //is projectile
        {
            speed = baseSpeed * (1 + gameplayManager.projectileSpeedMultiplier / 100);
            projectile = baseProjectile + gameplayManager.projectileAdditive;
            chain = baseChain + gameplayManager.chainAdditive;
            pierce = basePierce + gameplayManager.pierceAdditive;
            chainRange = baseChainRange * (1 + gameplayManager.chainRangeMultiplier / 100);
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.projectileAttackRangeMultiplier) / 100);
            cooldown = baseCooldown * (1 + (gameplayManager.cooldownMultiplier + gameplayManager.projectileCooldownMultiplier) / 100);
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.projectileCriticalChanceAdditive;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.projectileCriticalDamageAdditive;
            size = gameplayManager.sizeAdditive + gameplayManager.projectileAdditive;
        }
        if (gameplayManager.maxAttackRange < attackRange)
        {
            gameplayManager.maxAttackRange = attackRange;
        }
        lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive;
        lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive;
        currentCooldown = cooldown;
        knockBack = baseKnockBack;
        highestDamageType = damages.IndexOf(Mathf.Max(damages.ToArray()));  //Find highest damage type.
        UpdateSize();
    }
    public void UpdateSize()
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            poolList[i].transform.localScale = new Vector3(poolList[i].transform.localScale.x * (1 + size / 100), poolList[i].transform.localScale.y * (1 + size / 100), 1);
        }
        for (int i = 0; i < orbitPoolList.Count; i++)
        {
            orbitPoolList[i].transform.localScale = new Vector3(orbitPoolList[i].transform.localScale.x * (1 + size / 100), orbitPoolList[i].transform.localScale.y * (1 + size / 100), 1);
        }
    }
}
