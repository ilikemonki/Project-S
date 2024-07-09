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
    //Stats
    public float currentHealth;
    public float maxHealth;
    public List<float> damageTypes;
    public List<float> reductions;
    public float moveSpeed, baseMoveSpeed; //baseMoveSpeed is the current movespeed, use to reset current movespeed back to normal when chilled.
    public float exp;
    public bool chilled, burned, shocked, bleeding;
    public List<float> topAilmentsEffect;
    //public List<float> ailmentsCounter;
    public SpriteRenderer spriteRenderer;
    float totalBurnDamage, totalBleedDamage;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    Material defaultMaterial;
    public Material damageFlashMaterial;
    public DropRate dropRate;
    public bool knockedBack, knockBackImmune;
    public bool isSpawning; //check if enemy is beginning to spawn
    bool isDead, showDamageFlash;
    public bool doNotMove;
    [Header("Attack")]
    public bool canAttack; //initialize in awake
    public bool spreadAttack, circleAttack, burstAttack, barrageAttack;
    public float barrageCooldown, barrageCounter; //For barrage
    public float attackCooldown, attackTimer, attackRange;
    public float projectile;
    public float projectileSpeed;
    public float projectileRange;
    public float projectileSize;
    public bool isAttacking;
    public GameObject attackImage;
    //Status Timers
    float knockbackTimer, damageFlashTimer, chilledTimer, burnedTimer, shockedTimer, bleedingTimer;
    int burnOneSecCounter, bleedOneSecCounter; //When timer hits 1 sec, increment counter. 5 counters mean 5 seconds


    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
        CheckAttack();
    }
    public void SetStats(EnemyStats enemy)
    {
        this.baseMoveSpeed = enemy.baseMoveSpeed;
        moveSpeed = enemy.moveSpeed;
        this.maxHealth = enemy.maxHealth;
        currentHealth = enemy.maxHealth;
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
    public void SetNonModifiedStats(EnemyStats enemyPrefab)
    {
    }
    public void CheckAttack()
    {
        if (spreadAttack || circleAttack || burstAttack || barrageAttack)
        {
            canAttack = true;
        }
        else canAttack = false;
    }
    public float CalculateDamage(float damage)
    {
        if (damage < 1) damage = 1;
        if (shocked)
            return Mathf.Round(damage * (1 + (topAilmentsEffect[3] / 100)));
        else
            return Mathf.Round(damage);
    }
    public virtual void TakeDamage(float damage, bool isCrit)
    {
        damage = CalculateDamage(damage);
        if (gameObject.activeSelf)
        {
            DamageFlash();
        }

        if (isCrit)
        {
            FloatingTextController.DisplayFloatingCritText(transform, (damage).ToString());
            GameManager.totalCrits++;
        }
        else
            FloatingTextController.DisplayFloatingText(transform, (damage).ToString());
        currentHealth -= damage;
        GameManager.totalDamageDealt += damage;
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    public virtual void TakeDotDamage(float damage)
    {
        damage = CalculateDamage(damage);
        if (bleeding)
            FloatingTextController.DisplayDoTText(transform, (damage).ToString(), Color.white);
        else if (burned)
            FloatingTextController.DisplayDoTText(transform, (damage).ToString(), Color.red);
        currentHealth -= damage;
        GameManager.totalDamageDealt += damage;
        GameManager.TotalDotDamage += damage;
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    public void Die()
    {
        isDead = true;
        enemyManager.gameplayManager.GainExp(exp);
        GameManager.totalKills++;
        enemyManager.enemiesAlive--;
        dropRate.DropLoot(transform);
        enemyManager.enemyDetector.RemoveEnemyFromList(this);
        PItemEffectManager.CheckAllPItemCondition(gameObject, PItemEffectManager.ConditionTag.EnemyKilled);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isAttacking = false;
        attackImage.SetActive(false);
        spriteRenderer.material = defaultMaterial;
        knockedBack = false;
        isSpawning = false;
        //for (int i = 0; i < topAilmentsEffect.Count; i++) //Reset at EnemyManager
        //{
        //    topAilmentsEffect[i] = 0;
        //}
        //chilled = false; burned = false; shocked = false; bleeding = false;
        totalBurnDamage = 0; totalBleedDamage = 0;
        attackTimer = attackCooldown;
        damageFlashTimer = 0;
        burnedTimer = 0; chilledTimer = 0; shockedTimer = 0; bleedingTimer = 0;
        burnOneSecCounter = 0; bleedOneSecCounter = 0; 
        if (barrageAttack)
        {
            barrageCooldown = 0; barrageCounter = 0;
        }
        if (enemyManager != null)
            enemyManager.enemyDetector.RemoveEnemyFromList(this);
    }
    protected virtual void OnEnable()
    {
        isDead = false;
    }
    public void KnockBack(Vector2 power) //applies knockback to the enemy
    {
        if (gameObject.activeSelf && !knockedBack && !knockBackImmune)
        {
            rb.AddForce(power, ForceMode2D.Impulse);
            knockbackTimer = 0;
            knockedBack = true;
        }
    }
    public void DamageFlash() //Flash color when hit
    {
        damageFlashTimer = 0;
        spriteRenderer.material = damageFlashMaterial;
        showDamageFlash = true;
    }
    public void ApplyChill(float chillEffect)
    {
        if (!chilled || (topAilmentsEffect[2] < chillEffect && chilled))
        {
            topAilmentsEffect[2] = chillEffect;
            moveSpeed = baseMoveSpeed * (1 - chillEffect / 100);
            chilled = true;
            chilledTimer = 0;
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
    public void ApplyBurn(float burnDamage)
    {
        if (burnDamage < 1) burnDamage = 1;
        burnDamage = Mathf.Round(burnDamage);
        if (!burned || (topAilmentsEffect[1] < burnDamage && burned))
        {
            topAilmentsEffect[1] = burnDamage;
            burned = true;
            burnedTimer = 0;
            burnOneSecCounter = 0;
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
    public void ApplyShock(float shockEffect)
    {
        if (!shocked || (topAilmentsEffect[3] < shockEffect && shocked))
        {
            topAilmentsEffect[3] = shockEffect;
            shocked = true;
            shockedTimer = 0;
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
    public void ApplyBleed(float bleedDamage)
    {
        if (bleedDamage < 1) bleedDamage = 1;
        bleedDamage = Mathf.Round(bleedDamage);
        if (!bleeding || (topAilmentsEffect[0] < bleedDamage && bleeding))
        {
            topAilmentsEffect[0] = bleedDamage;
            bleeding = true;
            bleedingTimer = 0;
            bleedOneSecCounter = 0;
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
    public void UpdateStatusEffect()
    {
        if (gameObject.activeSelf == false) return;
        if (knockedBack) //reset velocity after knockedback
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= 0.15f)
            {
                knockedBack = false;
                rb.velocity = Vector2.zero;
                knockbackTimer = 0;
            }
        }
        if (showDamageFlash) //reset damage flash
        {
            damageFlashTimer += Time.deltaTime;
            if (damageFlashTimer >= 0.1f)
            {
                showDamageFlash = false;
                spriteRenderer.material = defaultMaterial;
                damageFlashTimer = 0;
            }
        }
        if (chilled) //chill duration
        {
            chilledTimer += Time.deltaTime;
            if (chilledTimer >= 3f)
            {
                chilled = false;
                moveSpeed = baseMoveSpeed;
                topAilmentsEffect[2] = 0;
                chilledTimer = 0;
            }
        }
        if (shocked) //shock duration
        {
            shockedTimer += Time.deltaTime;
            if (shockedTimer >= 4f)
            {
                shocked = false;
                topAilmentsEffect[3] = 0;
                shockedTimer = 0;
            }
        }
        if (burned) //burn duration
        {
            burnedTimer += Time.deltaTime;
            if (burnedTimer >= 1f) //Take burn damage per second
            {
                totalBurnDamage += Mathf.Round(topAilmentsEffect[1]);
                TakeDotDamage(topAilmentsEffect[1]);
                GameManager.totalBurnDamage += topAilmentsEffect[1];
                burnOneSecCounter++;
                burnedTimer = 0;
            }
            if (burnOneSecCounter >= 20) //Remove burn
            {
                burned = false;
                topAilmentsEffect[1] = 0;
                burnedTimer = 0;
                burnOneSecCounter = 0;
                totalBurnDamage = 0; 
            }
        }
        if (bleeding) //bleed duration
        {
            bleedingTimer += Time.deltaTime;
            if (bleedingTimer >= 1f) //Take bleed damage per second
            {
                totalBleedDamage += Mathf.Round(topAilmentsEffect[0]);
                TakeDotDamage(topAilmentsEffect[0]);
                GameManager.totalBleedDamage += topAilmentsEffect[0];
                bleedOneSecCounter++;
                bleedingTimer = 0;
            }
            if (bleedOneSecCounter >= 3) //Remove bleed
            {
                bleeding = false;
                topAilmentsEffect[0] = 0;
                bleedingTimer = 0;
                bleedOneSecCounter = 0;
                totalBleedDamage = 0;
            }
        }
    }
}
