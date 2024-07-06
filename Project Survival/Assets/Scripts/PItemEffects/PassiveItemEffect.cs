using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassiveItemEffect : MonoBehaviour
{
    public ItemDescription itemDesc;
    public GameplayManager gameplayManager;
    public GameObject pItemCDEffectUI; //if effect has cd and on ui screen, set this to the ui.
    public PItemEffectManager.ConditionTag conditionTag;
    public float cooldown, currentCD, chance;
    public bool checkCondition, effectActivated;
    public float totalRecorded;
    public string totalrecordedString;
    public TextMeshProUGUI cdText;
    public virtual void CheckCondition(float valueToCheck)
    {

    }
    public virtual void CheckCondition()
    {

    }
    public virtual void WhenAcquired() //Gets called when acquired or enabled again.
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
        UpdateStats.UpdateAllActiveSkills();
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public virtual void RemoveEffect()
    {
        UpdateStats.UpdateAllActiveSkills();
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public virtual void UpdateCooldown()
    {
        if (pItemCDEffectUI == null) return;
        if (effectActivated && cooldown > 0 && pItemCDEffectUI.activeSelf) //Reset CD
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
        if (currentCD <= 0 && !effectActivated && checkCondition)
        {
            if (conditionTag.Equals(PItemEffectManager.ConditionTag.EndOfCooldown))
            {
                CheckCondition();
            }
        }
    }
}
