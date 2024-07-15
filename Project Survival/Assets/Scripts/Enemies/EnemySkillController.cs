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
    public AoeDamage FindAoeDamage(Transform target, EnemyStats enemy)
    {
        for (int i = 0; i < aoeDamageList.Count; i++)
        {
            if (i > aoeDamageList.Count - 2)
            {
                PopulateAoeDamagePool(10);
            }
            if (!aoeDamageList[i].gameObject.activeSelf)
            {
                aoeDamageList[i].SetAoeDamage(target, enemy);
                return aoeDamageList[i];
            }
        }
        return null;
    }
    public void BarrageBehavior(EnemyStats enemy, Transform targetPos)       //Spawn/Activate skill. Projectiles barrages.
    {
        direction = targetPos.position - enemy.transform.position;
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
                projectileList[i].gameObject.SetActive(true);
                break;
            }
        }
    }
    public void SpreadBehavior(EnemyStats enemy, Transform targetPos)       //Spawn/Activate skill. Projectiles spread.
    {
        counter = 0;
        if (enemy.useAoeProjectile || enemy.useAoeOnTarget)
            spreadAngle = 60 / enemy.projectile;
        else spreadAngle = 90 / enemy.projectile;
        direction = targetPos.position - enemy.transform.position;
        if (enemy.useAoeOnTarget)
        {
            for (int p = 0; p < enemy.projectile; p++)
            {
                if (p != 0) counter++;
                aoeDamage = FindAoeDamage(targetPos, enemy);
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
        for (int p = 0; p < enemy.projectile; p++)    //number of projectiles
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
                    if (enemy.useAoeProjectile)
                    {
                        aoeDamage = FindAoeDamage(targetPos, enemy);
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
                        projectileList[i].aoeProjectileDuration = enemy.aoeProjectileDuration;
                        aoeDamage.gameObject.SetActive(true);
                    }
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void CircleBehavior(EnemyStats enemy)
    {
        spreadAngle = 360 / enemy.projectile;
        for (int p = 0; p < enemy.projectile; p++)    //number of projectiles/strikes
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
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
    public void BurstBehavior(EnemyStats enemy, Transform targetPos)
    {
        direction = targetPos.position - enemy.transform.position;
        for (int p = 0; p < enemy.projectile; p++)    //number of projectiles
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
                    projectileList[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public void SetProjectileStats(SimpleProjectile proj, EnemyStats enemy)
    {
        proj.damageTypes.Clear();
        proj.damageTypes.AddRange(enemy.damageTypes);
        proj.travelRange = enemy.projectileRange;
        proj.travelSpeed = enemy.projectileSpeed;
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
