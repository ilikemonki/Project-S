using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourHeartCloverEffect : PassiveItemEffect
{
    //When damage taken is 25% of max health or more that will kill player, set life to 1.
    public override void CheckCondition(float valueToCheck)
    {
        if (!effectActivated && checkCondition)
        {
            if (valueToCheck >= gameplayManager.player.maxHealth * 0.25f)
                ActivateEffect();
        }
        //base.CheckCondition(valueToCheck);
    }
    public override void ActivateEffect()
    {
        gameplayManager.player.currentHealth = 1;
        FloatingTextController.DisplayPlayerText(gameplayManager.player.transform, " Four-Heart Clover Activated", Color.yellow, 1.5f);
        totalRecorded++; //Saves
        base.ActivateEffect();
    }
    public override void RemoveEffect()
    {
        pItemCDEffectUI.SetActive(false);
        checkCondition = false;
    }
    public override void WhenAcquired()
    {
        if (pItemCDEffectUI != null)
            pItemCDEffectUI.SetActive(true);
        checkCondition = true;
        currentCD = cooldown;
    }
}
