using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeDamage : MonoBehaviour
{
    public SpriteRenderer spriteRend;
    public Collider2D hitBoxCollider;
    public List<float> damageTypes;
    public float aoeProjectileDuration, aoeDespawnDuration, aoeHitBoxDuration;
    public float aoeProjectileTimer, aoeDespawnTimer;
    public RectTransform indicatorRect, fillRect;
    public bool startFill;
    // Update is called once per frame
    void Update()
    {
        if (spriteRend.enabled) 
        {
            AoeDamageDespawnTimer();
        }
    }

    public void SetAoeDamage(Transform targetPos, EnemyStats enemy)
    {
        transform.position = targetPos.position;
        aoeProjectileDuration = enemy.aoeProjectileDuration;
        aoeDespawnDuration = enemy.aoeDespawnDuration;
        aoeHitBoxDuration = enemy.aoeHitBoxDuration;
        startFill = true;
        aoeDespawnTimer = 0;
        damageTypes.Clear();
        damageTypes.AddRange(enemy.damageTypes);
        fillRect.localScale = new Vector3(0, 0, 1);
        indicatorRect.gameObject.SetActive(true);
        fillRect.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        startFill = false;
        spriteRend.enabled = false;
        hitBoxCollider.enabled = false;
    }
    private void OnEnable()
    {
        spriteRend.enabled = false;
        if (startFill)
        {
            fillRect.DOScale(indicatorRect.localScale, aoeProjectileDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                hitBoxCollider.enabled = true;
                spriteRend.enabled = true;
                indicatorRect.gameObject.SetActive(false);
                fillRect.gameObject.SetActive(false);
            });
        }
    }
    public void AoeDamageDespawnTimer()
    {
        aoeDespawnTimer += Time.deltaTime;
        if (aoeDespawnTimer >= aoeDespawnDuration)
        {
            gameObject.SetActive(false);
        }
        if (hitBoxCollider.enabled)
        {
            if (aoeDespawnTimer > aoeHitBoxDuration)
                hitBoxCollider.enabled = false;
        }
    }

}
