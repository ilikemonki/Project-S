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
    public GameObject poolParent, stayOnPlayerParent;
    public GameplayManager gameplayManager;
    public List<SkillBehavior> poolList = new();
    public List<SkillBehavior> stayOnPlayerPoolList = new();
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
    public int baseMeleeAmount, baseCombo, baseProjectileAmount, basePierce, baseChain;
    public float despawnTime; //skills that don't travel or have duration will have despawnTime. For animations to finish
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
    public int meleeAmount, combo, projectileAmount, pierce, chain;
    [Header("Other Stats")]
    public float hitboxColliderDuration; //duration for hitbox to stay up.
    public bool fixedProjMelee; //Set a number here to set max proj/melee.
    public bool fixedCooldown; //Cannot modify cooldown
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
    public bool useBarrage; //Use melee/projectile back to back on the same target/area.
    public bool useScatter; //Use melee/projectile back to back but has a uneven target/angle.
    public bool useSpread; //skill will evenly spread and angle itself.
    public bool useOrbit; //targetless. Orbits around the player
    public bool useCircular; //targetless. Skill will evenly spread itself around the player.
    public bool useLateral; //Skill will line itself up horizontally.
    public bool useBurst; //Use all melee/projectile at once but has uneven target/angle, long CD, Increased Stats.
    public bool useSimple;
    [Header("Secondary Behaviors")]
    public bool useOnTarget;    //Spawns on enemies. If false, spawns on player.
    public bool useRandomDirection; //targetless, is automatic, cannot be manual. Turn off autoUseSkill.
    public bool useBackwardsDirection; //Shoots from behind.
    public bool useReturnDirection; //projectiles only.
    public bool useRandomTargeting; //Randomly targets an enemy in range. Targetless/Manual does nothing.
    [Header("Other Behaviors")]
    public bool continuous; //doesn't despawn.
    public bool pierceAll; //infinite pierce.
    public bool stayOnPlayer; //Skill will stay on player and move with player.
    public bool alwaysActivate; //Skill keeps running.
    public bool automaticOnly; //Skill can only be automatic
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
    [HideInInspector] public int addedMeleeAmount, addedCombo, addedProjectileAmount, addedPierce, addedChain;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void CheckTargetless()
    {
        if (useOrbit || useRandomDirection || useCircular || !autoUseSkill)
            targetless = true;
        else targetless = false;
    }
    public virtual void Awake()
    {
        CheckTargetless();
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        if (devOnlyCheckThis)
        {
            UpdateSkillStats();
        }
        PopulatePool(projectileAmount + meleeAmount, prefabBehavior, poolParent, poolList); //populate skill objects in regular pool.
        if (isMelee && useOrbit) //orbit melee, spawn orbiting weapons
        {
            PopulatePool(meleeAmount, meleeWeaponPrefab, stayOnPlayerParent, stayOnPlayerPoolList);
            OrbitBehavior(meleeAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
        } 
        else if (!isMelee && useOrbit) //orbiting projectile
        {
            PopulatePool(projectileAmount, prefabBehavior, stayOnPlayerParent, stayOnPlayerPoolList);
            OrbitBehavior(projectileAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
        }
        else if (stayOnPlayer) //populate list.
        {
            PopulatePool(projectileAmount + meleeAmount, prefabBehavior, stayOnPlayerParent, stayOnPlayerPoolList);
        }
    }
    // Update is called once per frame
    public virtual void Update()
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
            if (barrageCounter < meleeAmount + projectileAmount) //fires the amount of attacks
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
            else if (barrageCounter >= meleeAmount + projectileAmount)
            {
                barrageCounter = 0;
                activateBarrage = false;
                stopFiring = false;
            }
        }
    }
    public void UseSkill()
    {
        if ((enemyDistances.closestEnemyList.Count <= 0 || enemyDistances.updatingInProgress) && !targetless && !alwaysActivate) return; //if no enemies alive, return.
        if (!targetless && !alwaysActivate)   //Return if auto and not in attack range.
        {
            if (Vector3.Distance(transform.position, enemyDistances.closestEnemyList[0].transform.position) > attackRange)
            {
                return;
            }
        }
        if (skillTrigger.isTriggerSkill) //Is a trigger skill. Check trigger condition then return.
        {
            if (!skillTrigger.CheckTriggerCondition()) return;
        }
        if (useRandomTargeting)
        {
            GetEnemiesInRangeUnsorted(transform);
            nearestEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }
        else if (!targetless && !alwaysActivate)
        {
            nearestEnemy = enemyDistances.closestEnemyList[0];
        }
        stopFiring = true;
        currentCooldown = 0;
        if (combo > 0 && isMelee)
        {
            if (comboCounter >= combo) 
            { 
                comboMultiplier = combo;
                comboCounter = 0; //reset combo
            }
            else comboMultiplier = comboCounter;
        }
        //use skills
        if (useBarrage)
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
            else if (autoUseSkill) direction = targetPos - transform.position;
            else direction = targetPos - transform.position;
        }
        else if (useBurst)
        {
            if (targetless)
                BurstBehavior(meleeAmount + projectileAmount, null, transform);
            else
                BurstBehavior(meleeAmount + projectileAmount, nearestEnemy.transform, transform);
        }
        else if (useSpread)
        {
            if (useRandomDirection)
                SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, null, transform, false);
            else if (useBackwardsDirection)
            {
                if (targetless)
                {
                    SpreadBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), maxSpreadAngle, null, transform, false);
                    SpreadBehavior((meleeAmount + projectileAmount) / 2, maxSpreadAngle, null, transform, true);
                }
                else
                {
                    SpreadBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), maxSpreadAngle, nearestEnemy.transform, transform, false);
                    SpreadBehavior((meleeAmount + projectileAmount) / 2, maxSpreadAngle, nearestEnemy.transform, transform, true);
                }
            }
            else
            {
                if (targetless)
                    SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, null, transform, false);
                else
                    SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, nearestEnemy.transform, transform, false);
            }
        }
        else if (useLateral)
        {
            if (useRandomDirection)
                LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
            else if (useBackwardsDirection)
            {
                if (targetless)
                {
                    LateralBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
                    LateralBehavior((meleeAmount + projectileAmount) / 2, poolList[0].transform.localScale.x - lateralOffset, null, transform, true);
                }
                else
                {
                    LateralBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), poolList[0].transform.localScale.x - lateralOffset, nearestEnemy.transform, transform, false);
                    LateralBehavior((meleeAmount + projectileAmount) / 2, poolList[0].transform.localScale.x - lateralOffset, nearestEnemy.transform, transform, true);
                }
            }
            else
            {
                if (targetless)
                    LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x * (1 - lateralOffset), null, transform, false);
                else
                    LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x * (1 - lateralOffset), nearestEnemy.transform, transform, false);
            }
        }
        else if (useCircular)
        {
            CircularBehavior(meleeAmount + projectileAmount, transform);
        }
        else if (useSimple)
        {
            SimpleBehavior(meleeAmount + projectileAmount, transform, transform, stayOnPlayerPoolList);
        }
        else if (useMultiTarget)
        {
            if (enemyDistances.closestEnemyList.Count > 0)
                MultiTargetBehavior(meleeAmount + projectileAmount, transform, enemyDistances.closestEnemyList);
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
        else if (autoUseSkill) direction = target - transform.position;
        else direction = target - transform.position;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(meleeAmount + projectileAmount, prefabBehavior, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                if (useOnTarget)
                {
                    if (autoUseSkill)
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
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false); //deactivate object that uses this skill ie turrets.
    }
    public void ScatterBehavior(Vector3 target, Transform spawnPos, SkillBehavior objectToDespawn)       //Spawn/Activate skill.
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(meleeAmount + projectileAmount, prefabBehavior, poolParent, poolList);
            }
            if (!poolList[i].isActiveAndEnabled)
            {
                if (useOnTarget)
                {
                    if (autoUseSkill)
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
        else if (autoUseSkill) direction = target.position - spawnPos.position;
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
                        if (autoUseSkill)
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
    public void SimpleBehavior(int numOfAttacks, Transform target, Transform spawnPos, List<SkillBehavior> pool)
    {
        if (useRandomDirection) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (i > pool.Count - 2)
                {
                    PopulatePool(numOfAttacks, prefabBehavior, stayOnPlayerParent, pool);
                }
                if (!pool[i].isActiveAndEnabled)
                {
                    if (useOnTarget)
                    {
                        if (autoUseSkill)
                            pool[i].transform.position = target.position;    //set starting position on target
                        else
                        {
                            if (direction.magnitude < attackRange)
                                pool[i].transform.position = gameplayManager.mousePos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                            else pool[i].transform.position = (spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        }
                    }
                    else
                        pool[i].transform.position = spawnPos.position;    //set starting position on player
                    SetBehavourStats(pool[i]);
                    pool[i].SetDirection((direction).normalized);
                    if (useBackwardsDirection && p % 2 == 1)
                    {
                        pool[i].SetDirection((-pool[i].direction).normalized);   //Set direction
                    }
                    //pool[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(pool[i].direction.y, pool[i].direction.x) * Mathf.Rad2Deg); //set angle
                    pool[i].gameObject.SetActive(true);
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
            stayOnPlayerParent.transform.Rotate(new Vector3(0, 0, -(60 + (travelSpeed * 10)) * Time.deltaTime)); //rotation speed
        }
        else
        {
            direction = gameplayManager.mousePos - stayOnPlayerParent.transform.position;
            stayOnPlayerParent.transform.rotation = Quaternion.RotateTowards(stayOnPlayerParent.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), (60 + (travelSpeed * 10)) * Time.deltaTime);
        }
        for (int i = 0; i < stayOnPlayerPoolList.Count; i++) //increment current CD to respawn the skill.
        {
            if (!stayOnPlayerPoolList[i].isActiveAndEnabled)
            {
                stayOnPlayerPoolList[i].currentDespawnTime += Time.deltaTime;
                if (stayOnPlayerPoolList[i].currentDespawnTime >= cooldown) //used to be set to despawnTime
                {
                    stayOnPlayerPoolList[i].travelSpeed = travelSpeed;
                    stayOnPlayerPoolList[i].pierce = pierce;
                    stayOnPlayerPoolList[i].chain = chain;
                    stayOnPlayerPoolList[i].currentDespawnTime = 0;
                    stayOnPlayerPoolList[i].gameObject.SetActive(true);
                }
            }
        }
    }
    public void OrbitBehavior(int numOfAttacks, Transform spawnPos, List<SkillBehavior> pList) //Used to set position of orbit skills and call at the start and when adding proj/melee.
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/melee
        {
            pList[p].isOrbitSkill = true;
            pList[p].transform.position = spawnPos.position + Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right * (attackRange * 0.5f); 
            SetBehavourStats(stayOnPlayerPoolList[p]);
            pList[p].gameObject.SetActive(true);
        }
        stopFiring = false;
    }
    public void CircularBehavior(int numOfAttacks, Transform spawnPos)
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/melee
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
                        if (autoUseSkill)
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
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget && !useOrbit)
                    {
                        if (autoUseSkill)
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
                    PopulatePool(numOfAttacks, prefabBehavior, poolParent, poolList);
                }
                if (!poolList[i].isActiveAndEnabled)
                {
                    if (useOnTarget)
                    {
                        if (autoUseSkill)
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
                    if (useRandomDirection)
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                        poolList[i].transform.position = new Vector3(spawnPos.position.x + Random.Range(-attackRange, attackRange), spawnPos.position.y + Random.Range(-attackRange, attackRange), 0);
                    } 
                    else if (!autoUseSkill && t == 0 ) //manual and first melee hits at mouse pos. Remove this later. Cannot be manual.
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
                                Debug.Log("Multi Target Error: Did not find target.");
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
    public void SpawnChainProjectile(List<EnemyStats> chainList, Transform target, Transform spawnPos)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(projectileAmount, prefabBehavior, poolParent, poolList);
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
            if (!fixedProjMelee)
                meleeAmount = baseMeleeAmount + gameplayManager.meleeAmountAdditive + addedMeleeAmount;
            else meleeAmount = baseMeleeAmount;
            combo = baseCombo + gameplayManager.comboAdditive + addedCombo;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.meleeAttackRangeMultiplier + addedAttackRange) / 100);
            if (!fixedCooldown)
                cooldown = baseCooldown * (1 - (gameplayManager.cooldownMultiplier + gameplayManager.meleeCooldownMultiplier + addedCooldown) / 100);
            else cooldown = baseCooldown;
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.meleeCriticalChanceAdditive + addedCriticalChance;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.meleeCriticalDamageAdditive + addedCriticalDamage;
            size = gameplayManager.sizeMultiplier + gameplayManager.meleeSizeMultiplier + addedSize;
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.meleeLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.meleeLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultiplier + gameplayManager.meleeTravelSpeedMultiplier + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultiplier + gameplayManager.meleeTravelRangeMultiplier + addedTravelRange) / 100);
        }
        else //is projectile
        {
            damage = gameplayManager.damageMultiplier + gameplayManager.projectileDamageMultiplier + addedDamage;
            if (!fixedProjMelee)
                projectileAmount = baseProjectileAmount + gameplayManager.projectileAmountAdditive + addedProjectileAmount;
            else projectileAmount = baseProjectileAmount;
            chain = baseChain + gameplayManager.chainAdditive + addedChain;
            pierce = basePierce + gameplayManager.pierceAdditive + addedPierce;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.projectileAttackRangeMultiplier + addedAttackRange) / 100);
            if (!fixedCooldown)
                cooldown = baseCooldown * (1 - (gameplayManager.cooldownMultiplier + gameplayManager.projectileCooldownMultiplier + addedCooldown) / 100);
            else cooldown = baseCooldown;
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.projectileCriticalChanceAdditive + addedCriticalChance;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.projectileCriticalDamageAdditive + addedCriticalDamage;
            size = gameplayManager.sizeMultiplier + gameplayManager.projectileSizeMultiplier + addedSize;
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.projectileLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.projectileLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultiplier + gameplayManager.projectileTravelSpeedMultiplier + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultiplier + gameplayManager.projectileTravelRangeMultiplier + addedTravelRange) / 100);
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
        if (useOrbit)
        {
            if (stayOnPlayerPoolList.Count < projectileAmount + meleeAmount) //if proj/melee has increased, spawn more to pool.
            {
                if (isMelee) //orbit melee, spawn orbiting weapons
                {
                    PopulatePool(meleeAmount - stayOnPlayerPoolList.Count, meleeWeaponPrefab, stayOnPlayerParent, stayOnPlayerPoolList);
                    OrbitBehavior(meleeAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
                }
                else //orbiting projectile
                {
                    PopulatePool(projectileAmount - stayOnPlayerPoolList.Count, prefabBehavior, stayOnPlayerParent, stayOnPlayerPoolList);
                    OrbitBehavior(projectileAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
                }
            }
            else if (stayOnPlayerPoolList.Count > projectileAmount + meleeAmount) //if decreased, delete behavior.
            {
                int amountToDelete = stayOnPlayerPoolList.Count - (projectileAmount + meleeAmount);
                for (int i = 0; i < amountToDelete; i++)
                {
                    Destroy(stayOnPlayerPoolList[^1].gameObject);
                    stayOnPlayerPoolList.RemoveAt(stayOnPlayerPoolList.Count - 1);
                }
                OrbitBehavior(projectileAmount + meleeAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
            }
            for (int i = 0; i < stayOnPlayerPoolList.Count; i++) //Set orbit behaviour pool stats.
            {
                SetBehavourStats(stayOnPlayerPoolList[i]);
            }
        }
    }
    public void SetBehavourStats(SkillBehavior sb)
    {
        if (isMelee && combo > 0 && comboMultiplier > 0)
        {
            sb.SetStats((damageTypes[0] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[0] / 100), (damageTypes[1] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[1] / 100),
                (damageTypes[2] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[2] / 100), (damageTypes[3] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[3] / 100), travelSpeed, pierce, chain, size + (3f * comboMultiplier));
            return;
        }
        sb.SetStats(damageTypes[0] * (1 - gameplayManager.enemyReductions[0] / 100), damageTypes[1] * (1 - gameplayManager.enemyReductions[1] / 100),
            damageTypes[2] * (1 - gameplayManager.enemyReductions[2] / 100), damageTypes[3] * (1 - gameplayManager.enemyReductions[3] / 100), travelSpeed, pierce, chain, size);
    }
}