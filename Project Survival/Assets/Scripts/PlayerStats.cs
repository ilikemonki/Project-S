using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
public class PlayerStats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public EnemyManager enemyManager;
    public GameplayManager gameplayManager;
    public PlayerCollector playerCollector;
    public EnemyDetector enemyDetector;
    public Slider healthBar;
    public Image healthBarImage;
    public SpriteRenderer spriteRenderer;
    Material defaultMaterial;
    public Material damageFlashMaterial;
    public bool isDead;
    [Header("Base Stats")]
    public float baseMoveSpeed;
    public float baseMaxHealth;
    public float baseDefense;
    public float baseRegen, baseDegen;
    public float baseMagnetRange;
    public List<float> baseReductions;
    [Header("Current Stats")]
    public float moveSpeed;
    public float currentHealth;
    public float maxHealth;
    public float defense;
    public float regen, degen;
    public float magnetRange;
    public List<float> reductions;
    //I-Frames
    public float iFrameDuration;
    public List<GameObject> dodgeList;
    float damageFlashTimer;
    bool showDamageFlash;
    [Header("Other Modifiers")]
    public bool regenIsZero;
    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
    }
    private void Start()
    {
        InvokeRepeating(nameof(Regenerate), 1f, 1f);
        UpdatePlayerStats();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }

    private void Update()
    {
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerMovement.inIFrames)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Enemy Projectile") || collision.gameObject.CompareTag("Enemy AoE"))
            {
                if (!dodgeList.Contains(collision.gameObject))
                {
                    dodgeList.Add(collision.gameObject);
                    FloatingTextController.DisplayPlayerText(transform, "Dodge", Color.white, 0.7f);
                }
            }
            return;
        }
        if(collision.gameObject.CompareTag("Player Skill"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isDead) return;
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy.iFrameTimer > 0) return;
            try
            {
                TakeDamage(enemy.enemyStats.damageTypes); //Do damage to player
                enemy.iFrameTimer = iFrameDuration;
            }
            catch { return; }
            gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.contactTrigger, 1); //Check trigger
            return;
        }
        ICollectibles collectible = collision.GetComponent<ICollectibles>();
        if (collision.tag.Contains("Coin"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.GainCoins(collectible.coinAmount);
            PItemEffectManager.CheckAllPItemCondition(PItemEffectManager.ConditionTag.CoinCollect);
        }
        else if (collision.CompareTag("Health Potion"))
        {
            collision.gameObject.SetActive(false);
            Heal(maxHealth * 0.15f, true);
        }
        else if (collision.CompareTag("Magnet"))
        {
            collision.gameObject.SetActive(false);
            playerCollector.MagnetCollectible(200);
        }
        else if (collision.CompareTag("Skill Orb") || collision.CompareTag("Skill Gem"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.inventory.AddCollectibleIntoInventory(collision.name);
            FloatingTextController.DisplayPlayerText(transform, "+1 " + collision.name, Color.white, 0.7f);
        }
        playerCollector.collectiblesList.Remove(collectible);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerMovement.inIFrames)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Enemy Projectile") || collision.gameObject.CompareTag("Enemy AoE"))
            {
                if (!dodgeList.Contains(collision.gameObject))
                {
                    dodgeList.Add(collision.gameObject);
                    FloatingTextController.DisplayPlayerText(transform, "Dodge", Color.white, 0.7f);
                }
            }
            return;
        }
        if (collision.gameObject.CompareTag("Enemy Projectile"))
        {
            SimpleProjectile enemyProj = collision.GetComponentInParent<SimpleProjectile>();
            TakeDamage(enemyProj.damageTypes); //Do damage to player
            enemyProj.gameObject.SetActive(false);
            return;
        }
        if (collision.gameObject.CompareTag("Enemy AoE"))
        {
            AoeDamage aoe = collision.transform.GetComponent<AoeDamage>();
            aoe.hitBoxCollider.enabled = false;
            TakeDamage(aoe.damageTypes); //Do damage to player
            return;
        }
    }
    public void TakeDamage(List<float> dmgType)
    {
        if (isDead) return;
        for (int i = 0; i < dmgType.Count; i++)
        {
            if(dmgType[i] > 0) //Enemies only deals one damage type. No need to check the rest.
            {
                float totalDamage = Mathf.Round((dmgType[i] - defense) * (1 - reductions[i] / 100));
                if (totalDamage <= 0)
                    totalDamage = 1;
                if (gameObject.activeSelf)
                {
                    DamageFlash();
                }
                currentHealth -= totalDamage;
                UpdateHealthBar();
                FloatingTextController.DisplayPlayerText(transform, "-" + (totalDamage).ToString(), Color.red, 0.7f);
                GameManager.totalDamageTaken += totalDamage;
                gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.damageTakenTrigger, totalDamage); //Check trigger
                PItemEffectManager.CheckAllPItemCondition(totalDamage, PItemEffectManager.ConditionTag.DamageTaken);
                break;
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TakeDegenDamage(float dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        PItemEffectManager.CheckAllPItemCondition(dmg, PItemEffectManager.ConditionTag.Degen);
        UpdateHealthBar();
    }
    public void UpdateMaxHealthBar()   //set health bar to current max
    {
        healthBar.maxValue = maxHealth;
        CheckHPBarColor();
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
        CheckHPBarColor();
    }

    void Die()
    {
        isDead = true;
    }

    public void Heal(float amt, bool showFloatingText)
    {
        if (currentHealth == maxHealth || isDead) return;
        if (currentHealth + amt > maxHealth)
        {
            if (showFloatingText)
                FloatingTextController.DisplayPlayerText(transform, "+" + (maxHealth - currentHealth).ToString(), Color.green, 0.7f);
            currentHealth = maxHealth;
            GameManager.totalHealing += maxHealth - currentHealth;
            gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.healTrigger, maxHealth - currentHealth); //Check trigger
        }
        else
        {
            if (showFloatingText)
                FloatingTextController.DisplayPlayerText(transform, "+" + (amt).ToString(), Color.green, 0.7f);
            currentHealth += amt;
            GameManager.totalHealing += amt;
            gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.healTrigger, amt); //Check trigger
        }
        UpdateHealthBar();
    }

    public void Regenerate()
    {
        if (isDead) return;
        float amt = regen - degen;
        if (amt > 0) //regen
        {
            if (amt + currentHealth > maxHealth)
            {
                Heal(maxHealth - currentHealth, false);
                GameManager.totalRegen += maxHealth - currentHealth;
            }
            else
            {
                Heal(amt, false);
                GameManager.totalRegen += amt;
            }
            GameManager.totalDegen += degen;
            gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.degenTrigger, degen); //Check trigger
        }
        else if (amt < 0 && currentHealth != 1) //Degen. Do not degen after 1 hp
        {
            amt = Mathf.Abs(amt); //make it positive
            if (amt >= currentHealth)
            {
                TakeDegenDamage(currentHealth - 1);
                GameManager.totalDegen += currentHealth - 1;
                gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.degenTrigger, currentHealth - 1); //Check trigger
            }
            else
            {
                TakeDegenDamage(amt);
                GameManager.totalDegen += amt;
                gameplayManager.UpdateTriggerCounter(SkillTrigger.TriggerType.degenTrigger, amt); //Check trigger
            }
        }
    }

    public void CheckHPBarColor()
    {
        gameplayManager.hpText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        if (currentHealth <= 0)
        {
            return;
        }
        float hpPercent = currentHealth * 100 / maxHealth;
        if (hpPercent <= 30)
        {
            healthBarImage.color = Color.red;
        }
        else if (hpPercent <= 60)
        {
            healthBarImage.color = Color.yellow;
        }
        else
        {
            healthBarImage.color = Color.green;
        }
    }

    public void UpdatePlayerStats()
    {
        moveSpeed = baseMoveSpeed * (1 + gameplayManager.moveSpeedMultiplier / 100);
        maxHealth = baseMaxHealth * (1 + gameplayManager.maxHealthMultiplier / 100);
        defense = baseDefense * (1 + gameplayManager.defenseMultiplier / 100);
        for(int i = 0; i < reductions.Count; i++)
        {
            reductions[i] = baseReductions[i] + gameplayManager.reductionsAdditive[i];
        }
        if (regenIsZero) regen = 0;
        else regen = baseRegen + gameplayManager.regenAdditive;
        degen = baseDegen + gameplayManager.degenAdditive;
        magnetRange = baseMagnetRange * (1 + gameplayManager.magnetRangeMultiplier / 100);
        playerCollector.SetMagnetRange(magnetRange);
        PItemEffectManager.CheckAllPItemCondition(PItemEffectManager.ConditionTag.UpdatingStats);

        UpdateMaxHealthBar();
        gameplayManager.UpdateRegenText();
    }
    public void DamageFlash() //Flash color when hit
    {
        damageFlashTimer = 0;
        spriteRenderer.material = damageFlashMaterial;
        showDamageFlash = true;
    }
}
