using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public TextMeshProUGUI killsNeededText;
    public List<int> killsNeededInRound;    //make sure to have same amount for killCounters too.
    public List<int> killCounters;
    public int level, exp, expCap, expCapIncrease;
    public int coins;
    public EnemyManager enemyManager;

    private void Start()
    {
        UpdateKillText();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GainExp(int amt)
    {
        exp += amt;
        if (exp >= expCap)
        {
            level++;
            exp -= expCap;
            expCap += expCapIncrease;
        }
    }
    public void CheckKillRequirement()
    {
        if (killCounters[enemyManager.roundCounter] >= killsNeededInRound[enemyManager.roundCounter])
        {
            enemyManager.GoToNextRound();
            UpdateKillText();
        }
    }
    //called when enemy dies
    public void CalculateKillCounter()
    {
        killCounters[enemyManager.roundCounter]++;
        UpdateKillText();
        CheckKillRequirement();
    }
    public void GainCoins(int amt)
    {
        coins += amt;
    }

    public void UpdateKillText()
    {
        killsNeededText.text = (killsNeededInRound[enemyManager.roundCounter] - killCounters[enemyManager.roundCounter]).ToString();
    }
}
