using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceEffect : PassiveItemEffect
{
    //Acquiring all will grant player +1 proj and melee.
    public bool effectRemoved;
    public override void CheckCondition()
    {
        if (!effectActivated && checkCondition)
        {
            if (itemDesc.quantityInInventory >= itemDesc.maxQuantity && itemDesc.quantityDisabledInInventory <= 0)
                ActivateEffect();
        }
        //base.CheckCondition(valueToCheck);
    }
    public override void ActivateEffect()
    {
        effectRemoved = false;
        gameplayManager.projectileAmountAdditive += 1;
        gameplayManager.meleeAmountAdditive += 1;
        base.ActivateEffect();
    }
    public override void RemoveEffect()
    {
        if (itemDesc.quantityDisabledInInventory > 0 && !effectRemoved && effectActivated)
        {
            checkCondition = false;
            effectActivated = false;
            effectRemoved = true;
            gameplayManager.projectileAmountAdditive -= 1;
            gameplayManager.meleeAmountAdditive -= 1;
            base.RemoveEffect();
        }
    }
    public override void WhenAcquired()
    {
        checkCondition = true;
        CheckCondition();
    }
}
