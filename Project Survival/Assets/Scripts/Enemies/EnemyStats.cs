using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using TMPro;
using System.Linq;

public class EnemyStats : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemyMovement enemyMovement;
    public GameplayManager gameplayManager;
    //Stats
    public float maxHealth;
    public List<float> damageTypes;
    public List<float> reductions;
    public float moveSpeed; //baseMoveSpeed is the current movespeed, use to reset current movespeed back to normal when chilled.
    public float exp;
    public SpriteRenderer spriteRenderer;
    //public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public Material defaultMaterial;
    public Material damageFlashMaterial;
    public DropRate dropRate;
    public bool knockBackImmune;
    [Header("Attack")]
    public bool canAttack; //initialize in awake
    public bool spreadAttack, circleAttack, burstAttack, barrageAttack;
    public bool useAoeProjectile, useAoeOnTarget;
    public float aoeProjectileDuration, aoeDespawnDuration, aoeHitBoxDuration, aoeDelay;
    public float attackCooldown, attackRange;
    public float projectile;
    public float projectileSpeed;
    public float projectileRange;
    public float projectileSize;


    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
        CheckAttack();
    }
    public void SetStats(EnemyStats enemy)
    {
        this.moveSpeed = enemy.moveSpeed;
        this.maxHealth = enemy.maxHealth;
        for (int i = 0; i < 4; i++)
        {
            this.damageTypes[i] = enemy.damageTypes[i];
            this.reductions[i] = enemy.reductions[i];
        }
        this.exp = enemy.exp;
        this.attackCooldown = enemy.attackCooldown;
        this.projectile = enemy.projectile;
        this.projectileSpeed = enemy.projectileSpeed;
        this.projectileSize = enemy.projectileSize;
        this.attackRange = enemy.attackRange;
        this.projectileRange = enemy.projectileRange;
        this.spreadAttack = enemy.spreadAttack;
        this.circleAttack = enemy.circleAttack;
        this.burstAttack = enemy.burstAttack;
        this.barrageAttack = enemy.barrageAttack;
        CheckAttack();
    }
    public void CheckAttack()
    {
        if (spreadAttack || circleAttack || burstAttack || barrageAttack)
        {
            canAttack = true;
        }
        else canAttack = false;
    }
    public float CalculateDamage(Enemy enemy, float damage)
    {
        if (damage < 1) damage = 1;
        if (enemy.shocked)
            return Mathf.Round(damage * (1 + (enemy.topAilmentsEffect[3] / 100)));
        else
            return Mathf.Round(damage);
    }
    public virtual void TakeDamage(Enemy enemy, float damage, bool isCrit)
    {
        damage = CalculateDamage(enemy, damage);
        if (enemy.gameObject.activeSelf)
        {
            DamageFlash(enemy);
        }

        if (isCrit)
        {
            FloatingTextController.DisplayFloatingCritText(enemy.transform, (damage).ToString());
            GameManager.totalCrits++;
        }
        else
            FloatingTextController.DisplayFloatingText(enemy.transform, (damage).ToString());
        enemy.currentHealth -= damage;
        GameManager.totalDamageDealt += damage;
        if (enemy.currentHealth <= 0f && !enemy.isDead)
        {
            Die(enemy);
        }
    }
    public virtual void TakeDotDamage(Enemy enemy, float damage)
    {
        damage = CalculateDamage(enemy, damage);
        if (enemy.bleeding)
            FloatingTextController.DisplayDoTText(enemy.transform, (damage).ToString(), Color.white);
        else if (enemy.burned)
            FloatingTextController.DisplayDoTText(enemy.transform, (damage).ToString(), Color.red);
        enemy.currentHealth -= damage;
        GameManager.totalDamageDealt += damage;
        GameManager.TotalDotDamage += damage;
        if (enemy.currentHealth <= 0f && !enemy.isDead)
        {
            Die(enemy);
        }
    }
    public void Die(Enemy enemy)
    {
        enemy.isDead = true;
        enemyManager.gameplayManager.GainExp(exp);
        GameManager.totalKills++;
        enemyManager.enemiesAlive--;
        dropRate.DropLoot(enemy.transform);
        enemyManager.enemyDetector.RemoveEnemyFromList(enemy);
        PItemEffectManager.CheckAllPItemCondition(enemy, PItemEffectManager.ConditionTag.EnemyKilled);
        enemy.gameObject.SetActive(false);
    }

    public void KnockBack(Enemy enemy, Vector2 power) //applies knockback to the enemy
    {
        if (enemy.gameObject.activeSelf && !enemy.knockedBack && !knockBackImmune)
        {
            enemy.rb.AddForce(power, ForceMode2D.Impulse);
            enemy.knockbackTimer = 0;
            enemy.knockedBack = true;
        }
    }
    public void DamageFlash(Enemy enemy) //Flash color when hit
    {
        enemy.damageFlashTimer = 0;
        enemy.spriteRenderer.material = damageFlashMaterial;
        enemy.showDamageFlash = true;
    }
    public void ApplyChill(Enemy enemy, float chillEffect)
    {
        if (!enemy.chilled || (enemy.topAilmentsEffect[2] < chillEffect && enemy.chilled))
        {
            enemy.topAilmentsEffect[2] = chillEffect;
            enemy.moveSpeed = moveSpeed * (1 - chillEffect / 100);
            enemy.chilled = true;
            enemy.chilledTimer = 0;
            GameManager.totalChill++;
            foreach (InventoryManager.Skill sc in enemyManager.gameplayManager.inventory.activeSkillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useChillTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }
    public void ApplyBurn(Enemy enemy, float burnDamage)
    {
        if (burnDamage < 1) burnDamage = 1;
        burnDamage = Mathf.Round(burnDamage);
        if (!enemy.burned || (enemy.topAilmentsEffect[1] < burnDamage && enemy.burned))
        {
            enemy.topAilmentsEffect[1] = burnDamage;
            enemy.burned = true;
            enemy.burnedTimer = 0;
            enemy.burnOneSecCounter = 0;
            GameManager.totalBurn++;
            foreach (InventoryManager.Skill sc in enemyManager.gameplayManager.inventory.activeSkillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useBurnTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }
    public void ApplyShock(Enemy enemy, float shockEffect)
    {
        if (!enemy.shocked || (enemy.topAilmentsEffect[3] < shockEffect && enemy.shocked))
        {
            enemy.topAilmentsEffect[3] = shockEffect;
            enemy.shocked = true;
            enemy.shockedTimer = 0;
            GameManager.totalShock++;
            foreach (InventoryManager.Skill sc in enemyManager.gameplayManager.inventory.activeSkillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useShockTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }
    public void ApplyBleed(Enemy enemy, float bleedDamage)
    {
        if (bleedDamage < 1) bleedDamage = 1;
        bleedDamage = Mathf.Round(bleedDamage);
        if (!enemy.bleeding || (enemy.topAilmentsEffect[0] < bleedDamage && enemy.bleeding))
        {
            enemy.topAilmentsEffect[0] = bleedDamage;
            enemy.bleeding = true;
            enemy.bleedingTimer = 0;
            enemy.bleedOneSecCounter = 0;
            GameManager.totalBleed++;
            foreach (InventoryManager.Skill sc in enemyManager.gameplayManager.inventory.activeSkillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useBleedTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }
    public void UpdateStatusEffect(Enemy enemy)
    {
        if (enemy.gameObject.activeSelf == false) return;
        if (enemy.knockedBack) //reset velocity after knockedback
        {
            enemy.knockbackTimer += Time.deltaTime;
            if (enemy.knockbackTimer >= 0.15f)
            {
                enemy.knockedBack = false;
                enemy.rb.velocity = Vector2.zero;
                enemy.knockbackTimer = 0;
            }
        }
        if (enemy.showDamageFlash) //reset damage flash
        {
            enemy.damageFlashTimer += Time.deltaTime;
            if (enemy.damageFlashTimer >= 0.1f)
            {
                enemy.showDamageFlash = false;
                enemy.spriteRenderer.material = defaultMaterial;
                enemy.damageFlashTimer = 0;
            }
        }
        if (enemy.chilled) //chill duration
        {
            enemy.chilledTimer += Time.deltaTime;
            if (enemy.chilledTimer >= 3f)
            {
                enemy.chilled = false;
                enemy.moveSpeed = moveSpeed;
                enemy.topAilmentsEffect[2] = 0;
                enemy.chilledTimer = 0;
            }
        }
        if (enemy.shocked) //shock duration
        {
            enemy.shockedTimer += Time.deltaTime;
            if (enemy.shockedTimer >= 4f)
            {
                enemy.shocked = false;
                enemy.topAilmentsEffect[3] = 0;
                enemy.shockedTimer = 0;
            }
        }
        if (enemy.burned) //burn duration
        {
            enemy.burnedTimer += Time.deltaTime;
            if (enemy.burnedTimer >= 1f) //Take burn damage per second
            {
                TakeDotDamage(enemy, enemy.topAilmentsEffect[1]);
                GameManager.totalBurnDamage += enemy.topAilmentsEffect[1];
                enemy.burnOneSecCounter++;
                enemy.burnedTimer = 0;
            }
            if (enemy.burnOneSecCounter >= 20) //Remove burn
            {
                enemy.burned = false;
                enemy.topAilmentsEffect[1] = 0;
                enemy.burnedTimer = 0;
                enemy.burnOneSecCounter = 0;
            }
        }
        if (enemy.bleeding) //bleed duration
        {
            enemy.bleedingTimer += Time.deltaTime;
            if (enemy.bleedingTimer >= 1f) //Take bleed damage per second
            {
                TakeDotDamage(enemy, enemy.topAilmentsEffect[0]);
                GameManager.totalBleedDamage += enemy.topAilmentsEffect[0];
                enemy.bleedOneSecCounter++;
                enemy.bleedingTimer = 0;
            }
            if (enemy.bleedOneSecCounter >= 3) //Remove bleed
            {
                enemy.bleeding = false;
                enemy.topAilmentsEffect[0] = 0;
                enemy.bleedingTimer = 0;
                enemy.bleedOneSecCounter = 0;
            }
        }
    }
    public void UpdateStats()
    {
        for (int j = 0; j < gameplayManager.damageTypeMultiplier.Count; j++)
        {
            if (damageTypes[j] > 0)
                damageTypes[j] = damageTypes[j] * (1 + (gameplayManager.enemyDamageMultiplier + gameplayManager.enemyDamageTypeMultiplier[j]) / 100);
            reductions[j] = reductions[j] + gameplayManager.enemyReductions[j];
        }
        moveSpeed = moveSpeed * (1 + gameplayManager.enemyMoveSpeedMultiplier / 100);
        maxHealth = maxHealth * (1 + gameplayManager.enemyMaxHealthMultiplier / 100);
        exp = exp * (1 + gameplayManager.expMultiplier / 100);
        attackCooldown = attackCooldown * (1 + gameplayManager.enemyAttackCooldownMultiplier / 100);
        if (canAttack == true)
        {
            projectile = projectile + gameplayManager.enemyProjectileAdditive;
            projectileSpeed = projectileSpeed * (1 + gameplayManager.enemyProjectileTravelSpeedMultiplier / 100);
            //projectileSize = projectileSize * (1 + gameplayManager.enemyProjectileSizeMultiplier / 100);
        }
        else
        {
            projectile = 0;
            projectileSpeed = 0;
            projectileSize = 0;
        }
    }
}
