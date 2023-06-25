using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public EnemyStats enemyStats;
    public Vector3 direction;
    public float speed;
    public float duration, baseDuration;
    public void OnEnable()
    {
        duration = baseDuration;
    }
}
