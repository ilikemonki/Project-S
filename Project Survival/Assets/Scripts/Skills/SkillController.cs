using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class SkillController : MonoBehaviour
{
    public SkillBehavior prefab;
    public SkillBehavior weaponPrefab;
    public GameObject poolParent, orbitParent;
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
    public List<SkillBehavior> orbitPoolList = new();
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    int counter;    //Used in spread skill
    Vector3 direction;
    float spreadAngle;
    public bool stopFiring;
    public int highestDamageType;
    //Behaviors 
    public bool autoUseSkill;
    public bool isMelee;
    public bool useCloseCombat;    //close Combat spawns on enemies, is always automatic unless behavior is changed.
    public bool useBarrage, useSpread;
    public bool useOrbit; //targetless
    public bool useCircular; //targetless
    public bool useScatter;
    public bool useLateral;
    public bool useRandomDirection; //targetless, is automatic, cannot be manual. Turn off autoUseSkill.
    public bool useBackwardsDirection; //Shoots from behind
    public bool useReturnDirection; //projectiles only.
    public bool targetless; //Keeps using it's skill regardless of enemies.
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
        if (useOrbit || useRandomDirection || useCircular || !autoUseSkill)
            targetless = true;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateSkillStats();
        if (isMelee && useOrbit) //orbit melee, spawn orbiting weapons
        {
            PopulatePool(strike, weaponPrefab, orbitParent, orbitPoolList);
            OrbitBehavior(strike, orbitParent.transform, orbitPoolList);
        } 
        else if (!isMelee && useOrbit) //orbiting projectile
        {
            PopulatePool(projectile, prefab, orbitParent, orbitPoolList);
            OrbitBehavior(projectile, orbitParent.transform, orbitPoolList);
        }
        PopulatePool(projectile + strike, prefab, poolParent, poolList);
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
                stopFiring = true;
                currentCooldown = cooldown; 
                if (useBarrage)
                {
                    if (useRandomDirection || targetless)
                        Timing.RunCoroutine(BarrageBehavior(strike + projectile, null, transform));
                    else
                        Timing.RunCoroutine(BarrageBehavior(strike + projectile, enemyDistances.closestEnemyList[0].transform, transform));
                }
                else if (useScatter)
                {
                    if (useRandomDirection || targetless)
                        Timing.RunCoroutine(ScatterBehavior(strike + projectile, null, transform));
                    else
                        Timing.RunCoroutine(ScatterBehavior(strike + projectile, enemyDistances.closestEnemyList[0].transform, transform));
                }
                else if (useSpread)
                {
                    if (useRandomDirection)
                        SpreadBehavior(strike + projectile, 90, null, transform, false);
                    else if (useBackwardsDirection)
                    {
                        if (targetless)
                        {
                            SpreadBehavior((strike + projectile) - ((strike + projectile) / 2), 90, null, transform, false);
                            SpreadBehavior((strike + projectile) / 2, 90, null, transform, true);
                        }
                        else
                        {
                            SpreadBehavior((strike + projectile) - ((strike + projectile) / 2), 90, enemyDistances.closestEnemyList[0].transform, transform, false);
                            SpreadBehavior((strike + projectile) / 2, 90, enemyDistances.closestEnemyList[0].transform, transform, true);
                        }
                    }
                    else
                    {
                        if (targetless)
                            SpreadBehavior(strike + projectile, 90, null, transform, false);
                        else
                            SpreadBehavior(strike + projectile, 90, enemyDistances.closestEnemyList[0].transform, transform, false);
                    }
                }
                else if (useLateral)
                {
                    if (useRandomDirection)
                        LateralBehavior(strike + projectile, 0.5f, null, transform, false);
                    else if (useBackwardsDirection)
                    {
                        if (targetless)
                        {
                            LateralBehavior((strike + projectile) - ((strike + projectile) / 2), 0.5f, null, transform, false);
                            LateralBehavior((strike + projectile) / 2, 0.5f, null, transform, true);
                        }
                        else
                        {
                            LateralBehavior((strike + projectile) - ((strike + projectile) / 2), 0.5f, enemyDistances.closestEnemyList[0].transform, transform, false);
                            LateralBehavior((strike + projectile) / 2, 0.5f, enemyDistances.closestEnemyList[0].transform, transform, true);
                        }
                    }
                    else
                    {
                        if (targetless)
                            LateralBehavior(strike + projectile, 0.5f, null, transform, false);
                        else
                            LateralBehavior(strike + projectile, 0.5f, enemyDistances.closestEnemyList[0].transform, transform, false);
                    }
                }
                else if (useCircular && !useCloseCombat)
                {
                    CircleBehavior(strike + projectile, transform);
                }
                else if (useCloseCombat)
                {
                    CloseCombatMelee(strike, transform, enemyDistances.closestEnemyList);
                }
            }
        }
    }
    public IEnumerator<float> BarrageBehavior(int numOfAttacks, Transform target, Transform spawnPos)       //Spawn/Activate skill. Projectiles barrages.
    {
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
                    if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    else if (autoUseSkill) direction = target.position - spawnPos.position;
                    else direction = gameplayManager.mousePos - spawnPos.position;
                    poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-direction).normalized);   //Set direction
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
    }
    public IEnumerator<float> ScatterBehavior(int numOfAttacks, Transform target, Transform spawnPos)       //Spawn/Activate skill. Projectiles barrages.
    {
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
                    if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    else if (autoUseSkill) direction = target.position - spawnPos.position;
                    else direction = gameplayManager.mousePos - spawnPos.position;
                    poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.forward) * direction).normalized);
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
    public void CircleBehavior(int numOfAttacks, Transform spawnPos)
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
        else if(autoUseSkill) direction = target.position - spawnPos.position;
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
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
                    poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    if (p == 0)
                    {
                        poolList[i].SetDirection((direction).normalized);   //Set direction
                    }
                    else if(p % 2 != 0) //shoots up angle
                    {
                        poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * counter, Vector3.forward) * direction).normalized);
                    }
                    else //shoots down angle
                    {
                        poolList[i].SetDirection((Quaternion.AngleAxis(-spreadAngle * counter, Vector3.forward) * direction).normalized);
                    }
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
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
        else if (autoUseSkill) direction = target.position - spawnPos.position;
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
                    poolList[i].SetStats(damages, speed, pierce, chain, despawnTime, ailmentsChance, ailmentsEffect);
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
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void CloseCombatMelee(int numOfAttacks, Transform spawnPos, List<EnemyStats> closestEnemyList)       //Spawn on closest enemies. Melee.
    {
        for (int p = 0; p < numOfAttacks; p++)
        {
            if(!targetless)
            {
                if (p >= closestEnemyList.Count) break;
            }
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefab, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useRandomDirection)
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        poolList[i].transform.position = new Vector3(spawnPos.position.x + Random.Range(-attackRange, attackRange), spawnPos.position.y + Random.Range(-attackRange, attackRange), 0);
                    } 
                    else if (useCircular)
                    {
                        spreadAngle = 360 / numOfAttacks;
                        if (autoUseSkill)
                        {
                            poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right * attackRange;
                            direction = transform.position - spawnPos.position;
                        }
                        else
                        {
                            direction = gameplayManager.mousePos - spawnPos.position;
                            poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * p, Vector3.forward) * Vector3.right * attackRange;
                        }
                    }
                    else //target enemy
                    {
                        direction = closestEnemyList[p].transform.position - spawnPos.position;
                        if (direction.magnitude <= attackRange)
                            poolList[i].transform.position = closestEnemyList[p].transform.position;
                        else
                        {
                            stopFiring = false;
                            return;
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
                    damages[i] = baseDamages[i] * (1 + gameplayManager.damageMultiplier / 100) * (1 + gameplayManager.damageTypeMultiplier[i] / 100) * (1 - gameplayManager.resistances[i] / 100) * (1 + gameplayManager.meleeDamageMultiplier / 100);
                else
                    damages[i] = baseDamages[i] * (1 + gameplayManager.damageMultiplier / 100) * (1 + gameplayManager.damageTypeMultiplier[i] / 100) * (1 - gameplayManager.resistances[i] / 100) * (1 + gameplayManager.projectileDamageMultiplier / 100);
            }
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
