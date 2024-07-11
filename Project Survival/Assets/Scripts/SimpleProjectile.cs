using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public bool isPlayerProjectile;
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public Vector3 direction;
    public Vector3 startingPos;
    public List<float> damageTypes;
    public List<bool> applyAilment;
    public bool applyCrit, applyLifeSteal;
    public float currentRange, travelRange, travelSpeed;
    float totalDamage;
    public PassiveItemEffect passiveItemEffect;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponentInParent<EnemyStats>(); if (enemy == null || !enemy.gameObject.activeSelf) return;
            enemy.TakeDamage(damageTypes.Sum(), false);
            if (passiveItemEffect != null)
            {
                passiveItemEffect.totalRecorded += damageTypes.Sum();
            }
            gameObject.SetActive(false);
        }
    }
    public void SetDirection(Vector3 dir) //set where the skill will go.
    {
        direction = dir;
        if ((direction.normalized.x < 0 && transform.localScale.y > 0) || (direction.normalized.x > 0 && transform.localScale.y < 0))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1);
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
