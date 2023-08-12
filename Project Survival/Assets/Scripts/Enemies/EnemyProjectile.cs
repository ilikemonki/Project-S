using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public EnemyStats enemyStats;
    public TrailRenderer trailRend;
    public Vector3 direction;
    public Vector3 startingPos;
    public float currentRange;
    public void OnEnable()
    {
        trailRend.Clear();
        startingPos = transform.position;
    }
    private void OnDisable()
    {
        trailRend.Clear();
    }
}
