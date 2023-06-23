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
    public TextMeshProUGUI timerText;
    public float timer, maxTimer;
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
    public float defenseAdditive;
    public float criticalChanceAdditive, criticalDamageAdditive;
    public float regenAdditive;
    public float magnetRangeMultiplier;
    public List<float> ailmentsChanceAdditive;
    public List<float> ailmentsEffectAdditive;
    public float knockBackMultiplier;

    //Enemy Global Multipliers
    public float enemyMoveSpeedMultiplier;
    public float enemyMaxHealthMultiplier;
    public float enemyDamageMultiplier;
    public int enemyExpMultiplier;
    public List<float> resistances;//[0]physical,[1]fire,[2]cold,[3]lightning
    public float rareMoveSpeedMultiplier, rareHealthMultiplier, rareDamageMultiplier, rareExpMultiplier;
    public float dropChanceMultiplier;

    private void Start()
    {
        timer = maxTimer;
        UpdateCoinText();
        expSlider.maxValue = expCap;
    }

    // Update is called once per frame
    void Update()
    {
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
    public void CalculateKillCounter()
    {
        totalKills++;
    }
    public void GainCoins(int amt)
    {
        coins += amt;
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
            UpdateRoundText();
        }
    }
}
