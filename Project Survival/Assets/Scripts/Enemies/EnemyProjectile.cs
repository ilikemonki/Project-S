using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public EnemyStats enemyStats;
    public TrailRenderer trailRend;
    public Vector3 direction;
    public float duration;
    public void OnEnable()
    {
        trailRend.Clear();
    }
    private void OnDisable()
    {
        trailRend.Clear();
    }
}
