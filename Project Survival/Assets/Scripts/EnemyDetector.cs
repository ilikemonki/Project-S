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
        if (detectorCollider.radius < newRange)
            detectorCollider.radius = newRange;
    }
    public void RemoveEnemyFromList(EnemyStats enemy)
    {
        if (enemyDetectorList.Contains(enemy))
            enemyDetectorList.Remove(enemy);
    }
}
