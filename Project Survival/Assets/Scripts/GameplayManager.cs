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


    private void Start()
    {
        UpdateKillText();
        UpdateCoinText();
        expSlider.maxValue = expCap;
    }

    // Update is called once per frame
    void Update()
    {
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
