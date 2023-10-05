using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public InventoryManager inventory;
    public GameObject waveUI;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI levelText;
    public Slider expSlider; 
    public ParticleSystem expSliderParticle;
    public TextMeshProUGUI coinText, classStarText, hpText, regenText, expText, expCapText, dashText, dashTimerText;
    public Vector3 mousePos;
    public Camera cam;
    public float maxAttackRange;    //Gets max range between all skills, used in EnemyDistances to find targets within the range.
    [Header("Game Stats")]
    public int waveCounter;
    public float timer, maxTimer;
    public int level, exp, expCap, expCapIncrease;
    public int coins, classStars;
    [Header("Player Multipliers")]
    //Player/Skill Global Multipliers
    public float damageMultiplier, projectileDamageMultiplier, meleeDamageMultiplier;
    public List<float> damageTypeMultiplier;
    public int strikeAdditive, projectileAdditive, pierceAdditive, chainAdditive;
    public float speedMultiplier;
    public float attackRangeMultiplier, projectileAttackRangeMultiplier, meleeAttackRangeMultiplier;
    public float travelRangeMultiplier;
    public float cooldownMultiplier, projectileCooldownMultiplier, meleeCooldownMultiplier;
    public float moveSpeedMultiplier;
    public float maxHealthMultiplier;
    public float defenseMultiplier;
    public float criticalChanceAdditive, projectileCriticalChanceAdditive, meleeCriticalChanceAdditive, criticalDamageAdditive, projectileCriticalDamageAdditive, meleeCriticalDamageAdditive;
    public float sizeAdditive, projectileSizeAdditive, meleeSizeAdditive;
    public float regenAdditive, degenAdditive, lifeStealChanceAdditive, lifeStealAdditive;
    public float magnetRangeMultiplier;
    public List<float> ailmentsChanceAdditive;
    public List<float> ailmentsEffectAdditive;
    public int dashChargesAdditive, dashCooldownMultiplier;
    public float dashPowerMultiplier;
    [Header("Enemy Multipliers")]
    //Enemy Global Multipliers
    public float enemyMoveSpeedMultiplier;
    public float enemyMaxHealthMultiplier;
    public float enemyDamageMultiplier;
    public int enemyExpMultiplier;
    public float enemyAttackCooldownMultiplier;
    public float enemyProjectileSpeedMultiplier;
    public List<float> resistances;//[0]physical,[1]fire,[2]cold,[3]lightning
    public float dropChanceMultiplier;

    private void Start()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(0, 1, 1);
        timer = maxTimer;
        UpdateCoinText();
        UpdateClassStarText();
        UpdateDashText();
        expSlider.maxValue = expCap;
        expCapText.text = expCap.ToString();
        hpText.text = player.currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.totalTime += Time.deltaTime;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTime(timer);
        }
        else
        {
            timer = 0;
            GoToNextRound();
        }
    }
    public void GainExp(int amt)
    {
        exp += amt;
        GameManager.totalExp += amt;
        if (exp >= expCap)  //Level UP
        {
            if (expSliderParticle != null) expSliderParticle.Play();
            level++;
            UpdateLevelText();
            exp -= expCap;
            expCap += expCapIncrease;
            expSlider.maxValue = expCap;
            expCapText.text = expCap.ToString();
        }
        UpdateExpBar();
    }
    public void GainCoins(int amt)
    {
        coins += amt;
        GameManager.totalCoinsCollected += amt;
        UpdateCoinText();
    }
    public void GainClassStars(int amt)
    {
        classStars += amt;
        GameManager.totalClassStarsCollected += amt;
        UpdateClassStarText();
    }
    public void UpdateTime(float timer)
    {
        timer += 1;
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    public void UpdateCoinText()
    {
        coinText.text = coins.ToString();
    }
    public void UpdateClassStarText()
    {
        classStarText.text = classStars.ToString();
    }

    public void UpdateRoundText()
    {
        waveText.text = "Round " + (waveCounter + 1);
    }
    public void UpdateLevelText()
    {
        levelText.text = "Lv. " + level;
    }
    public void UpdateDashText()
    {
        dashText.text = player.playerMovement.currentCharges.ToString();
    }
    public void UpdateDashTime(float timer)
    {
        dashTimerText.text = timer.ToString("F1");
    }
    public void UpdateExpBar()
    {
        expSlider.value = exp;
        expText.text = exp.ToString();
    }
    public void GoToNextRound()
    {
        if (waveCounter < enemyManager.rounds.Count - 1)
        {
            waveCounter++;
            UpdateRoundText();
        }
    }
}
