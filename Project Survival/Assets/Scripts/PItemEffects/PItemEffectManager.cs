using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PItemEffectManager : MonoBehaviour
{
    public enum ConditionTag
    {
        DamageTaken, Heal, WhenAcquired, Degen, CoinCollect, ActiveSkill, EndOfCooldown, EnemyKilled, UpdatingStats,
    }
    public static PItemEffectManager instance;
    public List<PassiveItemEffect> pItemEffectList;
    public GameObject pItemCDEffectUIPrefab; //Prefab to instantiate
    public GameObject pItemCDEffectParent; //Where to instantiate

    public void Awake()
    {
        instance = this;
    }
    public void Update()
    {
        if (instance.pItemEffectList.Count > 0 && Time.timeScale > 0)
        {
            for (int i = 0; i < instance.pItemEffectList.Count; i++) //update cooldowns for all effects.
            {
                if (pItemEffectList[i].cooldown > 0)
                    instance.pItemEffectList[i].UpdateCooldown();
            }
        }
    }
    public static void CheckAllPItemCondition(float valueToCheck, ConditionTag condTag) //Go through list and check conditionTag with the other
    {
        for(int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            if (instance.pItemEffectList[i].conditionTag.Equals(condTag) && instance.pItemEffectList[i].checkCondition)
            {
                instance.pItemEffectList[i].CheckCondition(valueToCheck);
            }
        }
    }
    public static void CheckAllPItemCondition(ConditionTag condTag) //Go through list and check conditionTag with the other
    {
        for (int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            if (instance.pItemEffectList[i].conditionTag.Equals(condTag) && instance.pItemEffectList[i].checkCondition)
            {
                instance.pItemEffectList[i].CheckCondition();
            }
        }
    }
    public static void CheckAllPItemCondition(GameObject obj, ConditionTag condTag) //Go through list and check conditionTag with the other
    {
        for (int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            if (instance.pItemEffectList[i].conditionTag.Equals(condTag) && instance.pItemEffectList[i].checkCondition)
            {
                instance.pItemEffectList[i].CheckCondition(obj);
            }
        }
    }
    public static void AcquireItem(PassiveItemEffect p)
    {
        p.WhenAcquired();
        if (!instance.pItemEffectList.Contains(p))
        {
            instance.pItemEffectList.Add(p);
            if (p.cooldown > 0) //Add pItem to CDEffectUI if it has cooldown.
            {
                GameObject objUI = Instantiate(instance.pItemCDEffectUIPrefab, instance.pItemCDEffectParent.transform);
                p.pItemCDEffectUI = objUI;
                if (objUI.TryGetComponent<Image>(out Image img))
                {
                    img.sprite = p.itemDesc.itemSprite;
                    p.cdText = objUI.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }
    }
    public static void UpdateAllPItemStats()
    {
        for (int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            instance.pItemEffectList[i].UpdateItemStats();
        }
    }
}
