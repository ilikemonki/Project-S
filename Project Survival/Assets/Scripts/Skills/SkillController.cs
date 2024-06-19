using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class SkillController : MonoBehaviour
{
    public SkillBehavior prefabBehavior;
    public SkillBehavior meleeWeaponPrefab;
    public ItemDescription skillOrbDescription;
    public GameObject poolParent, orbitParent;
    public GameplayManager gameplayManager;
    public List<SkillBehavior> poolList = new();
    public List<SkillBehavior> orbitPoolList = new();
    List<EnemyStats> rememberEnemiesList = new();
    public List<EnemyStats> enemiesInRange = new();
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyDistances enemyDistances;
    public Upgrades levelUpgrades;
    public int level;
    public float exp;
    EnemyStats nearestEnemy;
    float shortestDistance, distanceToEnemy;
    [Header("Base Stats")]
    public List<float> baseDamageTypes; //[0]physical,[1]fire,[2]cold,[3]lightning
    public List<float> baseAilmentsChance;
    public List<float> baseAilmentsEffect;
    public float baseTravelSpeed;
    public float baseAttackRange;
    public float baseTravelRange;
    public float baseDuration;
    public float baseCooldown;
    public float baseKnockBack;
    public float baseCriticalChance, baseCriticalDamage;
    public float baseLifeStealChance, baseLifeSteal;
    public int baseStrike, baseCombo, baseProjectile, basePierce, baseChain;
    public float despawnTime; //skills that don't travel or have duration will have despawnTime
    [Header("Current Stats")]
    public List<float> damageTypes;
    public List<float> ailmentsChance;
    public List<float> ailmentsEffect;
    public float damage;
    public float travelSpeed;
    public float attackRange;
    public float travelRange;
    public float duration;
    public float cooldown;
    public float knockBack;
    public float criticalChance, criticalDamage;
    public float size;
    public float lifeStealChance, lifeSteal;
    public int strike, combo, projectile, pierce, chain;
    [Header("Other Stats")]
    public float currentCooldown;
    int counter;    //Used in spread skill
    Vector3 direction;
    public float  comboCounter, comboMultiplier;
    public bool stopFiring;
    public int highestDamageType;
    public float spreadAngle, maxSpreadAngle, lateralOffset;
    public bool targetless; //Keeps using it's skill regardless of enemies. If false, requires a target to use skill.
    float barrageCooldown; //For barrage and scatter behaviors
    int barrageCounter;
    public Vector3 targetPos; //last known pos of mouse or enemy.
    public bool activateBarrage;
    //Behaviors 
    [Header("Behaviors")]
    public bool autoUseSkill; //Manual or automatic use of skill. Manual can freely target enemies.
    public bool isMelee; //whether or not it is melee or projectile.
    public bool useMultiTarget; //is automatic only.
    public bool useBarrage; //Use strike/projectile back to back on the same target/area.
    public bool useScatter; //Use strike/projectile back to back but has a uneven target/angle.
    public bool useSpread; //skill will evenly spread and angle itself.
    public bool useOrbit; //targetless. Orbits around the player
    public bool useCircular; //targetless. Skill will evenly spread itself around the player.
    public bool useLateral; //Skill will line itself up horizontally.
    public bool useBurst; //Use all strike/projectile at once but has uneven target/angle, long CD, Increased Stats.
    public bool useThrowWeapon; //Whether or not to put this in the game.
    [Header("Secondary Behaviors")]
    public bool useOnTarget;    //Spawns on enemies. If false, spawns on player.
    public bool useRandomDirection; //targetless, is automatic, cannot be manual. Turn off autoUseSkill.
    public bool useBackwardsDirection; //Shoots from behind
    public bool useReturnDirection; //projectiles only.
    public bool useRandomTargeting; //Randomly targets an enemy in range. Targetless/Manual does nothing.
    [Header("Trigger")]
    public SkillTrigger skillTrigger;
    public bool devOnlyCheckThis;
    //Stats that are altered are stored in these variables.
    public List<float> addedBaseDamageTypes;
    public List<float> addedDamageTypes;
    public List<float> addedAilmentsChance;
    public List<float> addedAilmentsEffect;
    [HideInInspector] public float addedDamage;
    [HideInInspector] public float addedTravelSpeed;
    [HideInInspector] public float addedAttackRange;
    [HideInInspector] public float addedTravelRange;
    [HideInInspector] public float addedDuration;
    [HideInInspector] public float addedCooldown;
    [HideInInspector] public float addedKnockBack;
    [HideInInspector] public float addedCriticalChance, addedCriticalDamage;
    [HideInInspector] public float addedSize;
    [HideInInspector] public float addedLifeStealChance, addedLifeSteal;
    [HideInInspector] public int addedStrike, addedCombo, addedProjectile, addedPierce, addedChain;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void CheckTargetless()
    {
        if (useOrbit || useRandomDirection || (useCircular && !useThrowWeapon) || !autoUseSkill)
            targetless = true;
        else targetless = false;
    }
    private void Awake()
    {
        CheckTargetless();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (devOnlyCheckThis)
        {
            UpdateSkillStats();
        }
        PopulatePool(projectile + strike, prefabBehavior, poolParent, poolList);
        if (isMelee && useOrbit) //orbit melee, spawn orbiting weapons
        {
            PopulatePool(strike, meleeWeaponPrefab, orbitParent, orbitPoolList);
            OrbitBehavior(strike, orbitParent.transform, orbitPoolList);
        } 
        else if (!isMelee && useOrbit) //orbiting projectile
        {
            PopulatePool(projectile, prefabBehavior, orbitParent, orbitPoolList);
            OrbitBehavior(projectile, orbitParent.transform, orbitPoolList);
        }
        else if (useThrowWeapon)
        {
            orbitParent.transform.SetParent(player.transform.parent);
            PopulatePool(strike, meleeWeaponPrefab, orbitParent, orbitPoolList);
        }
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (useOrbit)
        {
            RunOrbit();
        }
        else if (!stopFiring && !useOrbit)
        {
            currentCooldown += Time.deltaTime;
            if (currentCooldown >= cooldown)
            {
                if (skillTrigger.isTriggerSkill == false)   //Use skill if it is not a skill trigger
                {
                    UseSkill();
                }
            }
        }
        else if (activateBarrage) //activates barrage/scatter firing inverval.
        {
            if (barrageCounter < strike + projectile) //fires the amount of attacks
            {
                barrageCooldown += Time.deltaTime;
                if (barrageCooldown >= 0.15f) //firing interval here.
                {
                    if (useBarrage)
                        BarrageBehavior(targetPos, transform, null);
                    else if (useScatter)
                        ScatterBehavior(targetPos, transform, null);
                    barrageCounter++;
                    barrageCooldown = 0;
                }
            }
            else if (barrageCounter >= strike + projectile)
            {
                barrageCounter = 0;
                activateBarrage = false;
                stopFiring = false;
            }
        }
    }
    public void UseSkill()
    {
        if ((enemyDistances.closestEnemyList.Count <= 0 || enemyDistances.updatingInProgress) && !targetless) return; //if no enemies alive, return.
        if (!targetless)   //Return if auto and not in attack range.
        {
            if (Vector3.Distance(transform.position, enemyDistances.closestEnemyList[0].transform.position) > attackRange)
            {
                return;
            }
        }
        if (skillTrigger.isTriggerSkill == true) //Check trigger condition.
        {
            if (!skillTrigger.CheckTriggerCondition()) return;
        }
        if (useRandomTargeting)
        {
            GetEnemiesInRangeUnsorted(transform);
            nearestEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }
        else if (!targetless)
        {
            nearestEnemy = enemyDistances.closestEnemyList[0];
        }
        stopFiring = true;
        currentCooldown = 0;
        if (combo > 0)
        {
            if (comboCounter > combo) comboMultiplier = combo;
            else comboMultiplier = comboCounter;
            comboCounter = 0;
        }
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
            activateBarrage = true;
            if (targetless)
                targetPos = gameplayManager.mousePos;
            else
                targetPos = nearestEnemy.transform.position;
        }
        else if (useScatter)
        {
            activateBarrage = true;
            if (targetless)
                targetPos = gameplayManager.mousePos;
            else
                targetPos = nearestEnemy.transform.position;
            if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            else if (autoUseSkill || useThrowWeapon) direction = targetPos - transform.position;
            else direction = targetPos - transform.position;
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
        else if (useMultiTarget)
        {
            if (enemyDistances.closestEnemyList.Count > 0)
                MultiTargetBehavior(strike + projectile, transform, enemyDistances.closestEnemyList);
        }
        foreach (InventoryManager.Skill sc in player.gameplayManager.inventory.activeSkillList) //Check use trigger skill condition
        {
            if (sc.skillController != null)
            {
                if (sc.skillController.skillTrigger.useUsageTrigger)
                {
                    sc.skillController.skillTrigger.currentCounter++; 
                    if (sc.skillController.currentCooldown >= cooldown)
                        sc.skillController.UseSkill();
                }
            }
        }
        GameManager.totalSkillsUsed++;
    }
    public void BarrageBehavior(Vector3 target, Transform spawnPos, SkillBehavior objectToDespawn)       //Spawn/Activate skill. Projectiles barrages.
    {
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill || useThrowWeapon) direction = target - transform.position;
        else direction = target - transform.position;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(strike + projectile, prefabBehavior, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                if (useOnTarget)
                {
                    if (autoUseSkill || useThrowWeapon)
                        poolList[i].transform.position = target;    //set starting position on target
                    else
                    {
                        if (direction.magnitude < attackRange)
                            poolList[i].transform.position = gameplayManager.mousePos;
                        else poolList[i].transform.position = spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange; ;
                    }
                }
                else
                    poolList[i].transform.position = spawnPos.position;    //set starting position on player
                SetBehavourStats(poolList[i]);
                poolList[i].SetDirection((direction).normalized);   //Set direction
                if (useBackwardsDirection && barrageCounter % 2 == 1) //reverse direction.
                {
                    poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                }
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                poolList[i].gameObject.SetActive(true);
                break;
            }
        }
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false); //deactivate object that uses this skill ie throwWeapon.
    }
    public void ScatterBehavior(Vector3 target, Transform spawnPos, SkillBehavior objectToDespawn)       //Spawn/Activate skill.
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(strike + projectile, prefabBehavior, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                if (useOnTarget)
                {
                    if (autoUseSkill || useThrowWeapon)
                        poolList[i].transform.position = target + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);    //set starting position on target
                    else
                    {
                        if (direction.magnitude < attackRange)
                            poolList[i].transform.position = gameplayManager.mousePos + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                        else poolList[i].transform.position = (spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange) + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                    }
                }
                else
                    poolList[i].transform.position = spawnPos.position;    //set starting position on player
                SetBehavourStats(poolList[i]);
                poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.forward) * direction).normalized);
                if (useBackwardsDirection && barrageCounter % 2 == 1)
                {
                    poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                }
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                poolList[i].gameObject.SetActive(true);
                break;
            }
        }
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false);
    }
    public void BurstBehavior(int numOfAttacks, Transform target, Transform spawnPos)
    {
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill || useThrowWeapon) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget)
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
                    SetBehavourStats(poolList[i]);
                    poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.forward) * direction).normalized);
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                    }
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void RunOrbit()
    {
        if (autoUseSkill)
        {
            orbitParent.transform.Rotate(new Vector3(0, 0, -(60 + (travelSpeed * 10)) * Time.deltaTime));
        }
        else
        {
            direction = gameplayManager.mousePos - orbitParent.transform.position;
            orbitParent.transform.rotation = Quaternion.RotateTowards(orbitParent.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), (60 + (travelSpeed * 10)) * Time.deltaTime);
        }
        for (int i = 0; i < orbitPoolList.Count; i++)
        {
            if (!orbitPoolList[i].isActiveAndEnabled)
            {
                orbitPoolList[i].currentDespawnTime += Time.deltaTime;
                if (orbitPoolList[i].currentDespawnTime >= despawnTime)
                {
                    orbitPoolList[i].travelSpeed = travelSpeed;
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
                    SetBehavourStats(poolList[i]);
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
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget) 
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
                    SetBehavourStats(poolList[i]);
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
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget && !useOrbit)
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
                        if (useOnTarget && useOrbit)
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
                    SetBehavourStats(poolList[i]);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void LateralBehavior(int numOfAttacks, float distanceApart, Transform target, Transform spawnPos, bool useBackwards)
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
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget)
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
                    SetBehavourStats(poolList[i]);
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void MultiTargetBehavior(int numOfAttacks, Transform spawnPos, List<EnemyStats> closestEnemyList)       //Targets multiple mobs at once. Only automatic. Null closestEnemyList == doesn't spawn from player
    {
        rememberEnemiesList.Clear();
        if (useRandomTargeting) GetEnemiesInRangeUnsorted(spawnPos);
        for (int t = 0; t < numOfAttacks; t++)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useRandomDirection && !useThrowWeapon)
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        poolList[i].transform.position = new Vector3(spawnPos.position.x + Random.Range(-attackRange, attackRange), spawnPos.position.y + Random.Range(-attackRange, attackRange), 0);
                    } 
                    else if (!autoUseSkill && t == 0 && !useThrowWeapon) //manual and first strike hits at mouse pos. Remove this later. Cannot be manual.
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
                        if (closestEnemyList == null)   //skill used from another source other than player
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
                            if (closestEnemyList.Count < t || closestEnemyList.Count <= 0)
                            {
                                stopFiring = false;
                                return;
                            }
                            try //getting errors because the index is out of range.
                            {
                                direction = closestEnemyList[t].transform.position - spawnPos.position;
                            }
                            catch
                            {
                                stopFiring = false;
                                return;
                            }
                            if (direction.magnitude <= attackRange)
                                poolList[i].transform.position = closestEnemyList[t].transform.position;
                            else
                            {
                                stopFiring = false;
                                return;
                            }
                        }
                    }
                    if (!isMelee)
                    {
                        poolList[i].transform.position = spawnPos.position;
                    }
                    SetBehavourStats(poolList[i]);
                    poolList[i].SetDirection((direction).normalized);   //Set direction
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
                SetBehavourStats(poolList[i]);
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
                PopulatePool(projectile, prefabBehavior, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                direction = target.position - spawnPos.position; 
                poolList[i].transform.position = spawnPos.position;
                poolList[i].enemyChainList.AddRange(chainList);
                poolList[i].target = target;
                SetBehavourStats(poolList[i]);
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
    }
    public virtual void PopulatePool(int spawnAmount, SkillBehavior prefab, GameObject parent, List<SkillBehavior> poolList)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SkillBehavior skill = Instantiate(prefab, parent.transform);    //Spawn, add to list, and initialize prefabs
            skill.gameObject.SetActive(false);
            skill.skillController = this;
            skill.SetStats(damageTypes[0], damageTypes[1], damageTypes[2], damageTypes[3], travelSpeed, pierce, chain, size);
            poolList.Add(skill);
        }
    }
    public void GainExp(float amt)
    {
        if (level < 5)
        {
            exp += amt;
            if (exp >= gameplayManager.skillExpCapList[level - 1]) //check if level up
            {
                FloatingTextController.DisplayPlayerText(player.transform, skillOrbDescription.itemName + " Leveled Up!", Color.white, 3f);
                UpdateStats.ApplySkillUpgrades(levelUpgrades, this, level - 1);
                exp -= gameplayManager.skillExpCapList[level - 1];
                level++;
                if (level < 5)
                {
                    if (exp >= gameplayManager.skillExpCapList[level - 1]) //if skill levels up 2 times on one exp gain. level up again. Should not be able to level more than 2.
                    {
                        UpdateStats.ApplySkillUpgrades(levelUpgrades, this, level - 1);
                        exp -= gameplayManager.skillExpCapList[level - 1];
                        level++;
                    }
                }
                gameplayManager.itemManager.skillExpDict[skillOrbDescription.itemName] = exp;
                gameplayManager.itemManager.skillLevelDict[skillOrbDescription.itemName] = level;
            }
        }
    }
    public void UpdateSkillStats() //Uses base stats and global multipliers to create stats
    {
        for (int i = 0; i < ailmentsChance.Count; i++)
        {
            ailmentsChance[i] = baseAilmentsChance[i] + gameplayManager.ailmentsChanceAdditive[i] + addedAilmentsChance[i];
        }
        for (int i = 0; i < ailmentsEffect.Count; i++)
        {
            ailmentsEffect[i] = (baseAilmentsEffect[i] + gameplayManager.baseAilmentsEffect[i]) * (1 + (gameplayManager.ailmentsEffectMultiplier[i] + addedAilmentsEffect[i]) / 100);
        }
        if (isMelee)   //is melee
        {
            damage = gameplayManager.damageMultiplier + gameplayManager.meleeDamageMultiplier + addedDamage;
            strike = baseStrike + gameplayManager.strikeAdditive + addedStrike;
            combo = baseCombo + gameplayManager.comboAdditive + addedCombo;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.meleeAttackRangeMultiplier + addedAttackRange) / 100);
            cooldown = baseCooldown * (1 - (gameplayManager.cooldownMultiplier + gameplayManager.meleeCooldownMultiplier + addedCooldown) / 100);
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.meleeCriticalChanceAdditive + addedCriticalChance;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.meleeCriticalDamageAdditive + addedCriticalDamage;
            size = gameplayManager.sizeMultiplier + gameplayManager.meleeSizeMultiplier + addedSize;
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.meleeLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.meleeLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultipiler + gameplayManager.meleeTravelSpeedMultipiler + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultipiler + gameplayManager.meleeTravelRangeMultipiler + addedTravelRange) / 100);
        }
        else //is projectile
        {
            damage = gameplayManager.damageMultiplier + gameplayManager.projectileDamageMultiplier + addedDamage;
            projectile = baseProjectile + gameplayManager.projectileAdditive + addedProjectile;
            chain = baseChain + gameplayManager.chainAdditive + addedChain;
            pierce = basePierce + gameplayManager.pierceAdditive + addedPierce;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.projectileAttackRangeMultiplier + addedAttackRange) / 100);
            cooldown = baseCooldown * (1 - (gameplayManager.cooldownMultiplier + gameplayManager.projectileCooldownMultiplier + addedCooldown) / 100);
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.projectileCriticalChanceAdditive + addedCriticalChance;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.projectileCriticalDamageAdditive + addedCriticalDamage;
            size = gameplayManager.sizeMultiplier + gameplayManager.projectileSizeMultiplier + addedSize;
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.projectileLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.projectileLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultipiler + gameplayManager.projectileTravelSpeedMultipiler + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultipiler + gameplayManager.projectileTravelRangeMultipiler + addedTravelRange) / 100);
        }
        if (gameplayManager.furthestAttackRange < attackRange)
        {
            gameplayManager.furthestAttackRange = attackRange;
        }
        for (int i = 0; i < baseDamageTypes.Count; i++) //Calculate damage with enemy's resistance.
        {
            if (baseDamageTypes[i] + addedBaseDamageTypes[i] + gameplayManager.baseDamageTypeAdditive[i] > 0)
            {
                damageTypes[i] = (baseDamageTypes[i] + gameplayManager.baseDamageTypeAdditive[i] + addedBaseDamageTypes[i]) * (1 + (gameplayManager.damageTypeMultiplier[i] + damage + addedDamageTypes[i]) / 100);
            }
        }
        highestDamageType = damageTypes.IndexOf(Mathf.Max(damageTypes.ToArray()));  //Find highest damage type.
        duration = baseDuration * (1 + (gameplayManager.durationMultiplier  + addedCooldown / 100));
        knockBack = baseKnockBack + addedKnockBack;
        currentCooldown = 0;
    }
    public void SetBehavourStats(SkillBehavior sb)
    {
        if (isMelee && combo > 0 && comboMultiplier > 0)
        {
            sb.SetStats((damageTypes[0] * (1 + (combo * 0.02f * comboMultiplier))) * (1 - gameplayManager.enemyResistances[0] / 100), (damageTypes[1] * (1 + (combo * 0.02f * comboMultiplier))) * (1 - gameplayManager.enemyResistances[1] / 100),
                (damageTypes[2] * (1 + (combo * 0.02f * comboMultiplier))) * (1 - gameplayManager.enemyResistances[2] / 100), (damageTypes[3] * (1 + (combo * 0.02f * comboMultiplier))) * (1 - gameplayManager.enemyResistances[3] / 100), travelSpeed, pierce, chain, size);
            return;
        }
        sb.SetStats(damageTypes[0] * (1 - gameplayManager.enemyResistances[0] / 100), damageTypes[1] * (1 - gameplayManager.enemyResistances[1] / 100),
            damageTypes[2] * (1 - gameplayManager.enemyResistances[2] / 100), damageTypes[3] * (1 - gameplayManager.enemyResistances[3] / 100), travelSpeed, pierce, chain, size);
    }
}