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
    public LevelUpManager levelUpManager;
    public InventoryManager inventory;
    public ItemManager itemManager;
    public GameObject waveUI;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI levelText;
    public Slider expSlider; 
    public ParticleSystem expSliderParticle;
    public TextMeshProUGUI coinText, hpText, regenText, expText, dashText, dashTimerText;
    public Vector3 mousePos;
    public Camera cam;
    public float furthestAttackRange;    //Gets furthest range between all skills, used in EnemyDistances to find targets within the range.
    [Header("Game Stats")]
    public int waveCounter;
    public float timer, maxTimer;
    public int level;
    public float exp, expCap, expCapIncrease;
    public int coins; 
    public List<int> skillExpCapList;   //amount of exp needed for next level. For skills.
    public int expOrbBonus; //Skill gains this amount of exp if they already have that orb.
    [Header("Player Multipliers")]
    //Player/Skill Global Multipliers
    public float damageMultiplier, projectileDamageMultiplier, meleeDamageMultiplier;
    public float baseDamageAdditive, baseProjectileDamageAdditive, baseMeleeDamageAdditive;
    public List<float> baseDamageTypeAdditive;
    public List<float> damageTypeMultiplier;
    public int strikeAdditive, comboAdditive, projectileAdditive, pierceAdditive, chainAdditive;
    public float attackRangeMultiplier, projectileAttackRangeMultiplier, meleeAttackRangeMultiplier;
    public float durationMultiplier;
    public float cooldownMultiplier, projectileCooldownMultiplier, meleeCooldownMultiplier;
    public float moveSpeedMultiplier;
    public float maxHealthMultiplier;
    public float defenseMultiplier;
    public float criticalChanceAdditive, projectileCriticalChanceAdditive, meleeCriticalChanceAdditive, criticalDamageAdditive, projectileCriticalDamageAdditive, meleeCriticalDamageAdditive;
    public float sizeMultiplier, projectileSizeMultiplier, meleeSizeMultiplier;
    public float regenAdditive, degenAdditive, lifeStealChanceAdditive, meleeLifeStealChanceAdditive, projectileLifeStealChanceAdditive, lifeStealAdditive, meleeLifeStealAdditive, projectileLifeStealAdditive;
    public float magnetRangeMultiplier;
    public List<float> ailmentsChanceAdditive;
    public List<float> baseAilmentsEffect;
    public List<float> ailmentsEffectMultiplier;
    public int dashChargesAdditive, dashCooldownMultiplier;
    public float dashPowerMultiplier;
    public float travelRangeMultipiler, projectileTravelRangeMultipiler, meleeTravelRangeMultipiler, travelSpeedMultipiler, projectileTravelSpeedMultipiler, meleeTravelSpeedMultipiler;
    public float expMultiplier;
    public float dropChanceMultiplier, coinDropChanceMultipiler;
    [Header("Enemy Multipliers")]
    //Enemy Global Multipliers
    public float enemyMoveSpeedMultiplier;
    public float enemyMaxHealthMultiplier;
    public float enemyDamageMultiplier;
    public float enemyAttackCooldownMultiplier;
    public float enemyProjectileAdditive;
    public float enemyProjectileTravelSpeedMultiplier;
    //public float enemyProjectileTravelRangeMultiplier;
    public float enemyProjectileSizeMultiplier;
    public List<float> enemyResistances;//[0]physical,[1]fire,[2]cold,[3]lightning
    //add enemy spawn rate

    private void Start()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(0, 1, 1);
        timer = maxTimer;
        UpdateCoinText();
        UpdateDashText();
        UpdateExpBar();
        expSlider.maxValue = expCap;
        hpText.text = player.currentHealth.ToString() + "/" + player.maxHealth.ToString();
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
    public void GainExp(float amt)
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
            if (!levelUpManager.stopLevelUp) levelUpManager.OpenUI();
        }
        UpdateExpBar();
        //Add exp to active skill
        for (int i = 0; i < inventory.activeSkillList.Count; i++)
        {
            if (inventory.activeSkillList[i].skillController != null)
            {
                inventory.activeSkillList[i].skillController.GainExp(amt);
            }
        }
    }
    public void DevOnlyLevelUp()
    {
        level++;
        UpdateLevelText();
        expCap += expCapIncrease;
        expSlider.maxValue = expCap;
        levelUpManager.OpenUI();

    }
    public void GainCoins(int amt)
    {
        coins += amt;
        GameManager.totalCoinsCollected += amt;
        UpdateCoinText();
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
        expText.text = exp.ToString() + "/" + expCap;
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
