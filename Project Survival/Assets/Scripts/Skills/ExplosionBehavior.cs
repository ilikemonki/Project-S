using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : SkillBehavior
{
    public SkillBehavior mainBehavior;
    public float hitboxColliderDuration, despawnTime;
    public Vector3 baseLocalScale;
    public void Awake()
    {
        baseLocalScale = transform.localScale;
    }
    public override void SetStats(float physical, float fire, float cold, float lightning, float travelSpeed, int pierce, int chain, float aoe)
    {
        this.enabled = false;
        base.SetStats(physical, fire, cold, lightning, travelSpeed, pierce, chain, aoe);
        gameObject.SetActive(false);
        this.travelSpeed = 0;
        this.pierce = 0;
        this.chain = 0;
        this.applyCrit = mainBehavior.applyCrit;
        this.applyLifeSteal = mainBehavior.applyLifeSteal;
        this.applyAilment.Clear();
        this.applyAilment.AddRange(mainBehavior.applyAilment); 
        transform.localScale = new Vector3(baseLocalScale.x * (1 + aoe / 100), baseLocalScale.y * (1 + aoe / 100), 1);
    }
    public override void BehaviorUpdate()
    {
        if (stayUpRightOnly && target != null) //FlipX only if stayUpRight is true
        {
            if (target.transform.position.x > transform.position.x)
            {
                spriteRend.flipX = false;
            }
            else spriteRend.flipX = true;
        }
        if (rememberEnemyList.Count > 0 && skillController.damageCooldown > 0) //For damage cooldown
        {
            currentDamageCooldownTimer += Time.deltaTime;
            if (currentDamageCooldownTimer >= skillController.damageCooldown)
            {
                rememberEnemyList.Clear();
                hitboxCollider.enabled = false;
                hitboxCollider.enabled = true;
                currentDamageCooldownTimer = 0;
            }
        }
        if (hitboxColliderDuration > 0 && hitboxCollider.enabled) //hitbox collider
        {
            currenthitboxColliderTimer += Time.deltaTime;
            if (currenthitboxColliderTimer >= hitboxColliderDuration)
            {
                if (skillController.resetHitBoxCollider)
                {
                    hitboxCollider.enabled = true;
                    currenthitboxColliderTimer = 0;
                }
                else
                    hitboxCollider.enabled = false;
            }
        }
        if (rotateSkill) //Rotate skill object
        {
            if (travelSpeed > 0)
                transform.Rotate(new Vector3(0, 0, 160 + (travelSpeed * 8)) * Time.deltaTime);
            else
                transform.Rotate(new Vector3(0, 0, 160 + (8)) * Time.deltaTime);
        }
        if (!isOrbitSkill)
        {
            if (!returnSkill)
            {
                if (despawnTime > 0) //skills that doesn't travel or have duration will despawn
                {
                    currentDespawnTime += Time.deltaTime;
                    if (currentDespawnTime >= despawnTime)
                    {
                        gameObject.SetActive(false);
                        mainBehavior.gameObject.SetActive(false);
                    }
                }
                else if (skillController.duration > 0) //Skills with duration (can have travelspeed) will use duration
                {
                    currentDuration += Time.deltaTime;
                    if (currentDuration >= skillController.duration)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
