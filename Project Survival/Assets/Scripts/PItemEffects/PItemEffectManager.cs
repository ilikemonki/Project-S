using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PItemEffectManager : MonoBehaviour
{
    public enum ConditionTag
    {
        DamageTaken, Heal
    }
    public static PItemEffectManager instance;
    public List<PassiveItemEffect> pItemEffectList;

    public void Awake()
    {
        instance = this;
    }
    public void Update()
    {
        if (instance.pItemEffectList.Count > 0)
        {
            for (int i = 0; i < instance.pItemEffectList.Count; i++)
            {
                instance.pItemEffectList[i].UpdateCooldown();
            }
        }
    }
    public static void CheckAllPItemCondition(float valueToCheck, ConditionTag condTag) //Go through list and check conditionTag with the other
    {
        for(int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            if (instance.pItemEffectList[i].conditionTag.Equals(condTag))
            {
                instance.pItemEffectList[i].CheckCondition(valueToCheck);
            }
        }
    }
    public static void AddToList(PassiveItemEffect p)
    {
        instance.pItemEffectList.Add(p);
    }
}
