using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItemEffect : MonoBehaviour
{
    public ItemDescription itemDesc;
    public GameplayManager gameplayManager;
    public PItemEffectManager.ConditionTag conditionTag;
    public float cooldown, currentCD;
    public bool checkCondition, activateEffect;
    public virtual void CheckCondition(float valueToCheck)
    {

    }
    public virtual void ActivateEffect()
    {
        currentCD = 0;
        activateEffect = true;
    }
    public virtual void UpdateCooldown()
    {
        if (activateEffect && cooldown > 0) //Reset CD
        {
            currentCD += Time.deltaTime;
            if (currentCD >= cooldown)
            {
                activateEffect = false;
            }
        }
    }
}
