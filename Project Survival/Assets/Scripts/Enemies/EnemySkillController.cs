using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public SimpleProjectile prefab;
    public AoeDamage aoeDamagePrefab;
    public List<SimpleProjectile> projectileList;
    public List<AoeDamage> aoeDamageList;
    public GameObject damageIndicatorParent;
    public AoeDamage aoeDamage;

    int counter;
    float spreadAngle;
    Vector3 direction;
    private void Start()
    {
        PopulateProjectilePool(20);
        PopulateAoeDamagePool(10);
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < projectileList.Count; i++)  //Loop through all projectiles and move them.
        {
            if (projectileList[i].gameObject.activeSelf && !projectileList[i].isAoeProjectile)
            {
                projectileList[i].Move();
            }
        }
    }
    public AoeDamage FindAoeDamage(Transform target, Enemy enemy, int p)
    {
        for (int i = 0; i < aoeDamageList.Count; i++)
        {
            if (i > aoeDamageList.Count - 2)
            {
                PopulateAoeDamagePool(10);
            }
            if (!aoeDamageList[i].gameObject.activeSelf)
            {
                aoeDamageList[i].SetAoeDamage(target, enemy, p);
                return aoeDamageList[i];
            }
        }
        return null;
    }
    public void BarrageBehavior(Enemy enemy, Transform targetPos)  
    {
        direction = targetPos.position - enemy.transform.position;
        if (enemy.enemyStats.useAoeOnTarget)
        {
            aoeDamage = FindAoeDamage(targetPos, enemy, 0);
            aoeDamage.transform.position = targetPos.position;
            aoeDamage.gameObject.SetActive(true);
            return;
        }
        for (int i = 0; i < projectileList.Count; i++)
        {
            if (i > projectileList.Count - 2)
            {
                PopulateProjectilePool(10);
            }
            if (!projectileList[i].gameObject.activeSelf)
            {
                projectileList[i].transform.position = enemy.transform.position;    //set starting position on player
                projectileList[i].direction = direction.normalized;
                projectileList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileList[i].direction.y, projectileList[i].direction.x) * Mathf.Rad2Deg); //set angle
                SetProjectileStats(projectileList[i], enemy);
                if (enemy.enemyStats.useAoeProjectile)
                {
                    aoeDamage = FindAoeDamage(targetPos, enemy, i);
                    aoeDamage.transform.position = targetPos.position;
                    projectileList[i].target = aoeDamage.transform;
                    projectileList[i].hitBoxCollider.enabled = false;
                    projectileList[i].isAoeProjectile = true;
                    projectileList[i].aoeProjectileDuration = enemy.enemyStats.aoeProjectileDuration + (enemy.enemyStats.aoeDelay * i);
                    aoeDamage.gameObject.SetActive(true);
                }
                projectileList[i].gameObject.SetActive(true);
                break;
            }
        }
    }
    public void SpreadBehavior(Enemy enemy, Transform targetPos)  
    {
        counter = 0;
        if (enemy.enemyStats.useAoeProjectile || enemy.enemyStats.useAoeOnTarget)
            spreadAngle = 60 / enemy.enemyStats.projectile;
        else spreadAngle = 90 / enemy.enemyStats.projectile;
        direction = targetPos.position - enemy.transform.position;
        if (enemy.enemyStats.useAoeOnTarget)
        {
            for (int p = 0; p < enemy.enemyStats.projectile; p++)
            {
                if (p != 0) counter++;
                aoeDamage = FindAoeDamage(targetPos, enemy, p);
                if (p % 2 != 0)
                {
                    counter--;
                    aoeDamage.transform.position = enemy.transform.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * (p - counter), Vector3.forward) * Vector3.right * direction.magnitude;
                }
                else
                {
                    aoeDamage.transform.position = enemy.transform.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * (p - counter), Vector3.forward) * Vector3.right * direction.magnitude;
                }
                aoeDamage.gameObject.SetActive(true);
            }
            return;
        }
        for (int p = 0; p < enemy.enemyStats.projectile; p++)    //number of projectiles
        {
            if (p != 0) counter++;
            for (int i = 0; i < projectileList.Count; i++)
            {
                if (i > projectileList.Count - 2)
                {
                    PopulateProjectilePool(10);
                }
                if (!projectileList[i].gameObject.activeSelf)
                {
                    projectileList[i].transform.position = enemy.transform.position;    //set starting position
                    if (p == 0)
                    {
                        projectileList[i].direction = direction.normalized;   //Set direction
                    }
                    else if (p % 2 != 0) //shoots up angle
                    {
                        counter--;
                        projectileList[i].direction = (Quaternion.AngleAxis(spreadAngle * (p - counter), Vector3.forward) * direction).normalized;
                    }
                    else //shoots down angle
                    {
                        projectileList[i].direction = (Quaternion.AngleAxis(-spreadAngle * (p - counter), Vector3.forward) * direction).normalized;
                    }
                    projectileList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileList[i].direction.y, projectileList[i].direction.x) * Mathf.Rad2Deg);
                    projectileList[i].target = targetPos;
                    SetProjectileStats(projectileList[i], enemy);
                    if (enemy.enemyStats.useAoeProjectile)
                    {
                        aoeDamage = FindAoeDamage(targetPos, enemy, p);
                        if (p % 2 != 0)
                        {
                            aoeDamage.transform.position = enemy.transform.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * (p - counter), Vector3.forward) * Vector3.right * direction.magnitude;
                        }
                        else
                        {
                            aoeDamage.transform.position = enemy.transform.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + -spreadAngle * (p - counter), Vector3.forward) * Vector3.right * direction.magnitude;
                        }
                        projectileList[i].target = aoeDamage.transform;
                        projectileList[i].hitBoxCollider.enabled = false;
                        projectileList[i].isAoeProjectile = true;
                        projectileList[i].aoeProjectileDuration = enemy.enemyStats.aoeProjectileDuration + (enemy.enemyStats.aoeDelay * p);
                        aoeDamage.gameObject.SetActive(true);
                    }
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void CircleBehavior(Enemy enemy, Transform targetPos)
    {
        spreadAngle = 360 / enemy.enemyStats.projectile;
        if (enemy.enemyStats.useAoeOnTarget)
        {
            for (int p = 0; p < enemy.enemyStats.projectile; p++)
            {
                aoeDamage = FindAoeDamage(targetPos, enemy, p); 
                direction = targetPos.position - enemy.transform.position;
                aoeDamage.transform.position = targetPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * p, Vector3.forward) * Vector3.right * 2.5f;

                aoeDamage.gameObject.SetActive(true);
            }
            return;
        }
        for (int p = 0; p < enemy.enemyStats.projectile; p++)    //number of projectiles/strikes
        {
            for (int i = 0; i < projectileList.Count; i++)
            {
                if (i > projectileList.Count - 2)
                {
                    PopulateProjectilePool(10);
                }
                if (!projectileList[i].gameObject.activeSelf)
                {
                    projectileList[i].transform.position = enemy.transform.position;
                    projectileList[i].direction = (Quaternion.AngleAxis(spreadAngle * p, Vector3.forward) * Vector3.right).normalized;   //Set direction
                    projectileList[i].transform.eulerAngles = new Vector3(0, 0, spreadAngle * p);
                    SetProjectileStats(projectileList[i], enemy);

                    if (enemy.enemyStats.useAoeProjectile)
                    {
                        aoeDamage = FindAoeDamage(targetPos, enemy, p); 
                        direction = targetPos.position - enemy.transform.position;
                        aoeDamage.transform.position = targetPos.position + Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + spreadAngle * p, Vector3.forward) * Vector3.right * 2.5f;
                        projectileList[i].target = aoeDamage.transform;
                        projectileList[i].hitBoxCollider.enabled = false;
                        projectileList[i].isAoeProjectile = true;
                        projectileList[i].aoeProjectileDuration = enemy.enemyStats.aoeProjectileDuration + (enemy.enemyStats.aoeDelay * p);
                        aoeDamage.gameObject.SetActive(true);
                    }
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void BurstBehavior(Enemy enemy, Transform targetPos)
    {
        direction = targetPos.position - enemy.transform.position;
        if (enemy.enemyStats.useAoeOnTarget)
        {
            for (int p = 0; p < enemy.enemyStats.projectile; p++)
            {
                aoeDamage = FindAoeDamage(targetPos, enemy, p);
                aoeDamage.transform.position = targetPos.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                aoeDamage.gameObject.SetActive(true);
            }
            return;
        }
        for (int p = 0; p < enemy.enemyStats.projectile; p++)    //number of projectiles
        {
            for (int i = 0; i < projectileList.Count; i++)
            {
                if (i > projectileList.Count - 2)
                {
                    PopulateProjectilePool(10);
                }
                if (!projectileList[i].gameObject.activeSelf)
                {
                    projectileList[i].transform.position = enemy.transform.position;    //set starting position on player
                    projectileList[i].direction = (Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.forward) * direction).normalized;
                    projectileList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileList[i].direction.y, projectileList[i].direction.x) * Mathf.Rad2Deg); //set angle
                    SetProjectileStats(projectileList[i], enemy);
                    if (enemy.enemyStats.useAoeProjectile)
                    {
                        aoeDamage = FindAoeDamage(targetPos, enemy, p);
                        aoeDamage.transform.position = targetPos.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                        projectileList[i].target = aoeDamage.transform;
                        projectileList[i].hitBoxCollider.enabled = false;
                        projectileList[i].isAoeProjectile = true;
                        projectileList[i].aoeProjectileDuration = enemy.enemyStats.aoeProjectileDuration + (enemy.enemyStats.aoeDelay * p);
                        aoeDamage.gameObject.SetActive(true);
                    }
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public void SetProjectileStats(SimpleProjectile proj, Enemy enemy)
    {
        proj.damageTypes.Clear();
        proj.damageTypes.AddRange(enemy.enemyStats.damageTypes);
        proj.travelRange = enemy.enemyStats.projectileRange;
        proj.travelSpeed = enemy.enemyStats.projectileSpeed;
    }

    public void PopulateProjectilePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SimpleProjectile projectile = Instantiate(prefab, transform);    //Spawn, add to list, and initialize prefabs
            projectile.gameObject.SetActive(false);
            projectileList.Add(projectile);
        }
    }
    public void PopulateAoeDamagePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            AoeDamage di = Instantiate(aoeDamagePrefab, damageIndicatorParent.transform);    //Spawn, add to list, and initialize prefabs
            di.gameObject.SetActive(false);
            aoeDamageList.Add(di);
        }
    }
}
