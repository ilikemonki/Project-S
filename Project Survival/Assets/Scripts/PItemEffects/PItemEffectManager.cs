using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PItemEffectManager : MonoBehaviour
{
    public enum ConditionTag
    {
        DamageTaken, Heal, WhenAcquired, Degen, CoinCollect, ActiveSkill, EndOfCooldown
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
    public static void CheckAllPItemCondition(float valueToCheck, ConditionTag condTag, bool checkValue) //Go through list and check conditionTag with the other
    {
        for(int i = 0; i < instance.pItemEffectList.Count; i++)
        {
            if (instance.pItemEffectList[i].conditionTag.Equals(condTag) && instance.pItemEffectList[i].checkCondition)
            {
                if (checkValue)
                    instance.pItemEffectList[i].CheckCondition(valueToCheck);
                else
                    instance.pItemEffectList[i].CheckCondition();
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
}
