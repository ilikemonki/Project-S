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
    List<Enemy> multiTargetList = new(); //Used in multitarget behavior to remember enemies targeted.
    public PlayerStats player;
    public EnemyManager enemyManager;
    public Upgrades levelUpgrades;
    public int level;
    public float exp;
    Enemy enemyTarget;
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
    public float aoe;
    public float lifeStealChance, lifeSteal;
    public int meleeAmount, combo, projectileAmount, pierce, chain;
    [Header("Other Stats")]
    public float hitboxColliderDuration; //duration for hitbox to stay up.
    public bool resetHitBoxCollider; //after hitbox duration, renable hitbox
    public float damageCooldown; //CD for when enemies can be hit again.
    public bool fixedProjMelee; //Set a number here to set max proj/melee.
    public bool fixedCooldown; //Cannot modify cooldown
    public bool cannotPierce, cannotChain; //Projectiles that cannot pierce or chain.
    public bool limitAmountAtATime; //There can only be the max proj/melee at a time enabled.
    int numberOfSkillObjectsEnabled; //num of skill objects currently enabled.
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
    public bool useOrbit; //targetless. Orbits around the player.
    public bool useCircular; //targetless. Skill will evenly spread itself around the player.
    public bool useLateral; //Skill will line itself up horizontally.
    public bool useBurst; //Use all melee/projectile at once but has uneven target/angle, long CD, Increased Stats.
    public bool useSimple;
    [Header("Secondary Behaviors")]
    public bool useOnTarget;    //Spawns on enemies. If false, spawns on player. Non-Gem
    public bool useRandomTargetless; //targetless, cannot be manual. Automatic.
    public bool useRandomTarget; //Randomly targets an enemy in range. Automatic.
    public bool useBackwardSplit; //Half of proj/melee is used in the opposite direction
    public bool useReturn; //Travel skills only.
    public bool useHoming, useHomingReturn; //Homes onto target. Return homes to player. Travel skills only.
    [Header("Other Behaviors")]
    public bool isAoe;
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
    [HideInInspector] public float addedAoe;
    [HideInInspector] public float addedLifeStealChance, addedLifeSteal;
    [HideInInspector] public int addedMeleeAmount, addedCombo, addedProjectileAmount, addedPierce, addedChain;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void CheckTargetless()
    {
        if (useOrbit || useRandomTargetless || useCircular || !autoUseSkill)
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
        if (!useOrbit)
            PopulatePool(projectileAmount + meleeAmount); //populate all needed objects to their pool list.
        if (useOrbit) //orbit melee, spawn orbiting weapons
        {
            OrbitBehavior(projectileAmount + meleeAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
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
                if (!skillTrigger.isTriggerSkill)   //Use skill if it is not a skill trigger
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
                if (barrageCooldown >= 0.10f) //firing interval here.
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
        if ((player.enemyDetector.enemyDetectorList.Count <= 0) && !targetless && !alwaysActivate) return; //if no enemies detected, return.
        if (!targetless && !alwaysActivate)   //Return if automatic and no enemy in attack range.
        {
            enemyTarget = player.enemyDetector.FindNearestTarget(); //Get the nearest enemy
            if (enemyTarget == null) return;
            if (Vector3.Distance(transform.position, enemyTarget.transform.position) > attackRange)
            {
                return;
            }
        }
        if (skillTrigger.isTriggerSkill) //Is a trigger skill. Check trigger condition then return.
        {
            if (!skillTrigger.CheckTriggerCondition()) return;
        }
        if (useRandomTarget)
        {
            enemyTarget = player.enemyDetector.FindRandomTarget();
            if (enemyTarget == null) return;
        }
        stopFiring = true;
        currentCooldown = 0;
        if (combo > 0 && isMelee) //combo
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
                targetPos = enemyTarget.transform.position;
        }
        else if (useScatter)
        {
            activateBarrage = true;
            if (targetless)
                targetPos = gameplayManager.mousePos;
            else
                targetPos = enemyTarget.transform.position;
            if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            else if (autoUseSkill) direction = targetPos - transform.position;
            else direction = targetPos - transform.position;
        }
        else if (useBurst)
        {
            if (targetless)
                BurstBehavior(meleeAmount + projectileAmount, null, transform);
            else
                BurstBehavior(meleeAmount + projectileAmount, enemyTarget.transform, transform);
        }
        else if (useSpread)
        {
            if (useRandomTargetless)
                SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, null, transform, false);
            else if (useBackwardSplit)
            {
                if (targetless)
                {
                    SpreadBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), maxSpreadAngle, null, transform, false);
                    SpreadBehavior((meleeAmount + projectileAmount) / 2, maxSpreadAngle, null, transform, true);
                }
                else
                {
                    SpreadBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), maxSpreadAngle, enemyTarget.transform, transform, false);
                    SpreadBehavior((meleeAmount + projectileAmount) / 2, maxSpreadAngle, enemyTarget.transform, transform, true);
                }
            }
            else
            {
                if (targetless)
                    SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, null, transform, false);
                else
                    SpreadBehavior(meleeAmount + projectileAmount, maxSpreadAngle, enemyTarget.transform, transform, false);
            }
        }
        else if (useLateral)
        {
            if (useRandomTargetless)
                LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
            else if (useBackwardSplit)
            {
                if (targetless)
                {
                    LateralBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), poolList[0].transform.localScale.x - lateralOffset, null, transform, false);
                    LateralBehavior((meleeAmount + projectileAmount) / 2, poolList[0].transform.localScale.x - lateralOffset, null, transform, true);
                }
                else
                {
                    LateralBehavior((meleeAmount + projectileAmount) - ((meleeAmount + projectileAmount) / 2), poolList[0].transform.localScale.x - lateralOffset, enemyTarget.transform, transform, false);
                    LateralBehavior((meleeAmount + projectileAmount) / 2, poolList[0].transform.localScale.x - lateralOffset, enemyTarget.transform, transform, true);
                }
            }
            else
            {
                if (targetless)
                    LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x * (1 - lateralOffset), null, transform, false);
                else
                    LateralBehavior(meleeAmount + projectileAmount, poolList[0].transform.localScale.x * (1 - lateralOffset), enemyTarget.transform, transform, false);
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
            MultiTargetBehavior(meleeAmount + projectileAmount, transform);
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
        if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill) direction = target - transform.position;
        else direction = target - transform.position;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(meleeAmount + projectileAmount);
            }
            if (!poolList[i].gameObject.activeSelf)
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
                poolList[i].SetDirection((direction).normalized);   //Set direction
                if (useBackwardSplit && barrageCounter % 2 == 1) //reverse direction.
                {
                    poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                }
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                SetBehavourStats(poolList[i]);
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
                PopulatePool(meleeAmount + projectileAmount);
            }
            if (!poolList[i].gameObject.activeSelf)
            {
                if (useOnTarget)
                {
                    if (autoUseSkill)
                        poolList[i].transform.position = target + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);    //set starting position on target
                    else
                    {
                        if (direction.magnitude < attackRange)
                            poolList[i].transform.position = gameplayManager.mousePos + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
                        else poolList[i].transform.position = (spawnPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), Vector3.forward) * Vector3.right * attackRange) + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
                    }
                }
                else
                    poolList[i].transform.position = spawnPos.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);    //set starting position on player
                poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * direction).normalized); //scatter angle
                if (useBackwardSplit && barrageCounter % 2 == 1)
                {
                    poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                }
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                SetBehavourStats(poolList[i]);
                poolList[i].gameObject.SetActive(true);
                break;
            }
        }
        if (objectToDespawn != null) objectToDespawn.gameObject.SetActive(false);
    }
    public void BurstBehavior(int numOfAttacks, Transform target, Transform spawnPos)
    {
        if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks);
                }
                if (!poolList[i].gameObject.activeSelf)
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
                    poolList[i].SetDirection((Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.forward) * direction).normalized);
                    if (useBackwardSplit && p % 2 == 1)
                    {
                        poolList[i].SetDirection((-poolList[i].direction).normalized);   //Set direction
                    }
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    SetBehavourStats(poolList[i]);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void SimpleBehavior(int numOfAttacks, Transform target, Transform spawnPos, List<SkillBehavior> pool)
    {
        if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        else if (autoUseSkill) direction = target.position - spawnPos.position;
        else direction = gameplayManager.mousePos - spawnPos.position;
        for (int p = 0; p < numOfAttacks; p++)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (i > pool.Count - 2)
                {
                    PopulatePool(numOfAttacks);
                }
                if (!pool[i].gameObject.activeSelf)
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
                    pool[i].SetDirection((direction).normalized);
                    if (useBackwardSplit && p % 2 == 1)
                    {
                        pool[i].SetDirection((-pool[i].direction).normalized);   //Set direction
                    }
                    //pool[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(pool[i].direction.y, pool[i].direction.x) * Mathf.Rad2Deg); //set angle
                    SetBehavourStats(pool[i]);
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
        else //manually move. Orbit behavior is automatic only but leave this here.
        {
            direction = gameplayManager.mousePos - stayOnPlayerParent.transform.position;
            stayOnPlayerParent.transform.rotation = Quaternion.RotateTowards(stayOnPlayerParent.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), (60 + (travelSpeed * 10)) * Time.deltaTime);
        }
        if (!continuous)
        {
            for (int i = 0; i < stayOnPlayerPoolList.Count; i++) //increment current CD to respawn the skill.
            {
                if (!stayOnPlayerPoolList[i].gameObject.activeSelf)
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
    }
    public void OrbitBehavior(int numOfAttacks, Transform spawnPos, List<SkillBehavior> pList) //Used to set position of orbit skills and call at the start and when adding proj/melee.
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/melee
        {
            pList[p].isOrbitSkill = true;
            pList[p].transform.position = spawnPos.position + Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right * (attackRange * 0.5f);
            direction = pList[p].transform.position - spawnPos.position;
            if (isMelee) pList[p].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            else pList[p].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
            SetBehavourStats(stayOnPlayerPoolList[p]);
            pList[p].gameObject.SetActive(true);
        }
        stopFiring = false;
    }
    public void CircularBehavior(int numOfAttacks, Transform spawnPos) //targetless
    {
        spreadAngle = 360 / numOfAttacks;
        for (int p = 0; p < numOfAttacks; p++)    //number of projectiles/melee
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks);
                }
                if (!poolList[i].gameObject.activeSelf)
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
        if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
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
                    PopulatePool(numOfAttacks);
                }
                if (!poolList[i].gameObject.activeSelf)
                {
                    if (useOnTarget && !useOrbit)
                    {
                        if (autoUseSkill)
                        {
                            if (p % 2 == 1)
                            {
                                poolList[i].SetDirection((Quaternion.AngleAxis(spreadAngle * counter, Vector3.forward) * direction).normalized);
                                if (direction.magnitude < 3) //limit the spawn position minimum so it cannot spawn close to player.
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
                    if (!targetless)
                        poolList[i].target = enemyTarget.transform;
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
        if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
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
                    PopulatePool(numOfAttacks);
                }
                if (!poolList[i].gameObject.activeSelf)
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
                    poolList[i].SetDirection((direction).normalized);   //Set direction
                    poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg);
                    if (!targetless)
                        poolList[i].target = enemyTarget.transform;
                    SetBehavourStats(poolList[i]);
                    poolList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void MultiTargetBehavior(int numOfAttacks, Transform spawnPos) //Targets multiple mobs at once. Only automatic. Null closestEnemyList == doesn't spawn from player
    {
        if (limitAmountAtATime)
        {
            CheckLimitAmountAtATime();
            numOfAttacks -= numberOfSkillObjectsEnabled;
            if (numOfAttacks <= 0) //reset cd
            {
                currentCooldown = cooldown - 0.5f;
                stopFiring = false;
                return;
            }
        }
        FindMultiTargets(); //Get all possible targets into a multi target list
        for (int t = 0; t < numOfAttacks; t++)
        {
            if (multiTargetList.Count <= 0) //stop targeting if list has less than num of attacks or is zero
            {
                break;
            }
            if (useRandomTarget) enemyTarget = multiTargetList[Random.Range(0, multiTargetList.Count)];
            else enemyTarget = FindNearestMultiTarget(); //get nearest target
            if (enemyTarget == null) break;
            if (useRandomTargetless) direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            else direction = enemyTarget.transform.position - spawnPos.position;
            for (int i = 0; i < poolList.Count; i++)
            {
                if (i > poolList.Count - 2)
                {
                    PopulatePool(numOfAttacks);
                }
                if (!poolList[i].gameObject.activeSelf)
                {
                    if (useRandomTargetless && isMelee)
                    {
                        poolList[i].transform.position = new Vector3(spawnPos.position.x + Random.Range(-attackRange, attackRange), spawnPos.position.y + Random.Range(-attackRange, attackRange), 0);
                    }
                    else if (useOnTarget)
                    {
                        poolList[i].transform.position = enemyTarget.transform.position;    //set starting position on target
                    }
                    else
                        poolList[i].transform.position = spawnPos.position;    //set starting position on player
                    if (!isMelee)
                    {
                        poolList[i].SetDirection((direction).normalized);
                        poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(poolList[i].direction.y, poolList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    }
                    else poolList[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                    if (!targetless)
                        poolList[i].target = enemyTarget.transform;
                    SetBehavourStats(poolList[i]);
                    poolList[i].gameObject.SetActive(true);
                    multiTargetList.Remove(enemyTarget);
                    break;
                }
            }
        }
        stopFiring = false;
    }
    public void FindMultiTargets() //Find all multi targets within attack range, add to list
    {
        multiTargetList.Clear();
        multiTargetList.AddRange(player.enemyDetector.enemyDetectorList);
        for (int i = 0; i < multiTargetList.Count; i++) //Remove enemy from list if not in attack range
        {
            if (Vector3.Distance(transform.position, player.enemyDetector.enemyDetectorList[i].transform.position) > attackRange)
            {
                multiTargetList.Remove(player.enemyDetector.enemyDetectorList[i]);
            }
        }
    }
    public Enemy FindNearestMultiTarget()
    {
        if (multiTargetList.Count <= 0) return null;
        float shortestDistance = Mathf.Infinity;
        float distanceToEnemy;
        Enemy nearestEnemy = null;
        for (int i = 0; i < multiTargetList.Count; i++) //search all mobs for nearest distance
        {
            if (multiTargetList[i].gameObject.activeSelf)
            {
                distanceToEnemy = Vector3.Distance(transform.position, multiTargetList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = multiTargetList[i];
                }
            }
        }
        if (nearestEnemy != null) return nearestEnemy;
        else return null;
    }
    public void CheckLimitAmountAtATime() //Check the num of objects enabled.
    {
        numberOfSkillObjectsEnabled = 0;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (poolList[i].gameObject.activeSelf)
            {
                numberOfSkillObjectsEnabled++;
                if (numberOfSkillObjectsEnabled == (projectileAmount + meleeAmount)) break;
            }
        }
    }
    //Used for when orbit projectile has chain. Orbit proj will despawn and spawn a new projectile to chain.
    public void SpawnChainProjectile(List<Enemy> chainList, Transform target, Transform spawnPos)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (i > poolList.Count - 2)
            {
                PopulatePool(projectileAmount);
            }
            if (!poolList[i].gameObject.activeSelf)
            {
                direction = target.position - spawnPos.position; 
                poolList[i].transform.position = spawnPos.position;
                poolList[i].rememberEnemyList.AddRange(chainList);
                poolList[i].target = target;
                SetBehavourStats(poolList[i]);
                poolList[i].SetDirection((direction).normalized);   //Set direction
                poolList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set angle
                poolList[i].gameObject.SetActive(true);
                return;
            }
        }
    }
    public virtual void PopulatePool(int spawnAmount)
    {
        if (isMelee && useOrbit) //orbit melee. populate to stayOnPlayer list with melee weapon object.
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                SkillBehavior skill = Instantiate(meleeWeaponPrefab, stayOnPlayerParent.transform);    //Spawn, add to list, and initialize prefabs
                skill.gameObject.SetActive(false);
                skill.skillController = this;
                skill.SetStats(damageTypes[0], damageTypes[1], damageTypes[2], damageTypes[3], travelSpeed, pierce, chain, aoe);
                stayOnPlayerPoolList.Add(skill);
            }
        }
        else if ((!isMelee && useOrbit) || stayOnPlayer) //orbiting projectile. populate to stayOnPlayer list with skill object. 
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                SkillBehavior skill = Instantiate(prefabBehavior, stayOnPlayerParent.transform);    //Spawn, add to list, and initialize prefabs
                skill.gameObject.SetActive(false);
                skill.skillController = this;
                skill.SetStats(damageTypes[0], damageTypes[1], damageTypes[2], damageTypes[3], travelSpeed, pierce, chain, aoe);
                stayOnPlayerPoolList.Add(skill);
            }
        }
        if (useOrbit || stayOnPlayer) //if orbit or stayonPlayer, check poolList if need to populate more.
        {
            if (stayOnPlayerPoolList.Count > poolList.Count)
            {
                for (int i = 0; i < stayOnPlayerPoolList.Count - poolList.Count; i++)
                {
                    SkillBehavior skill = Instantiate(prefabBehavior, poolParent.transform);    //Spawn, add to list, and initialize prefabs
                    skill.gameObject.SetActive(false);
                    skill.skillController = this;
                    skill.SetStats(damageTypes[0], damageTypes[1], damageTypes[2], damageTypes[3], travelSpeed, pierce, chain, aoe);
                    poolList.Add(skill);
                }
            }
            return;
        }
        for (int i = 0; i < spawnAmount; i++) //Normal populate to poolList
        {
            SkillBehavior skill = Instantiate(prefabBehavior, poolParent.transform);    //Spawn, add to list, and initialize prefabs
            skill.gameObject.SetActive(false);
            skill.skillController = this;
            skill.SetStats(damageTypes[0], damageTypes[1], damageTypes[2], damageTypes[3], travelSpeed, pierce, chain, aoe);
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
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.meleeLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.meleeLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultiplier + gameplayManager.meleeTravelSpeedMultiplier + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultiplier + gameplayManager.meleeTravelRangeMultiplier + addedTravelRange) / 100);
            if (isAoe)
            {
                aoe = gameplayManager.aoeMultiplier + gameplayManager.meleeAoeMultiplier + addedAoe;
            }
        }
        else //is projectile
        {
            damage = gameplayManager.damageMultiplier + gameplayManager.projectileDamageMultiplier + addedDamage;
            if (!fixedProjMelee)
                projectileAmount = baseProjectileAmount + gameplayManager.projectileAmountAdditive + addedProjectileAmount;
            else projectileAmount = baseProjectileAmount;
            if (!cannotChain)
                chain = baseChain + gameplayManager.chainAdditive + addedChain;
            else chain = 0;
            if (!cannotPierce)
                pierce = basePierce + gameplayManager.pierceAdditive + addedPierce;
            else pierce = 0;
            attackRange = baseAttackRange * (1 + (gameplayManager.attackRangeMultiplier + gameplayManager.projectileAttackRangeMultiplier + addedAttackRange) / 100);
            if (!fixedCooldown)
                cooldown = baseCooldown * (1 - (gameplayManager.cooldownMultiplier + gameplayManager.projectileCooldownMultiplier + addedCooldown) / 100);
            else cooldown = baseCooldown;
            criticalChance = baseCriticalChance + gameplayManager.criticalChanceAdditive + gameplayManager.projectileCriticalChanceAdditive + addedCriticalChance;
            criticalDamage = baseCriticalDamage + gameplayManager.criticalDamageAdditive + gameplayManager.projectileCriticalDamageAdditive + addedCriticalDamage;
            lifeStealChance = baseLifeStealChance + gameplayManager.lifeStealChanceAdditive + gameplayManager.projectileLifeStealChanceAdditive + addedLifeStealChance;
            lifeSteal = baseLifeSteal + gameplayManager.lifeStealAdditive + gameplayManager.projectileLifeStealAdditive + addedLifeSteal;
            travelSpeed = baseTravelSpeed * (1 + (gameplayManager.travelSpeedMultiplier + gameplayManager.projectileTravelSpeedMultiplier + addedTravelSpeed) / 100);
            travelRange = baseTravelRange * (1 + (gameplayManager.travelRangeMultiplier + gameplayManager.projectileTravelRangeMultiplier + addedTravelRange) / 100); 
            if (isAoe)
            {
                aoe = gameplayManager.aoeMultiplier + gameplayManager.meleeAoeMultiplier + addedAoe;
            }
        }
        if (gameplayManager.furthestAttackRange <= attackRange)
        {
            gameplayManager.furthestAttackRange = attackRange;
            if (player != null) player.enemyDetector.SetDetectorRange(gameplayManager.furthestAttackRange);
        }
        for (int i = 0; i < baseDamageTypes.Count; i++) //Calculate damage
        {
            if (baseDamageTypes[i] > 0)
            {
                damageTypes[i] = (baseDamageTypes[i] + gameplayManager.baseDamageTypeAdditive[i] + addedBaseDamageTypes[i]) * (1 + (gameplayManager.damageTypeMultiplier[i] + damage + addedDamageTypes[i]) / 100);
                ailmentsChance[i] = baseAilmentsChance[i] + gameplayManager.ailmentsChanceAdditive[i] + addedAilmentsChance[i];
                ailmentsEffect[i] = (baseAilmentsEffect[i] + gameplayManager.baseAilmentsEffect[i]) * (1 + (gameplayManager.ailmentsEffectMultiplier[i] + addedAilmentsEffect[i]) / 100);
                break;
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
                    PopulatePool(meleeAmount - stayOnPlayerPoolList.Count);
                    OrbitBehavior(meleeAmount, stayOnPlayerParent.transform, stayOnPlayerPoolList);
                }
                else //orbiting projectile
                {
                    PopulatePool(projectileAmount - stayOnPlayerPoolList.Count);
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
                (damageTypes[2] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[2] / 100), (damageTypes[3] * (1 + (0.03f * comboMultiplier))) * (1 - gameplayManager.enemyReductions[3] / 100), travelSpeed, pierce, chain, aoe + (3f * comboMultiplier));
            return;
        }
        sb.SetStats(damageTypes[0] * (1 - gameplayManager.enemyReductions[0] / 100), damageTypes[1] * (1 - gameplayManager.enemyReductions[1] / 100),
            damageTypes[2] * (1 - gameplayManager.enemyReductions[2] / 100), damageTypes[3] * (1 - gameplayManager.enemyReductions[3] / 100), travelSpeed, pierce, chain, aoe);
    }

    public void SetAutomatic(bool automatic)
    {
        if (automatic)
        {
            autoUseSkill = true;
            if (useOrbit) //if useOrbit
            {
                damageCooldown = 0;
            }
        }
        else //manual
        {
            autoUseSkill = false;
            if (useOrbit) //if useOrbit, set damage cooldown.
            {
                damageCooldown = 1;
            }
        }
    }
}