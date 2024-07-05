using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassiveItemEffect : MonoBehaviour
{
    public ItemDescription itemDesc;
    public GameplayManager gameplayManager;
    public PItemEffectManager.ConditionTag conditionTag;
    public float cooldown, currentCD;
    public bool checkCondition, effectActivated;
    public float totalRecorded;
    public TextMeshProUGUI cdText;
    public virtual void CheckCondition(float valueToCheck)
    {

    }
    public virtual void CheckCondition()
    {

    }
    public virtual void ActivateEffect()
    {
        if (cooldown > 0)
        {
            currentCD = cooldown;
            checkCondition = true;
        }
        else
        {
            checkCondition = false;
        }
        effectActivated = true;
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public virtual void RemoveEffect()
    {
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public virtual void UpdateCooldown()
    {
        if (effectActivated && cooldown > 0) //Reset CD
        {
            currentCD -= Time.deltaTime;
            if (cdText != null)
            {
                cdText.text = string.Format("{0:0}", currentCD);
            }
            if (currentCD <= 0)
            {
                cdText.text = "";
                effectActivated = false;
            }
        }
    }
}
