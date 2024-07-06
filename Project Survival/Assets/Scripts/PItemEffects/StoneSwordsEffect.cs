using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSwordsEffect : PassiveItemEffect
{
    //Each active melee skill will increase base defense by 2.
    public float baseDefenseAdded;
    public override void CheckCondition()
    {
        if (checkCondition)
        {
             ActivateEffect();
        }
    }
    public override void ActivateEffect()
    {
        if (effectActivated) gameplayManager.player.baseDefense -= baseDefenseAdded;
        baseDefenseAdded = 0;
        for (int i = 0; i < gameplayManager.inventory.activeSkillList.Count; i++)
        {
            if (gameplayManager.inventory.activeSkillList[i].skillController != null)
            {
                if (gameplayManager.inventory.activeSkillList[i].skillController.isMelee)
                {
                    baseDefenseAdded += 2;
                }
            }
        }
        effectActivated = true;
        gameplayManager.player.baseDefense += baseDefenseAdded;
        gameplayManager.player.UpdatePlayerStats();
        UpdateStats.UpdateAllActiveSkills();
        UpdateStats.FormatPlayerStatsToString();
        UpdateStats.FormatEnemyStatsToString();
    }
    public override void RemoveEffect()
    {
        if (effectActivated) gameplayManager.player.baseDefense -= baseDefenseAdded;
        gameplayManager.player.UpdatePlayerStats();
        effectActivated = false;
        checkCondition = false;
        base.RemoveEffect();
    }
    public override void WhenAcquired()
    {
        checkCondition = true;
        ActivateEffect();
    }
}
