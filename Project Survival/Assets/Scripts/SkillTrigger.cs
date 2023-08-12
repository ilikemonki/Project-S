using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    [Header("Trigger Stats")]
    public float counterGoal;
    public float currentCounter;  //variable can be reused for multiple triggers that needs a counter.
    [Header("Trigger Skills")]
    public bool useCritTrigger;
    public bool useDamageTakenTrigger;
    public bool useHealTrigger;
    public bool useDashTrigger;
    public bool useUsageTrigger;
    public bool useAilmentTrigger;
    public bool useBloodTrigger; //Give degen to player, amount of degen accumulated is the condition

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
