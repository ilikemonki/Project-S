using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectilePool : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public EnemyProjectile prefab;
    public List<EnemyProjectile> projectileList;
    private void Start()
    {
        PopulatePool(50);
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < projectileList.Count; i++)  //Loop through all projectiles and move them.
        {
            if (projectileList[i].isActiveAndEnabled)
            {
                projectileList[i].duration -= Time.fixedDeltaTime;
                if (projectileList[i].duration <= 0)
                {
                    projectileList[i].gameObject.SetActive(false);
                }
                projectileList[i].rb.MovePosition(projectileList[i].transform.position + (projectileList[i].speed * Time.fixedDeltaTime * projectileList[i].direction));
            }
        }
    }
    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            EnemyProjectile projectile = Instantiate(prefab, transform);    //Spawn, add to list, and initialize prefabs
            projectile.gameObject.SetActive(false);
            projectileList.Add(projectile);
        }
    }

    public void SpawnProjectile(EnemyStats enemy, Transform playerPos)
    {
        for (int i = 0; i < projectileList.Count; i++)
        {
            if (i > projectileList.Count - 5)
            {
                PopulatePool(10);
            }
            if (!projectileList[i].isActiveAndEnabled)
            {
                projectileList[i].enemyStats = enemy;
                projectileList[i].transform.position = enemy.transform.position;    //set starting position on enemy
                projectileList[i].direction = (playerPos.position - enemy.transform.position).normalized;
                projectileList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileList[i].direction.y, projectileList[i].direction.x) * Mathf.Rad2Deg);
                projectileList[i].gameObject.SetActive(true);
                break;
            }
        }
    }
}
