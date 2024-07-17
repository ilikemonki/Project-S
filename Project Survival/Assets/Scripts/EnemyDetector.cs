using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public CircleCollider2D detectorCollider;
    public List<Enemy> enemyDetectorList;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (!enemyDetectorList.Contains(enemy))
            {
                enemyDetectorList.Add(enemy);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemyDetectorList.Contains(enemy))
            {
                enemyDetectorList.Remove(enemy);
            }
        }
    }
    public void SetDetectorRange(float newRange)
    {
        detectorCollider.radius = newRange;
    }
    public void RemoveEnemyFromList(Enemy enemy)
    {
        if (enemyDetectorList.Contains(enemy))
            enemyDetectorList.Remove(enemy);
    }
    public Enemy FindNearestTarget()
    {
        if (enemyDetectorList.Count <= 0) return null;
        float shortestDistance = Mathf.Infinity;
        float distanceToEnemy;
        Enemy nearestEnemy = null;
        for (int i = 0; i < enemyDetectorList.Count; i++) //search all mobs for nearest distance
        {
            if (enemyDetectorList[i] == null)
                enemyDetectorList.Remove(enemyDetectorList[i]);
            try
            {
                if (enemyDetectorList[i].isActiveAndEnabled)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, enemyDetectorList[i].transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemyDetectorList[i];
                    }
                }
                else
                    enemyDetectorList.Remove(enemyDetectorList[i]);

            }
            catch
            {
                return null;
            }
        }
        if (nearestEnemy != null) return nearestEnemy;
        else return null;
    }
    public Enemy FindRandomTarget()
    {
        if (enemyDetectorList.Count <= 0) return null;
        else
        {
            try
            {

                int rand = Random.Range(0, enemyDetectorList.Count);
                if (enemyDetectorList[rand].isActiveAndEnabled)
                    return enemyDetectorList[rand];
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
