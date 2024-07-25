using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using static SkillTrigger;

public class GameplayManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public EnemyUpgradeManager enemyUpgradeManager;
    public LevelUpManager levelUpManager;
    public InventoryManager inventory;
    public ItemManager itemManager;
    public GameObject waveUI;
    public TextMeshProUGUI waveText, aliveMobsText;
    public TextMeshProUGUI levelText;
    public Slider expSlider; 
    public ParticleSystem expSliderParticle;
    public TextMeshProUGUI coinText, hpText, regenText, expText;
    public Vector3 mousePos;
    public Camera cam;
    public float furthestAttackRange;    //Gets furthest range between all skills, used in EnemyDistances to find targets within the range.
    [Header("Game Stats")]
    public int waveCounter, maxWave;
    public float timer, maxTimer;
    public int level;
    public float exp, expCap, expCapIncrease;
    public int coins; 
    public List<int> skillExpCapList;   //amount of exp needed for next level. For skills.
    public int expOrbBonus; //Skill gains this amount of exp if they already have that orb.
    [Header("Player Multipliers")]
    //Player/Skill Global Multipliers
    public float damageMultiplier;
    public float projectileDamageMultiplier, meleeDamageMultiplier;
    public List<float> baseDamageTypeAdditive;
    public List<float> damageTypeMultiplier;
    public int meleeAmountAdditive, comboAdditive, projectileAmountAdditive, pierceAdditive, chainAdditive;
    public float attackRangeMultiplier, projectileAttackRangeMultiplier, meleeAttackRangeMultiplier;
    public float durationMultiplier;
    public float cooldownMultiplier, projectileCooldownMultiplier, meleeCooldownMultiplier;
    public float moveSpeedMultiplier;
    public float maxHealthMultiplier;
    public float defenseMultiplier;
    public float criticalChanceAdditive, projectileCriticalChanceAdditive, meleeCriticalChanceAdditive, criticalDamageAdditive, projectileCriticalDamageAdditive, meleeCriticalDamageAdditive;
    public float aoeMultiplier, projectileAoeMultiplier, meleeAoeMultiplier;
    public float regenAdditive, degenAdditive, lifeStealChanceAdditive, meleeLifeStealChanceAdditive, projectileLifeStealChanceAdditive, lifeStealAdditive, meleeLifeStealAdditive, projectileLifeStealAdditive;
    public float magnetRangeMultiplier;
    public List<float> ailmentsChanceAdditive;
    public List<float> baseAilmentsEffect;
    public List<float> ailmentsEffectMultiplier;
    public int dashChargesAdditive, dashCooldownMultiplier;
    public float dashPowerMultiplier;
    public float travelRangeMultiplier, projectileTravelRangeMultiplier, meleeTravelRangeMultiplier, travelSpeedMultiplier, projectileTravelSpeedMultiplier, meleeTravelSpeedMultiplier;
    public float expMultiplier;
    public float dropChanceMultiplier, coinDropChanceMultiplier;
    public List<float> reductionsAdditive;
    public int freeLevelupRerollAdditive, freeShopRerollAdditive; 
    [Header("Enemy Multipliers")]
    //Enemy Global Multipliers
    public float enemyMoveSpeedMultiplier;
    public float enemyMaxHealthMultiplier;
    public float enemyDamageMultiplier;
    public List<float> enemyDamageTypeMultiplier;
    public float enemyAttackCooldownMultiplier;
    public float enemyProjectileAdditive;
    public float enemyProjectileTravelSpeedMultiplier;
    public float enemyAoEMultiplier;
    public List<float> enemyReductions;//[0]physical,[1]fire,[2]cold,[3]lightning
    //add enemy spawn rate

    private void Start()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(0, 1, 1);
        timer = maxTimer;
        UpdateCoinText();
        UpdateExpBar();
        expSlider.maxValue = expCap;
        hpText.text = player.currentHealth.ToString() + "/" + player.maxHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        UpdateAliveMobsText();
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
            timer = maxTimer;
            enemyUpgradeManager.OpenUI();
            //GoToNextRound();
        }
    }
    public void UpdateAliveMobsText()
    {
        aliveMobsText.text = enemyManager.enemiesAlive.ToString();
    }
    public void GainExp(float amt)
    {
        exp += amt;
        GameManager.totalExp += amt;
        if (exp >= expCap)  //Level UP
        {
            if (expSliderParticle != null) expSliderParticle.Play();
            do
            {
                level++;
                levelUpManager.numberOfLevelUps++;
                exp -= expCap;
                expCap += expCapIncrease;
            }
            while (exp >= expCap);
            UpdateLevelText();
            expSlider.maxValue = expCap;
            if (!levelUpManager.stopLevelUp)
            {
                levelUpManager.OpenUI();
            }
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
        UpdateExpBar();
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
    public void UpdateRegenText()
    {
        if (player.regen - player.degen == 0) regenText.text = "";
        else if (player.regen - player.degen < 0) regenText.text = "<color=red> -" + (player.regen - player.degen).ToString() + "<color=white> HP/s";
        else regenText.text = "<color=green> +" + (player.regen - player.degen).ToString() + "<color=white> HP/s";
    }
    public void UpdateExpBar()
    {
        expSlider.value = exp;
        expText.text = exp.ToString() + "/" + expCap;
    }
    public void GoToNextRound()
    {
        waveCounter++;
        UpdateRoundText();
    }

    //Check all active trigger skill condition
    public void UpdateTriggerCounter(TriggerType triggerType, float counterAmount)
    {
        foreach (InventoryManager.Skill sc in inventory.activeSkillList)
        {
            if (sc.skillController != null)
            {
                if (sc.skillController.skillTrigger.isTriggerSkill)
                {
                    if (sc.skillController.skillTrigger.triggerType.Equals(triggerType))
                    {
                        sc.skillController.skillTrigger.currentCounter += counterAmount;
                        if (sc.skillController.currentCooldown <= 0f || triggerType.Equals(TriggerType.dashTrigger)) //Check cooldown or if some triggers bypass cooldown
                        {
                            if (sc.skillController.skillTrigger.CheckTriggerCondition())
                            {
                                sc.skillController.UseSkill();
                            }
                        }
                    }
                }
            }
        }
    }
}
