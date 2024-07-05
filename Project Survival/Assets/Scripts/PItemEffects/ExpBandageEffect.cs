using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBandageEffect : PassiveItemEffect
{
    //Gain exp per damage taken
    public float expAmount;
    public override void CheckCondition(float valueToCheck)
    {
        if (!effectActivated && checkCondition)
        {
            expAmount = valueToCheck;
            ActivateEffect();
        }
        //base.CheckCondition(valueToCheck);
    }
    public override void ActivateEffect()
    {
        gameplayManager.GainExp(expAmount);
        FloatingTextController.DisplayPlayerText(gameplayManager.player.transform, " +" + expAmount.ToString() +" Exp", Color.yellow, 1f);
        totalRecorded += expAmount;
        base.ActivateEffect();
    }
}
