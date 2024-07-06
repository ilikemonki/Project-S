using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixFireEffect : PassiveItemEffect
{
    //Regen is set to 0. If Degen reaches 1 HP, restore all HP.
    public override void CheckCondition(float valueToCheck) //returns degen amt
    {
        if (checkCondition)
        {
            if (gameplayManager.player.currentHealth <= 1 && !gameplayManager.player.isDead)
                ActivateEffect();
        }
        //base.CheckCondition(valueToCheck);
    }
    public override void ActivateEffect()
    {
        gameplayManager.player.currentHealth = gameplayManager.player.maxHealth; 
        checkCondition = true;
    }
    public override void RemoveEffect()
    {
        gameplayManager.player.regenIsZero = false;
        checkCondition = false;
        gameplayManager.player.UpdatePlayerStats();
        base.RemoveEffect();
    }
    public override void WhenAcquired()
    {
        gameplayManager.player.regenIsZero = true;
        checkCondition = true;
        gameplayManager.player.UpdatePlayerStats();
        UpdateStats.FormatPlayerStatsToString();
    }
}
