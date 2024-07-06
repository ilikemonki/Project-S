using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeMagnetEffect : PassiveItemEffect
{
    //you will draw all coins within a 3x radius of magnet range. CD
    public override void CheckCondition()
    {
        if (!effectActivated && checkCondition)
        {
            ActivateEffect();
        }
    }
    public override void ActivateEffect()
    {
        gameplayManager.player.playerCollector.MagnetCollectible(gameplayManager.player.magnetRange * 3);
        base.ActivateEffect();
    }
    public override void RemoveEffect()
    {
        pItemCDEffectUI.SetActive(false);
        effectActivated = false;
        checkCondition = false;
    }
    public override void WhenAcquired()
    {
        if (pItemCDEffectUI != null)
            pItemCDEffectUI.SetActive(true);
        effectActivated = true;
        checkCondition = true;
        currentCD = cooldown;
    }
}
