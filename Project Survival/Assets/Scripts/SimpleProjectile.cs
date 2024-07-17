using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class SimpleProjectile : MonoBehaviour
{
    public bool isPlayerProjectile;
    public SpriteRenderer spriteRend;
    public Rigidbody2D rb;
    public Collider2D hitBoxCollider;
    public TrailRenderer trailRend;
    public Vector3 direction;
    public Vector3 startingPos;
    public Transform target;
    public List<float> damageTypes;
    public List<bool> applyAilment;
    public bool applyCrit, applyLifeSteal;
    public float currentRange, travelRange, travelSpeed, aoeProjectileDuration;
    public bool isAoeProjectile;
    public PassiveItemEffect passiveItemEffect;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>(); if (enemy == null || !enemy.gameObject.activeSelf) return;
            enemy.enemyStats.TakeDamage(enemy, damageTypes.Sum(), false);
            if (passiveItemEffect != null)
            {
                passiveItemEffect.totalRecorded += damageTypes.Sum();
            }
            gameObject.SetActive(false);
        }
    }
    public void Move()
    {
        currentRange = Vector3.Distance(transform.position, startingPos);
        if (currentRange >= travelRange)
        {
            gameObject.SetActive(false);
        }
        rb.MovePosition(transform.position + (travelSpeed * Time.fixedDeltaTime * direction));
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
        if (trailRend != null)
            trailRend.Clear();
        startingPos = transform.position;
        if (isAoeProjectile)
        {
            transform.DOJump(target.position, 2, 1, aoeProjectileDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
    private void OnDisable()
    {
        hitBoxCollider.enabled = true;
        isAoeProjectile = false;
        if (trailRend != null)
            trailRend.Clear();
    }
}
