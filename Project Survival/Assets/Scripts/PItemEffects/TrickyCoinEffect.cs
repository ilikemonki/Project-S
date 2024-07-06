using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickyCoinEffect : PassiveItemEffect
{
    //Chance to gain 1 coin every time player collects coin. <br>+10% chance per item.
    public override void CheckCondition()
    {
        if (checkCondition)
        {
            if (Random.Range(1, 101) <= chance)
                ActivateEffect();
        }
    }
    public override void ActivateEffect()
    {
        gameplayManager.GainCoins(1);
        totalRecorded++;
    }
    public override void RemoveEffect()
    {
        chance = 25 * (itemDesc.quantityInInventory - itemDesc.quantityDisabledInInventory);
        if (chance == 0) chance = 25;
        if (itemDesc.quantityDisabledInInventory >= itemDesc.quantityInInventory)
        {
            checkCondition = false;
        }
    }
    public override void WhenAcquired()
    {
        chance = 25 * itemDesc.quantityInInventory;
        checkCondition = true;
    }
}
