using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabidBloodEffect : PassiveItemEffect
{
    //Convert all regen into x damage.
    public float damageGained;
    public override void CheckCondition()
    {
        if (checkCondition)
        {
            ActivateEffect();
        }
    }
    public override void ActivateEffect()
    {
        if (effectActivated)
            gameplayManager.damageMultiplier -= damageGained;
        damageGained = (gameplayManager.player.baseRegen + gameplayManager.regenAdditive) * 3;
        gameplayManager.player.regen = 0;
        gameplayManager.damageMultiplier += damageGained;
        effectActivated = true;
        gameplayManager.UpdateRegenText();
        UpdateStats.UpdateAllActiveSkills();
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public override void RemoveEffect()
    {
        if (effectActivated)
            gameplayManager.damageMultiplier -= damageGained;
        damageGained = 0;
        gameplayManager.player.regen = gameplayManager.player.baseRegen + gameplayManager.regenAdditive;
        gameplayManager.UpdateRegenText();
        effectActivated = false;
        checkCondition = false;
        base.RemoveEffect();
    }
    public override void WhenAcquired()
    {
        checkCondition = true;
        ActivateEffect();
    }
}
