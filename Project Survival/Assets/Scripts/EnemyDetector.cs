using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public CircleCollider2D detectorCollider;
    public List<EnemyStats> enemyDetectorList;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponentInParent<EnemyStats>();
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
            EnemyStats enemy = collision.GetComponentInParent<EnemyStats>();
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
    public void RemoveEnemyFromList(EnemyStats enemy)
    {
        if (enemyDetectorList.Contains(enemy))
            enemyDetectorList.Remove(enemy);
    }
    public EnemyStats FindNearestTarget()
    {
        if (enemyDetectorList.Count <= 0) return null;
        float shortestDistance = Mathf.Infinity;
        float distanceToEnemy;
        EnemyStats nearestEnemy = null;
        for (int i = 0; i < enemyDetectorList.Count; i++) //search all mobs for nearest distance
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
        }
        if (nearestEnemy != null) return nearestEnemy;
        else return null;
    }
    public EnemyStats FindRandomTarget()
    {
        if (enemyDetectorList.Count <= 0) return null;
        return enemyDetectorList[Random.Range(0, enemyDetectorList.Count)];
    }
}
