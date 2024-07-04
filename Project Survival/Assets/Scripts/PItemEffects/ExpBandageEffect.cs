using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBandageEffect : PassiveItemEffect
{
    //Gain exp per damage taken
    public float expAmount;
    public override void CheckCondition(float valueToCheck)
    {
        if (!activateEffect)
        {
            expAmount = valueToCheck;
            ActivateEffect();
        }
        //base.CheckCondition(valueToCheck);
    }
    public override void ActivateEffect()
    {
        gameplayManager.GainExp(expAmount);
        FloatingTextController.DisplayPlayerText(transform, " +" + expAmount +" Exp", Color.white, 0.7f);
        base.ActivateEffect();
    }
}
