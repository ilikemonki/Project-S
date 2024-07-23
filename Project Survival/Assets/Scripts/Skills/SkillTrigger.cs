using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        critTrigger, damageTakenTrigger, healTrigger, dashTrigger, usageTrigger,
        bleedTrigger, burnTrigger, chillTrigger, shockTrigger,
        degenTrigger, contactTrigger
    }
    public SkillController skillController;
    [Header("Trigger Stats")]
    public float counterGoal;
    public float currentCounter;  //variable can be reused for multiple triggers that needs a counter.
    [Header("Trigger Skills")]
    public bool isTriggerSkill;
    public TriggerType triggerType;

    public bool CheckTriggerCondition() //Used for DamageTaken, Heal
    {
        if (currentCounter >= counterGoal)
        {
            currentCounter = 0;
            return true;
        }
        else return false;
    }
}
