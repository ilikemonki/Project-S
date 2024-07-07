using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public SimpleProjectile prefab;
    public List<SimpleProjectile> projectileList;
    int counter;
    float spreadAngle;
    Vector3 direction;
    private void Start()
    {
        PopulatePool(20);
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < projectileList.Count; i++)  //Loop through all projectiles and move them.
        {
            if (projectileList[i].isActiveAndEnabled)
            {
                projectileList[i].currentRange = Vector3.Distance(projectileList[i].transform.position, projectileList[i].startingPos);
                if (projectileList[i].currentRange >= projectileList[i].travelRange)
                {
                    projectileList[i].gameObject.SetActive(false);
                }
                projectileList[i].rb.MovePosition(projectileList[i].transform.position + (projectileList[i].travelSpeed * Time.fixedDeltaTime * projectileList[i].direction));
            }
        }
    }
    public void SpreadBehavior(EnemyStats enemy, Transform playerPos)       //Spawn/Activate skill. Projectiles spread.
    {
        counter = 0;
        spreadAngle = 90 / enemy.projectile;
        direction = playerPos.position - enemy.transform.position;
        for (int p = 0; p < enemy.projectile; p++)    //number of projectiles
        {
            if (p != 0) counter++;
            for (int i = 0; i < projectileList.Count; i++)
            {
                if (i > projectileList.Count - 2)
                {
                    PopulatePool(10);
                }
                if (!projectileList[i].isActiveAndEnabled)
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
                    SetProjectileStats(projectileList[i], enemy);
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
                    PopulatePool(10);
                }
                if (!projectileList[i].isActiveAndEnabled)
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
    public void BurstBehavior(EnemyStats enemy, Transform playerPos)
    {
        direction = playerPos.position - enemy.transform.position;
        for (int p = 0; p < enemy.projectile; p++)    //number of projectiles
        {
            for (int i = 0; i < projectileList.Count; i++)
            {
                if (i > projectileList.Count - 2)
                {
                    PopulatePool(10);
                }
                if (!projectileList[i].isActiveAndEnabled)
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

    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SimpleProjectile projectile = Instantiate(prefab, transform);    //Spawn, add to list, and initialize prefabs
            projectile.gameObject.SetActive(false);
            projectileList.Add(projectile);
        }
    }
}
