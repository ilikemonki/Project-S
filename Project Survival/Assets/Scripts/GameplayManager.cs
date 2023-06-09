using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public int totalKills;
    public int roundCounter;
    public TextMeshProUGUI killsNeededText;
    public List<int> killsNeededInRound;    //make sure to have same amount for killCounters too.
    public List<int> killCounters;
    public int level, exp, expCap, expCapIncrease;
    public int coins;
    public PlayerStats player;
    public EnemyManager enemyManager;
    public GameObject roundUI;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI levelText;
    public Slider expSlider; 
    public ParticleSystem expSliderParticle;
    public TextMeshProUGUI coinText;
    public Vector3 mousePos;

    //Player/Skill Global Multipliers
    public float damageMultiplier;
    public float speedMultiplier;
    public float attackRangeMultiplier;
    public float chainRangeMultiplier;
    public float cooldownMultiplier;
    public float moveSpeedMultiplier;
    public float maxHealthMultiplier;
    public float defenseMultiplier;
    public float criticalChanceMultiplier, criticalDamageMultiplier;
    public float regenMultiplier;
    public float magnetRangeMultiplier;
    public float chillChanceMultiplier, burnChanceMultiplier, shockChanceMultiplier, bleedChanceMultiplier;
    public float chillSlowMultiplier, burnDamageMultiplier, shockDamageMultiplier, bleedDamageMultiplier;
    public float knockBackMultiplier;


    //Enemy Global Multipliers
    public float enemyMoveSpeedMultiplier;
    public float enemyMaxHealthMultiplier;
    public float enemyDamageMultiplier;
    public int enemyExpMultiplier;
    public float enemyFireResMultiplier, enemyColdResMultiplier, enemyLightningResMultiplier, enemyPhysicalResMultiplier;

    private void Start()
    {
        UpdateKillText();
        UpdateCoinText();
        expSlider.maxValue = expCap;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    }
    public void GainExp(int amt)
    {
        exp += amt;
        if (exp >= expCap)  //Level UP
        {
            if (expSliderParticle != null) expSliderParticle.Play();
            level++;
            UpdateLevelText();
            exp -= expCap;
            expCap += expCapIncrease;
            expSlider.maxValue = expCap;
        }
        UpdateExpBar();
    }
    public void CheckKillRequirement()
    {
        if (killCounters[roundCounter] >= killsNeededInRound[roundCounter])
        {
            GoToNextRound();
            UpdateRoundText();
            UpdateKillText();
        }
    }
    //called when enemy dies
    public void CalculateKillCounter()
    {
        totalKills++;
        killCounters[roundCounter]++;
        UpdateKillText();
        CheckKillRequirement();
    }
    public void GainCoins(int amt)
    {
        coins += amt;
        UpdateCoinText();
    }
    public void UpdateCoinText()
    {
        coinText.text = coins.ToString();
    }

    public void UpdateKillText()
    {
        killsNeededText.text = (killsNeededInRound[roundCounter] - killCounters[roundCounter]).ToString();
    }

    public void UpdateRoundText()
    {
        roundText.text = "Round " + (roundCounter + 1);
    }
    public void UpdateLevelText()
    {
        levelText.text = "Lv. " + level;
    }
    public void UpdateExpBar()
    {
        expSlider.value = exp;
    }
    public void GoToNextRound()
    {
        if (roundCounter < enemyManager.rounds.Count - 1)
        {
            roundCounter++;
        }
    }
}
