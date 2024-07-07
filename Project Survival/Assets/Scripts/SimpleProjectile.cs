using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public Vector3 direction;
    public Vector3 startingPos;
    public List<float> damageTypes;
    public float currentRange, travelRange, travelSpeed;
    public PassiveItemEffect passiveItemEffect;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponentInParent<EnemyStats>(); if (enemy == null || !enemy.isActiveAndEnabled) return;
            enemy.TakeDamage(damageTypes.Sum(), false);
            if (passiveItemEffect != null)
            {
                passiveItemEffect.totalRecorded += damageTypes.Sum();
            }
            gameObject.SetActive(false);
        }
    }
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
