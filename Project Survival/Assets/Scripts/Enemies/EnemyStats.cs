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
    public float damage;
    public float moveSpeed, baseMoveSpeed;
    public int exp;
    public bool chilled, burned, shocked, bleeding;
    public List<float> topAilmentsEffect;
    public List<float> ailmentsCounter;
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI dotTextDamage;
    float totalBurnDamage, totalBleedDamage;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    Material defaultMaterial;
    public Material damageFlashMaterial;
    public DropRate dropRate;
    public bool knockedBack, knockBackImmune;
    public bool isSpawning; //check if enemy is beginning to spawn
    bool isDead;
    public bool canAttack; //initialize in awake
    public bool spreadAttack, circleAttack;
    public float attackCooldown, attackTimer, attackRange;
    public float projectiles;
    public float projectileSpeed;
    public float projectileDuration;

    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
        CheckAttack();
    }
    public void SetStats(float baseMoveSpeed, float maxHealth, float damage, int exp, float attackCooldown, float projectileSpeed)
    {
        this.baseMoveSpeed = baseMoveSpeed;
        moveSpeed = baseMoveSpeed;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.damage = damage;
        this.exp = exp;
        this.attackCooldown = attackCooldown;
        this.projectileSpeed = projectileSpeed;
    }
    public void SetNonModifiedStats(float attackRange, float projectiles, bool spreadAttack, bool circleAttack, float projectileDuration)
    {
        this.attackRange = attackRange;
        this.projectiles = projectiles;
        this.spreadAttack = spreadAttack;
        this.circleAttack = circleAttack;
        this.projectileDuration = projectileDuration;
        CheckAttack();
    }
    public float CalculateDamage(float damage)
    {
        if (damage < 1) damage = 1;
        if (shocked)
            return Mathf.Round(damage * (1 + (topAilmentsEffect[3] / 100)));
        else
            return Mathf.Round(damage);
    }
    public virtual void TakeDamage(float damage, bool isCrit, Color textColor)
    {
        damage = CalculateDamage(damage);
        if (gameObject.activeSelf)
        {
            Timing.RunCoroutine(DamageFlash());
        }

        if (isCrit)
            enemyManager.floatingTextController.DisplayFloatingCritText(transform, damage, textColor);
        else
            enemyManager.floatingTextController.DisplayFloatingText(transform, damage, textColor);
        currentHealth -= damage;
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    public virtual void TakeDotDamage(float damage)
    {
        damage = CalculateDamage(damage);
        if (bleeding && burned)
            dotTextDamage.text = totalBleedDamage.ToString() + " " + "<color=red>" + totalBurnDamage.ToString();
        else if (bleeding)
            dotTextDamage.text = totalBleedDamage.ToString();
        else if (burned)
            dotTextDamage.text = "<color=red>" + totalBurnDamage.ToString();
        currentHealth -= damage;
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    public void CheckAttack()
    {
        if (spreadAttack || circleAttack)
        {
            canAttack = true;
        }
        else canAttack = false;
    }
    public void Die()
    {
        isDead = true;
        dropRate.DropLoot(transform);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        spriteRenderer.material = defaultMaterial;
        if (enemyManager != null)
        {
            enemyManager.gameplayMananger.GainExp(exp);
            enemyManager.gameplayMananger.CalculateKillCounter();
            enemyManager.enemiesAlive--;
        }
        knockedBack = false;
        isSpawning = false;
        chilled = false; burned = false; shocked = false; bleeding = false;
        totalBurnDamage = 0; totalBleedDamage = 0;
        attackTimer = attackCooldown;
        dotTextDamage.text = "";
        for(int i = 0; i > topAilmentsEffect.Count; i++)
        {
            topAilmentsEffect[i] = 0;
        }
    }
    protected virtual void OnEnable()
    {
        isDead = false;
    }
    public IEnumerator<float> ResetVelocity()
    {
        yield return Timing.WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        knockedBack = false;
    }

    public void KnockBack(Vector2 power)
    {
        if (gameObject.activeSelf)
        {
            knockedBack = true;
            rb.AddForce(power, ForceMode2D.Impulse);
            Timing.RunCoroutine(ResetVelocity());
        }
    }

    public IEnumerator<float> DamageFlash()
    {
        spriteRenderer.material = damageFlashMaterial;
        yield return Timing.WaitForSeconds(0.1f);
        spriteRenderer.material = defaultMaterial;
    }

    
    public void ApplyChill(float chillEffect)
    {
        if (!chilled)
        {
            moveSpeed = baseMoveSpeed * (1 - chillEffect / 100);
            ailmentsCounter[2] = 3;
            topAilmentsEffect[2] = chillEffect;
            Timing.RunCoroutine(SlowMovement());
        }
        else
        {
            ailmentsCounter[2] = 3; //reset duration
        }
        if(topAilmentsEffect[2] < chillEffect && chilled)   //if chill is higher and mob is already chilled, rechill.
        {
            topAilmentsEffect[2] = chillEffect;
            moveSpeed = baseMoveSpeed * (1 - chillEffect / 100);
        }
    }
    public void ApplyBurn(float burnDamage)
    {
        if (burnDamage < 1) burnDamage = 1;
        burnDamage = Mathf.Round(burnDamage);
        if (!burned)
        {
            ailmentsCounter[1] = 5; 
            topAilmentsEffect[1] = burnDamage;
            Timing.RunCoroutine(TakeBurnDamage());
        }
        else
        {
            ailmentsCounter[1] = 5; //reset duration
        }
        if (topAilmentsEffect[1] < burnDamage && burned)   //if burn damage is higher and mob is already burned, reburn.
        {
            topAilmentsEffect[1] = burnDamage;
        }
    }
    public void ApplyShock(float shockEffect)
    {
        if (!shocked)
        {
            ailmentsCounter[3] = 4;
            topAilmentsEffect[3] = shockEffect;
            Timing.RunCoroutine(TakeShockEffect());
        }
        else
        {
            ailmentsCounter[3] = 4; //reset duration
        }
        if (topAilmentsEffect[3] < shockEffect && shocked)   //if burn damage is higher and mob is already burned, reburn.
        {
            topAilmentsEffect[3] = shockEffect;
        }
    }
    public void ApplyBleed(float bleedDamage)
    {
        if (bleedDamage < 1) bleedDamage = 1;
        bleedDamage = Mathf.Round(bleedDamage);
        if (!bleeding)
        {
            ailmentsCounter[0] = 2;
            topAilmentsEffect[0] = bleedDamage;
            Timing.RunCoroutine(TakeBleedDamage());
        }
        else
        {
            ailmentsCounter[0] = 2; //reset duration
        }
        if (topAilmentsEffect[0] < bleedDamage && bleeding)   //if burn damage is higher and mob is already burned, reburn.
        {
            topAilmentsEffect[0] = bleedDamage;
        }
    }
    IEnumerator<float> SlowMovement()
    {
        chilled = true;
        while (ailmentsCounter[2] > 0)
        {
            yield return Timing.WaitForSeconds(1);
            ailmentsCounter[2]--;
        }
        moveSpeed = baseMoveSpeed;
        chilled = false;
        topAilmentsEffect[2] = 0;
    }
    IEnumerator<float> TakeBurnDamage()
    {
        burned = true;
        while (ailmentsCounter[1] > 0)
        {
            totalBurnDamage += Mathf.Round(topAilmentsEffect[1]);
            TakeDotDamage(topAilmentsEffect[1]);
            yield return Timing.WaitForSeconds(1f);
            ailmentsCounter[1]--;
        }
        burned = false;
        topAilmentsEffect[1] = 0;
        totalBurnDamage = 0;
        if (bleeding)
            dotTextDamage.text = totalBleedDamage.ToString();
        else
            dotTextDamage.text = "";
    }
    IEnumerator<float> TakeBleedDamage()
    {
        bleeding = true;
        while (ailmentsCounter[0] > 0)
        {
            totalBleedDamage += Mathf.Round(topAilmentsEffect[0]);
            TakeDotDamage(topAilmentsEffect[0]);
            yield return Timing.WaitForSeconds(1f);
            ailmentsCounter[0]--;
        }
        bleeding = false;
        topAilmentsEffect[0] = 0;
        totalBleedDamage = 0; 
        if (burned)
            dotTextDamage.text = "<color=red>" + totalBurnDamage.ToString();
        else
            dotTextDamage.text = "";
    }
    IEnumerator<float> TakeShockEffect()
    {
        shocked = true;
        while (ailmentsCounter[3] > 0)
        {
            yield return Timing.WaitForSeconds(1);
            ailmentsCounter[3]--;
        }
        shocked = false;
        topAilmentsEffect[3] = 0;
    }
}
