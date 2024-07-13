using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mono.Cecil.Cil;
using static Cinemachine.DocumentationSortingAttribute;

public class UpdateStats : MonoBehaviour
{
    public enum Modifier
    {
        //additives
        Melee_Amount, Projectile_Amount, Pierce, Chain, Combo,
        Base_Physical_Damage, Base_Fire_Damage, Base_Cold_Damage, Base_Lightning_Damage,
        Critical_Chance, Projectile_Critical_Chance, Melee_Critical_Chance, Critical_Damage, Projectile_Critical_Damage, Melee_Critical_Damage,
        Regen, Degen, Life_Steal_Chance, Projectile_Life_Steal_Chance, Melee_Life_Steal_Chance, Life_Steal, Projectile_Life_Steal, Melee_Life_Steal,
        Bleed_Chance, Burn_Chance, Chill_Chance, Shock_Chance,
        Bleed_Effect, Burn_Effect, Chill_Effect, Shock_Effect,
        knockback,
        //multipliers
        Damage, Projectile_Damage, Melee_Damage, Physical_Damage, Fire_Damage, Cold_Damage, Lightning_Damage,
        Attack_Range, Projectile_Attack_Range, Melee_Attack_Range,
        Travel_Range, Projectile_Travel_Range, Melee_Travel_Range,
        Travel_Speed, Projectile_Travel_Speed, Melee_Travel_Speed,
        Cooldown, Projectile_Cooldown, Melee_Cooldown,
        Movement_Speed,
        Max_Health,
        Defense,
        Magnet_Range,
        AoE, Projectile_AoE, Melee_AoE,
        //behaviors
        Backward_Split,
        Return_,
        Pierce_All,
}
    public enum EnemyModifier
    {
        Move_Speed, Max_Health, Damage, Attack_Cooldown, Projectile, Projectile_Travel_Speed,
        Physical_Reduction, Fire_Reduction, Cold_Reduction, Lightning_Reduction
    }
    public static UpdateStats instance;
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    public EnemyManager enemyManager;
    public PlayerStats player;
    public ItemManager itemManager;
    float currentMaxHP;
    public void Awake()
    {
        instance = this;
    }
    public static void ApplyGlobalUpgrades(Upgrades upgrade, bool unapplyUpgrades)
    {
        if (upgrade.levelModifiersList.Count <= 0) return;
        float x;
        int lv;
        if (upgrade.itemDescription != null)
            lv = upgrade.itemDescription.currentLevel;
        else
            lv = 0;
        instance.currentMaxHP = instance.player.maxHealth;
        for (int i = 0; i < upgrade.levelModifiersList[lv].modifier.Count; i++)
        {
            if (unapplyUpgrades)
                x = -upgrade.levelModifiersList[lv].amt[i];
            else
                x = upgrade.levelModifiersList[lv].amt[i];
            switch (upgrade.levelModifiersList[lv].modifier[i])
            {
                case Modifier.Attack_Range: instance.gameplayManager.attackRangeMultiplier += x; break;
                case Modifier.Base_Cold_Damage: instance.gameplayManager.baseDamageTypeAdditive[2] += x; break;
                case Modifier.Base_Fire_Damage: instance.gameplayManager.baseDamageTypeAdditive[1] += x; break;
                case Modifier.Base_Lightning_Damage: instance.gameplayManager.baseDamageTypeAdditive[3] += x; break;
                case Modifier.Base_Physical_Damage: instance.gameplayManager.baseDamageTypeAdditive[0] += x; break;
                case Modifier.Bleed_Chance: instance.gameplayManager.ailmentsChanceAdditive[0] += x; break;
                case Modifier.Bleed_Effect: instance.gameplayManager.ailmentsEffectMultiplier[0] += x; break;
                case Modifier.Burn_Chance: instance.gameplayManager.ailmentsChanceAdditive[1] += x; break;
                case Modifier.Burn_Effect: instance.gameplayManager.ailmentsEffectMultiplier[1] += x; break;
                case Modifier.Chain: instance.gameplayManager.chainAdditive += (int)x; break;
                case Modifier.Chill_Chance: instance.gameplayManager.ailmentsChanceAdditive[2] += x; break;
                case Modifier.Chill_Effect: instance.gameplayManager.ailmentsEffectMultiplier[2] += x; break;
                case Modifier.Cold_Damage: instance.gameplayManager.damageTypeMultiplier[2] += x; break;
                case Modifier.Combo: instance.gameplayManager.comboAdditive += (int)x; break;
                case Modifier.Cooldown: instance.gameplayManager.cooldownMultiplier += x; break;
                case Modifier.Critical_Chance: instance.gameplayManager.criticalChanceAdditive += x; break;
                case Modifier.Critical_Damage: instance.gameplayManager.criticalDamageAdditive += x; break;
                case Modifier.Damage: instance.gameplayManager.damageMultiplier += x; break;
                case Modifier.Defense: instance.gameplayManager.defenseMultiplier += x; break;
                case Modifier.Degen: instance.gameplayManager.degenAdditive += x; break;
                case Modifier.Fire_Damage: instance.gameplayManager.damageTypeMultiplier[1] += x; break;
                case Modifier.Life_Steal: instance.gameplayManager.lifeStealAdditive += x; break;
                case Modifier.Life_Steal_Chance: instance.gameplayManager.lifeStealChanceAdditive += x; break;
                case Modifier.Lightning_Damage: instance.gameplayManager.damageTypeMultiplier[3] += x; break;
                case Modifier.Magnet_Range: instance.gameplayManager.magnetRangeMultiplier += x; break;
                case Modifier.Max_Health: instance.gameplayManager.maxHealthMultiplier += x; break;
                case Modifier.Melee_Attack_Range: instance.gameplayManager.meleeAttackRangeMultiplier += x; break;
                case Modifier.Melee_Cooldown: instance.gameplayManager.meleeCooldownMultiplier += x; break;
                case Modifier.Melee_Critical_Chance: instance.gameplayManager.meleeCriticalChanceAdditive += x; break;
                case Modifier.Melee_Critical_Damage: instance.gameplayManager.meleeCriticalDamageAdditive += x; break;
                case Modifier.Melee_Damage: instance.gameplayManager.meleeDamageMultiplier += x; break;
                case Modifier.Melee_Life_Steal: instance.gameplayManager.meleeLifeStealAdditive += x; break;
                case Modifier.Melee_Life_Steal_Chance: instance.gameplayManager.meleeLifeStealChanceAdditive += x; break;
                case Modifier.Melee_AoE: instance.gameplayManager.meleeAoeMultiplier += x; break;
                case Modifier.Melee_Travel_Range: instance.gameplayManager.meleeTravelRangeMultiplier += x; break;
                case Modifier.Melee_Travel_Speed: instance.gameplayManager.meleeTravelSpeedMultiplier += x; break;
                case Modifier.Movement_Speed: instance.gameplayManager.moveSpeedMultiplier += x; break;
                case Modifier.Physical_Damage: instance.gameplayManager.damageTypeMultiplier[0] += x; break;
                case Modifier.Pierce: instance.gameplayManager.pierceAdditive += (int)x; break;
                case Modifier.Projectile_Amount: instance.gameplayManager.projectileAmountAdditive += (int)x; break;
                case Modifier.Projectile_Attack_Range: instance.gameplayManager.projectileAttackRangeMultiplier += x; break;
                case Modifier.Projectile_Cooldown: instance.gameplayManager.projectileCooldownMultiplier += x; break;
                case Modifier.Projectile_Critical_Chance: instance.gameplayManager.projectileCriticalChanceAdditive += x; break;
                case Modifier.Projectile_Critical_Damage: instance.gameplayManager.projectileCriticalDamageAdditive += x; break;
                case Modifier.Projectile_Damage: instance.gameplayManager.projectileDamageMultiplier += x; break;
                case Modifier.Projectile_Life_Steal: instance.gameplayManager.projectileLifeStealAdditive += x; break;
                case Modifier.Projectile_Life_Steal_Chance: instance.gameplayManager.projectileLifeStealChanceAdditive += x; break;
                case Modifier.Projectile_AoE: instance.gameplayManager.projectileAoeMultiplier += x; break;
                case Modifier.Projectile_Travel_Range: instance.gameplayManager.projectileTravelRangeMultiplier += x; break;
                case Modifier.Projectile_Travel_Speed: instance.gameplayManager.projectileTravelSpeedMultiplier += x; break;
                case Modifier.Regen: instance.gameplayManager.regenAdditive += x; break;
                case Modifier.Shock_Chance: instance.gameplayManager.ailmentsChanceAdditive[3] += x; break;
                case Modifier.Shock_Effect: instance.gameplayManager.ailmentsEffectMultiplier[3] += x; break;
                case Modifier.AoE: instance.gameplayManager.aoeMultiplier += x; break;
                case Modifier.Melee_Amount: instance.gameplayManager.meleeAmountAdditive += (int)x; break;
                case Modifier.Travel_Range: instance.gameplayManager.travelRangeMultiplier += x; break;
                case Modifier.Travel_Speed: instance.gameplayManager.travelSpeedMultiplier += x; break;
                default: Debug.Log("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[lv].modifier[i]); break;
            }
        }
        instance.player.UpdatePlayerStats();
        UpdateAllActiveSkills();
        PItemEffectManager.UpdateAllPItemStats();
        if (instance.currentMaxHP != instance.player.maxHealth) //if hp is upgraded, update current hp.
        {
            instance.player.currentHealth += instance.player.maxHealth - instance.currentMaxHP;
            instance.player.UpdateHealthBar();
        }
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    //Upgrade skill when it levels.
    public static void ApplySkillUpgrades(Upgrades upgrade, SkillController skill, int level)
    {
        if (upgrade.levelModifiersList.Count < level) 
        {
            Debug.Log("Skill Level UP doesn't exist at level " + level);
            return;
        }
        for (int i = 0; i < upgrade.levelModifiersList[level].modifier.Count; i++)
        {
            switch (upgrade.levelModifiersList[level].modifier[i])
            {
                case Modifier.Attack_Range: skill.addedAttackRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Base_Cold_Damage: skill.addedBaseDamageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Base_Fire_Damage: skill.addedBaseDamageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Base_Lightning_Damage: skill.addedBaseDamageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Base_Physical_Damage: skill.addedBaseDamageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Bleed_Chance: skill.addedAilmentsChance[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Bleed_Effect: skill.addedAilmentsEffect[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Burn_Chance: skill.addedAilmentsChance[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Burn_Effect: skill.addedAilmentsEffect[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Chain: skill.addedChain += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Chill_Chance: skill.addedAilmentsChance[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Chill_Effect: skill.addedAilmentsEffect[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Cold_Damage: skill.addedDamageTypes[2] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Combo: skill.addedCombo += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Cooldown: skill.addedCooldown += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Critical_Chance: skill.addedCriticalChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Critical_Damage: skill.addedCriticalDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Damage: skill.addedDamage += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Fire_Damage: skill.addedDamageTypes[1] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.knockback: skill.addedKnockBack += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Life_Steal: skill.addedLifeSteal += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Life_Steal_Chance: skill.addedLifeStealChance += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Lightning_Damage: skill.addedDamageTypes[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Physical_Damage: skill.addedDamageTypes[0] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Pierce: skill.addedPierce += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Projectile_Amount: skill.addedProjectileAmount += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Shock_Chance: skill.addedAilmentsChance[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Shock_Effect: skill.addedAilmentsEffect[3] += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.AoE: skill.addedAoe += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Melee_Amount: skill.addedMeleeAmount += (int)upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Travel_Range: skill.addedTravelRange += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Travel_Speed: skill.addedTravelSpeed += upgrade.levelModifiersList[level].amt[i]; break;
                case Modifier.Backward_Split: skill.useBackwardSplit = true; break;
                case Modifier.Return_: skill.useReturn = true; break;
                case Modifier.Pierce_All: skill.pierceAll = true; break;
                default: Debug.Log("ApplySkillMod has no switch case for " + upgrade.levelModifiersList[level].modifier[i]); break;
            }
        }
        skill.UpdateSkillStats();
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    //Apply gem modifiers to skill.
    public static void ApplyGemUpgrades(Upgrades upgrade, SkillController skill, Image frameImage, bool unapplyUpgrades)
    {
        //Check if gem can apply to the skill orb. If gem doesn't have projectile or melee tag, it can be applied to all gems.
        if ((upgrade.itemDescription.itemTags.Contains("Projectile") && skill.skillOrbDescription.itemTags.Contains("Projectile")) ||
            (upgrade.itemDescription.itemTags.Contains("Melee") && skill.skillOrbDescription.itemTags.Contains("Melee")) ||
            (!upgrade.itemDescription.itemTags.Contains("Projectile") && !upgrade.itemDescription.itemTags.Contains("Melee")))
        {
            float x;
            for (int i = 0; i < upgrade.levelModifiersList[0].modifier.Count; i++)
            {
                if (unapplyUpgrades)
                    x = -upgrade.levelModifiersList[0].amt[i];
                else
                    x = upgrade.levelModifiersList[0].amt[i];
                switch (upgrade.levelModifiersList[0].modifier[i])
                {
                    case Modifier.Attack_Range: skill.addedAttackRange += x; break;
                    case Modifier.Melee_Attack_Range: skill.addedAttackRange += x; break;
                    case Modifier.Projectile_Attack_Range: skill.addedAttackRange += x; break;
                    case Modifier.Base_Cold_Damage: skill.addedBaseDamageTypes[2] += x; break;
                    case Modifier.Base_Fire_Damage: skill.addedBaseDamageTypes[1] += x; break;
                    case Modifier.Base_Lightning_Damage: skill.addedBaseDamageTypes[3] += x; break;
                    case Modifier.Base_Physical_Damage: skill.addedBaseDamageTypes[0] += x; break;
                    case Modifier.Bleed_Chance: skill.addedAilmentsChance[0] += x; break;
                    case Modifier.Bleed_Effect: skill.addedAilmentsEffect[0] += x; break;
                    case Modifier.Burn_Chance: skill.addedAilmentsChance[1] += x; break;
                    case Modifier.Burn_Effect: skill.addedAilmentsEffect[1] += x; break;
                    case Modifier.Chain: skill.addedChain += (int)x; break;
                    case Modifier.Chill_Chance: skill.addedAilmentsChance[2] += x; break;
                    case Modifier.Chill_Effect: skill.addedAilmentsEffect[2] += x; break;
                    case Modifier.Cold_Damage: skill.addedDamageTypes[2] += x; break;
                    case Modifier.Combo: skill.addedCombo += (int)x; break;
                    case Modifier.Cooldown: skill.addedCooldown += x; break;
                    case Modifier.Melee_Cooldown: skill.addedCooldown += x; break;
                    case Modifier.Projectile_Cooldown: skill.addedCooldown += x; break;
                    case Modifier.Critical_Chance: skill.addedCriticalChance += x; break;
                    case Modifier.Critical_Damage: skill.addedCriticalDamage += x; break;
                    case Modifier.Melee_Critical_Chance: skill.addedCriticalChance += x; break;
                    case Modifier.Melee_Critical_Damage: skill.addedCriticalDamage += x; break;
                    case Modifier.Projectile_Critical_Chance: skill.addedCriticalChance += x; break;
                    case Modifier.Projectile_Critical_Damage: skill.addedCriticalDamage += x; break;
                    case Modifier.Damage: skill.addedDamage += x; break;
                    case Modifier.Melee_Damage: skill.addedDamage += x; break;
                    case Modifier.Projectile_Damage: skill.addedDamage += x; break;
                    case Modifier.Fire_Damage: skill.addedDamageTypes[1] += x; break;
                    case Modifier.knockback: skill.addedKnockBack += x; break;
                    case Modifier.Life_Steal: skill.addedLifeSteal += x; break;
                    case Modifier.Life_Steal_Chance: skill.addedLifeStealChance += x; break;
                    case Modifier.Melee_Life_Steal: skill.addedLifeSteal += x; break;
                    case Modifier.Melee_Life_Steal_Chance: skill.addedLifeStealChance += x; break;
                    case Modifier.Projectile_Life_Steal: skill.addedLifeSteal += x; break;
                    case Modifier.Projectile_Life_Steal_Chance: skill.addedLifeStealChance += x; break;
                    case Modifier.Lightning_Damage: skill.addedDamageTypes[3] += x; break;
                    case Modifier.Physical_Damage: skill.addedDamageTypes[0] += x; break;
                    case Modifier.Pierce: skill.addedPierce += (int)x; break;
                    case Modifier.Projectile_Amount: skill.addedProjectileAmount += (int)x; break;
                    case Modifier.Shock_Chance: skill.addedAilmentsChance[3] += x; break;
                    case Modifier.Shock_Effect: skill.addedAilmentsEffect[3] += x; break;
                    case Modifier.AoE: skill.addedAoe += x; break;
                    case Modifier.Melee_AoE: skill.addedAoe += x; break;
                    case Modifier.Projectile_AoE: skill.addedAoe += x; break;
                    case Modifier.Melee_Amount: skill.addedMeleeAmount += (int)x; break;
                    case Modifier.Travel_Range: skill.addedTravelRange += x; break;
                    case Modifier.Travel_Speed: skill.addedTravelSpeed += x; break;
                    case Modifier.Melee_Travel_Range: skill.addedTravelRange += x; break;
                    case Modifier.Melee_Travel_Speed: skill.addedTravelSpeed += x; break;
                    case Modifier.Projectile_Travel_Range: skill.addedTravelRange += x; break;
                    case Modifier.Projectile_Travel_Speed: skill.addedTravelSpeed += x; break;
                    case Modifier.Backward_Split: skill.useBackwardSplit = true; break;
                    case Modifier.Return_: skill.useReturn = true; break;
                    case Modifier.Pierce_All: skill.pierceAll = true; break;
                    default: Debug.Log("ApplyGemMod has no switch case for " + upgrade.levelModifiersList[0].modifier[i]); break;
                }
                if (unapplyUpgrades) Debug.Log("Unapplying Gem : " + upgrade.name);
                else Debug.Log("Applying Gem : " + upgrade.name);
                frameImage.color = Color.white;
            }
            skill.UpdateSkillStats();
            FormatPlayerStatsToString();
            FormatEnemyStatsToString();
        }
        else //Do not apply gem upgrades. Change item frame color to red.
        {
            if (frameImage != null) frameImage.color = Color.red;
            if (unapplyUpgrades) 
            {
                Debug.Log("white");
                frameImage.color = Color.white; 
            }
            Debug.Log(upgrade.name + "tags do not match. Not applied to orb.");
        }
    }
    public static void ApplyEnemyUpgrades(EnemyUpgradeManager.EnemyUpgrades upgrade)
    {
        for (int i = 0; i < upgrade.modifier.Count; i++)
        {
            switch (upgrade.modifier[i])
            {
                case EnemyModifier.Attack_Cooldown: instance.gameplayManager.enemyAttackCooldownMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.Cold_Reduction: instance.gameplayManager.enemyReductions[2] += upgrade.amt[i]; break;
                case EnemyModifier.Damage: instance.gameplayManager.enemyDamageMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.Fire_Reduction: instance.gameplayManager.enemyReductions[1] += upgrade.amt[i]; break;
                case EnemyModifier.Lightning_Reduction: instance.gameplayManager.enemyReductions[3] += upgrade.amt[i]; break;
                case EnemyModifier.Max_Health: instance.gameplayManager.enemyMaxHealthMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.Move_Speed: instance.gameplayManager.enemyMoveSpeedMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.Physical_Reduction: instance.gameplayManager.enemyReductions[0] += upgrade.amt[i]; break;
                case EnemyModifier.Projectile: instance.gameplayManager.enemyProjectileAdditive += upgrade.amt[i]; break;
                //case EnemyModifier.Projectile_size: instance.gameplayManager.enemyProjectileSizeMultiplier += upgrade.amt[i]; break;
                //case EnemyModifier.Projectile_Travel_Range: instance.gameplayManager.enemyProjectileTravelRangeMultiplier += upgrade.amt[i]; break;
                case EnemyModifier.Projectile_Travel_Speed: instance.gameplayManager.enemyProjectileTravelSpeedMultiplier += upgrade.amt[i]; break;
                default: Debug.Log("ApplyEnemyUpgrades has no switch case for " + upgrade.modifier[i]); break;
            }
        }
        instance.enemyManager.UpdateAllEnemyStats();
        FormatPlayerStatsToString();
        FormatEnemyStatsToString();
    }
    public static string FormatSkillUpgradesToString(Upgrades upgrade) //Format skill level up stats to string for Tooltip.
    {
        string skillUpgradesString = "";
        for (int k = 0; k < upgrade.levelModifiersList.Count; k++) //number of level ups (4)
        {
            skillUpgradesString += "Lv. " + (k + 2) + "\n";
            for (int i = 0; i < upgrade.levelModifiersList[k].modifier.Count; i++) //number of upgrades
            {
                switch (upgrade.levelModifiersList[k].modifier[i])
                {
                    case Modifier.Attack_Range: skillUpgradesString += "\tAttack Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Base_Cold_Damage: skillUpgradesString += "\tBase Cold Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Base_Fire_Damage: skillUpgradesString += "\tBase Fire Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Base_Lightning_Damage: skillUpgradesString += "\tBase Lightning Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Base_Physical_Damage: skillUpgradesString += "\tBase Physical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Bleed_Chance: skillUpgradesString += "\tBleed Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Bleed_Effect: skillUpgradesString += "\tBleed Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Burn_Chance: skillUpgradesString += "\tBurn Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Burn_Effect: skillUpgradesString += "\tBurn Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Chain: skillUpgradesString += "\tChain: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Chill_Chance: skillUpgradesString += "\tChill Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Chill_Effect: skillUpgradesString += "\tChill Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Cold_Damage: skillUpgradesString += "\tCold Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Combo: skillUpgradesString += "\tCombo: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Cooldown: skillUpgradesString += "\tCooldown: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Critical_Chance: skillUpgradesString += "\tCritical Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Critical_Damage: skillUpgradesString += "\tCritical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Damage: skillUpgradesString += "\tDamage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Fire_Damage: skillUpgradesString += "\tFire Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.knockback: skillUpgradesString += "\tKnockback: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Life_Steal: skillUpgradesString += "\tLife Steal: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Life_Steal_Chance: skillUpgradesString += "\tLife Steal Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Lightning_Damage: skillUpgradesString += "\tLightning Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Physical_Damage: skillUpgradesString += "\tPhysical Damage: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Pierce: skillUpgradesString += "\tPierce: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Projectile_Amount: skillUpgradesString += "\tProjectile Amount: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Shock_Chance: skillUpgradesString += "\tShock Chance: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Shock_Effect: skillUpgradesString += "\tShock Effect: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.AoE: skillUpgradesString += "\tSize: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Melee_Amount: skillUpgradesString += "\tMelee Amount: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "</color>\n"; break;
                    case Modifier.Travel_Range: skillUpgradesString += "\tTravel Range: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    case Modifier.Travel_Speed: skillUpgradesString += "\tTravel Speed: <color=green>+" + upgrade.levelModifiersList[k].amt[i] + "%</color>\n"; break;
                    default: Debug.Log("SkillUpgradesToString has no switch case for " + upgrade.levelModifiersList[k].modifier[i]); break;
                }
            }
        }
        return skillUpgradesString;
    }
    public static string FormatItemUpgradeStatsToString(Upgrades.LevelModifiers stats)
    {
        string statNameString = "";
        string fullString = "";
        for (int i = 0; i < stats.modifier.Count; i++)
        {
            statNameString = stats.modifier[i].ToString();
            if (statNameString.Contains("_"))
            {
                statNameString = statNameString.Replace('_', ' ');
            }
            //statNameString = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(statNameString.ToLower()); //Capitalizes each word
            if (stats.amt[i] > 0) //if positive, green w/ + sign.
            {
                if (fullString.Equals("")) //For first modifier only
                {
                    if (instance.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = statNameString + ": " + "<color=green>+" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = statNameString + ": " + "<color=green>+" + stats.amt[i] + "%</color>";
                    }
                }
                else
                {
                    if (instance.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=green>+" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=green>+" + stats.amt[i] + "%</color>";
                    }
                }
            }
            else //red for negative
            {
                if (fullString.Equals("")) //For first modifier only
                {
                    if (instance.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = statNameString + ": " + "<color=red>" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = statNameString + ": " + "<color=red>" + stats.amt[i] + "%</color>";
                    }
                }
                else
                {
                    if (instance.CheckPercentModifier(stats.modifier[i]))
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=red>" + stats.amt[i] + "</color>";
                    }
                    else
                    {
                        fullString = fullString + "\n" +
                            statNameString + ": " + "<color=red>" + stats.amt[i] + "%</color>";
                    }
                }
            }
        }
        return fullString;
    }
    public static string FormatSkillStatsToString(SkillController sc) //Return stats of the skill controller as string.
    {
        string fullString = "";
        if (sc.level == 5)
            fullString += "Level: " + sc.level + " (max)\n";
        else
            fullString += "Level: " + sc.level + "\n";
        fullString += "Exp: " + sc.exp + "/" + instance.gameplayManager.skillExpCapList[sc.level - 1] + "\n";
        for (int i = 0; i < sc.damageTypes.Count; i++)
        {
            if (sc.damageTypes[i] > 0)
            {
                if (i == 0) //physical damage
                {
                    if ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) != 0)
                        fullString += "<color=grey>Physical Damage</color>: " + (sc.damageTypes[i]) + " (" + ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) > 0 ? "+" + (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) : (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i])) + "%)\n";
                    else
                        fullString += "<color=grey>Physical Damage</color>: " + (sc.damageTypes[i]) + "\n";
                }
                else if (i == 1) //Fire damage
                {
                    if ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) != 0)
                        fullString += "<color=red>Fire Damage</color>: " + (sc.damageTypes[i]) + " (" + ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) > 0 ? "+" + (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) : (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i])) + "%)\n";
                    else
                        fullString += "<color=red>Fire Damage</color>: " + (sc.damageTypes[i]) + "\n";
                }
                else if (i == 2) //cold damage
                {
                    if ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) != 0)
                        fullString += "<color=blue>Cold Damage</color>: " + (sc.damageTypes[i]) + " (" + ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) > 0 ? "+" + (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) : (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i])) + "%)\n";
                    else
                        fullString += "<color=blue>Cold Damage</color>: " + (sc.damageTypes[i]) + "\n";
                }
                else if (i == 3) //lightning damage
                {
                    if ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) != 0)
                        fullString += "<color=yellow>Lightning Damage</color>: " + (sc.damageTypes[i]) + " (" + ((instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) > 0 ? "+" + (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i]) : (instance.gameplayManager.damageTypeMultiplier[i] + sc.damage + sc.addedDamageTypes[i])) + "%)\n";
                    else
                        fullString += "<color=yellow>Lightning Damage</color>: " + (sc.damageTypes[i]) + "\n";
                }
            }
        }
        for (int i = 0; i < sc.ailmentsChance.Count; i++)
        {
            if (sc.ailmentsChance[i] > 0) 
            {
                if (i == 0) //physical damage
                {
                    fullString += "Bleed Chance: " + sc.ailmentsChance[i] + "%\n";
                    if ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) != 0)
                        fullString += "Bleed Effect: " + (sc.ailmentsEffect[i]) + " (" + ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) > 0 ? "+" + (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) : (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i])) + "%)\n";
                    else
                        fullString += "Bleed Effect: " + (sc.ailmentsEffect[i]) + "%\n";
                    if (sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) >= 1)
                        fullString += "Bleed Damage: " + sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) + "\n";
                    else
                        fullString += "Bleed Damage: " + 1 + "\n";
                }
                else if (i == 1) //Fire damage
                {
                    fullString += "Burn Chance: " + sc.ailmentsChance[i] + "%\n";
                    if ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) != 0)
                        fullString += "Burn Effect: " + (sc.ailmentsEffect[i]) + " (" + ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) > 0 ? "+" + (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) : (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i])) + "%)\n";
                    else
                        fullString += "Burn Effect: " + (sc.ailmentsEffect[i]) + "%\n";
                    if (sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) >= 1)
                        fullString += "Burn Damage: " + sc.damageTypes[i] * (sc.ailmentsEffect[i] / 100) + "\n";
                    else
                        fullString += "Burn Damage: " + 1 + "\n"; ;
                }
                else if (i == 2) //cold damage
                {
                    fullString += "Chill Chance: " + sc.ailmentsChance[i] + "%\n";
                    if ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) != 0)
                        fullString += "Chill Effect: " + (sc.ailmentsEffect[i]) + " (" + ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) > 0 ? "+" + (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) : (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i])) + "%)\n";
                    else
                        fullString += "Chill Effect: " + (sc.ailmentsEffect[i]) + "%\n";
                }
                else if (i == 3) //lightning damage
                {
                    fullString += "Shock Chance: " + sc.ailmentsChance[i] + "%\n";
                    if ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) != 0)
                        fullString += "Shock Effect: " + (sc.ailmentsEffect[i]) + " (" + ((instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) > 0 ? "+" + (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i]) : (instance.gameplayManager.ailmentsEffectMultiplier[i] + sc.addedAilmentsEffect[i])) + "%)\n";
                    else
                        fullString += "Shock Effect: " + (sc.ailmentsEffect[i]) + "%\n";
                }
            }
        }
        if (sc.criticalChance > 0)
        {
            fullString += "Critical Chance: " + sc.criticalChance + "%\n";
            fullString += "Critical Damage: " + sc.criticalDamage + "%\n";
        }
        if (sc.cooldown > 0)
        {
            if (sc.isMelee)
            {
                if ((instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.meleeCooldownMultiplier + sc.addedCooldown) != 0)
                    fullString += "Cooldown: " + (sc.cooldown) + "s (" + ((instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.meleeCooldownMultiplier + sc.addedCooldown) > 0 ? "+" + (instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.meleeCooldownMultiplier + sc.addedCooldown) : (instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.meleeCooldownMultiplier + sc.addedCooldown)) + "%)\n";
                else
                    fullString += "Cooldown: " + (sc.cooldown) + "s\n";
            }
            else
            {
                if ((instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.projectileCooldownMultiplier + sc.addedCooldown) != 0)
                    fullString += "Cooldown: " + (sc.cooldown) + "s (" + ((instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.projectileCooldownMultiplier + sc.addedCooldown) > 0 ? "+" + (instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.projectileCooldownMultiplier + sc.addedCooldown) : (instance.gameplayManager.cooldownMultiplier + instance.gameplayManager.projectileCooldownMultiplier + sc.addedCooldown)) + "%)\n";
                else
                    fullString += "Cooldown: " + (sc.cooldown) + "s\n";
            }
        }
        if (sc.projectileAmount > 0)
            fullString += "Projectile Amount: " + sc.projectileAmount + "\n";
        if (sc.pierce > 0)
            fullString += "Pierce: " + sc.pierce + "\n";
        if (sc.chain > 0)
            fullString += "Chain: " + sc.chain + "\n";
        if (sc.meleeAmount > 0)
            fullString += "Melee Amount: " + sc.meleeAmount + "\n";
        if (sc.combo > 0)
            fullString += "Combo: " + sc.combo + "\n";
        if (sc.lifeStealChance > 0)
            fullString += "Lifesteal Chance: " + sc.lifeStealChance + "%\n";
        if (sc.lifeSteal > 0)
            fullString += "Lifesteal: " + sc.lifeSteal + "\n";
        if (sc.attackRange > 0)
        {
            if (sc.isMelee)
            {
                if ((instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.meleeAttackRangeMultiplier + sc.addedAttackRange) != 0)
                    fullString += "Attack Range: " + (sc.attackRange) + " (" + ((instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.meleeAttackRangeMultiplier + sc.addedAttackRange) > 0 ? "+" + (instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.meleeAttackRangeMultiplier + sc.addedAttackRange) : (instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.meleeAttackRangeMultiplier + sc.addedAttackRange)) + "%)\n";
                else
                    fullString += "Attack Range: " + (sc.attackRange) + "\n";
            }
            else
            {
                if ((instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.projectileAttackRangeMultiplier + sc.addedAttackRange) != 0)
                    fullString += "Attack Range: " + (sc.attackRange) + " (" + ((instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.projectileAttackRangeMultiplier + sc.addedAttackRange) > 0 ? "+" + (instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.projectileAttackRangeMultiplier + sc.addedAttackRange) : (instance.gameplayManager.attackRangeMultiplier + instance.gameplayManager.projectileAttackRangeMultiplier + sc.addedAttackRange)) + "%)\n";
                else
                    fullString += "Attack Range: " + (sc.attackRange) + "\n";
            }
        }
        if (sc.travelRange > 0)
        {
            if (sc.isMelee)
            {
                if ((instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.meleeTravelRangeMultiplier + sc.addedTravelRange) != 0)
                    fullString += "Travel Range: " + (sc.travelRange) + " (" + ((instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.meleeTravelRangeMultiplier + sc.addedTravelRange) > 0 ? "+" + (instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.meleeTravelRangeMultiplier + sc.addedTravelRange) : (instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.meleeTravelRangeMultiplier + sc.addedTravelRange)) + "%)\n";
                else
                    fullString += "Travel Range: " + (sc.travelRange) + "\n";
            }
            else
            {
                if ((instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.projectileTravelRangeMultiplier + sc.addedTravelRange) != 0)
                    fullString += "Travel Range: " + (sc.travelRange) + " (" + ((instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.projectileTravelRangeMultiplier + sc.addedTravelRange) > 0 ? "+" + (instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.projectileTravelRangeMultiplier + sc.addedTravelRange) : (instance.gameplayManager.travelRangeMultiplier + instance.gameplayManager.projectileTravelRangeMultiplier + sc.addedTravelRange)) + "%)\n";
                else
                    fullString += "Travel Range: " + (sc.travelRange) + "\n";
            }
        }
        if (sc.travelSpeed > 0)
        {
            if (sc.isMelee)
            {
                if ((instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.meleeTravelSpeedMultiplier + sc.addedTravelSpeed) != 0)
                    fullString += "Travel Speed: " + (sc.travelSpeed) + " (" + ((instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.meleeTravelSpeedMultiplier + sc.addedTravelSpeed) > 0 ? "+" + (instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.meleeTravelSpeedMultiplier + sc.addedTravelSpeed) : (instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.meleeTravelSpeedMultiplier + sc.addedTravelSpeed)) + "%)\n";
                else
                    fullString += "Travel Speed: " + (sc.travelSpeed) + "\n";
            }
            else
            {
                if ((instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.projectileTravelSpeedMultiplier + sc.addedTravelSpeed) != 0)
                    fullString += "Travel Speed: " + (sc.travelSpeed) + " (" + ((instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.projectileTravelSpeedMultiplier + sc.addedTravelSpeed) > 0 ? "+" + (instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.projectileTravelSpeedMultiplier + sc.addedTravelSpeed) : (instance.gameplayManager.travelSpeedMultiplier + instance.gameplayManager.projectileTravelSpeedMultiplier + sc.addedTravelSpeed)) + "%)\n";
                else
                    fullString += "Travel Speed: " + (sc.travelSpeed) + "\n";
            }
        }
        if (sc.knockBack > 0)
            fullString += "Knockback: " + sc.knockBack + "\n";
        if (sc.aoe > 0)
            fullString += "Size: +" + sc.aoe + "%\n";

        return fullString;
    }
    public static string FormatBaseSkillStatsToString(SkillController sc) //Return stats of the skill controller as string.
    {
        string fullString = "";
        if (sc.level == 5)
            fullString += "Level: " + sc.level + " (max)\n";
        else
            fullString += "Level: " + sc.level + "\n";
        fullString += "Exp: " + sc.exp + "/" + instance.gameplayManager.skillExpCapList[sc.level - 1] + "\n";
        for (int i = 0; i < sc.baseDamageTypes.Count; i++)
        {
            if ((sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) > 0)
            {
                if (i == 0) //physical damage
                {
                    fullString += "<color=grey>Base Physical Damage</color>: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) + "\n";
                }
                else if (i == 1) //Fire damage
                {
                    fullString += "<color=red>Base Fire Damage</color>: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) + "\n";
                }
                else if (i == 2) //cold damage
                {
                    fullString += "<color=blue>Base Cold Damage</color>: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) + "\n";
                }
                else if (i == 3) //lightning damage
                {
                    fullString += "<color=yellow>Base Lightning Damage</color>: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) + "\n";
                }
            }
        }
        for (int i = 0; i < sc.baseAilmentsChance.Count; i++)
        {
            if ((sc.baseAilmentsChance[i] + sc.addedAilmentsChance[i]) > 0)
            {
                if (i == 0) //physical damage
                {
                    fullString += "Bleed Chance: " + (sc.baseAilmentsChance[i] + sc.addedAilmentsChance[i]) + "%\n";
                    fullString += "Bleed Effect: +" + (sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) + "%\n";
                    if ((sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) * (1 + (sc.addedDamage + sc.addedDamageTypes[i]) / 100) * ((sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) / 100) >= 1)
                        fullString += "Bleed Damage: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) * (1 + (sc.addedDamage + sc.addedDamageTypes[i]) / 100) * ((sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) / 100) + "\n";
                    else
                        fullString += "Bleed Damage: " + 1 + "\n";
                }
                else if (i == 1) //Fire damage
                {
                    fullString += "Burn Chance: " + (sc.baseAilmentsChance[i] + sc.addedAilmentsChance[i]) + "%\n";
                    fullString += "Burn Effect: +" + (sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) + "%\n";
                    if ((sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) * (1 + (sc.addedDamage + sc.addedDamageTypes[i]) / 100) * ((sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) / 100) >= 1)
                        fullString += "Burn Damage: " + (sc.baseDamageTypes[i] + sc.addedBaseDamageTypes[i]) * (1 + (sc.addedDamage + sc.addedDamageTypes[i]) / 100) * ((sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) / 100) + "\n";
                    else
                        fullString += "Burn Damage: " + 1 + "\n"; ;
                }
                else if (i == 2) //cold damage
                {
                    fullString += "Chill Chance: " + (sc.baseAilmentsChance[i] + sc.addedAilmentsChance[i]) + "%\n";
                    fullString += "Chill Effect: +" + (sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) + "%\n";
                }
                else if (i == 3) //lightning damage
                {
                    fullString += "Shock Chance: " + (sc.baseAilmentsChance[i] + sc.addedAilmentsChance[i]) + "%\n";
                    fullString += "Shock Effect: +" + (sc.baseAilmentsEffect[i] + sc.addedAilmentsEffect[i]) + "%\n";
                }
            }
        }
        if ((sc.baseCriticalChance + sc.addedCriticalChance) > 0)
        {
            fullString += "Critical Chance: " + (sc.baseCriticalChance + sc.addedCriticalChance) + "%\n";
            fullString += "Critical Damage: +" + (sc.baseCriticalDamage + sc.addedCriticalDamage) + "%\n";
        }
        if ((sc.baseCooldown + sc.addedCooldown) > 0)
            fullString += "Cooldown: " + (sc.baseCooldown + sc.addedCooldown) + "s\n";
        if ((sc.baseProjectileAmount + sc.addedProjectileAmount) > 0)
            fullString += "Projectile Amount: " + (sc.baseProjectileAmount + sc.addedProjectileAmount) + "\n";
        if ((sc.basePierce + sc.addedPierce) > 0)
            fullString += "Pierce: " + (sc.basePierce + sc.addedPierce) + "\n";
        if ((sc.baseChain + sc.addedChain) > 0)
            fullString += "Chain: " + (sc.baseChain + sc.addedChain) + "\n";
        if ((sc.baseMeleeAmount + sc.addedMeleeAmount) > 0)
            fullString += "Melee Amount: " + (sc.baseMeleeAmount + sc.addedMeleeAmount) + "\n";
        if ((sc.baseCombo + sc.addedCombo) > 0)
            fullString += "Combo: " + (sc.baseCombo + sc.addedCombo) + "\n";
        if ((sc.baseLifeStealChance + sc.addedLifeStealChance) > 0)
            fullString += "Lifesteal Chance: " + (sc.baseLifeStealChance + sc.addedLifeStealChance) + "%\n";
        if ((sc.baseLifeSteal + sc.addedLifeSteal) > 0)
            fullString += "Lifesteal: " + (sc.baseLifeSteal + sc.addedLifeSteal) + "\n";
        if ((sc.baseAttackRange + sc.addedAttackRange) > 0)
            fullString += "Attack Range: " + (sc.baseAttackRange + sc.addedAttackRange) + "\n";
        if ((sc.baseTravelRange + sc.addedTravelRange) > 0)
            fullString += "Travel Range: " + (sc.baseTravelRange + sc.addedTravelRange) + "\n";
        if ((sc.baseTravelSpeed + sc.addedTravelSpeed) > 0)
            fullString += "Travel Speed: " + (sc.baseTravelSpeed + sc.addedTravelSpeed) + "\n";
        if ((sc.baseKnockBack + sc.addedKnockBack) > 0)
            fullString += "Knockback: " + (sc.baseKnockBack + sc.addedKnockBack) + "\n";
        if ((sc.addedAoe) > 0)
            fullString += "Size: +" + sc.addedAoe + "%\n";

        return fullString;
    }
    public static void FormatPlayerStatsToString()
    {
        instance.inventoryManager.playerStats1Text.text = string.Empty;
        string fullString = "";
        fullString += "Health: " + instance.player.currentHealth + "/" + instance.player.maxHealth + " (" + (instance.gameplayManager.maxHealthMultiplier >= 0 ? "+" + instance.gameplayManager.maxHealthMultiplier : instance.gameplayManager.maxHealthMultiplier) + "%)\n";
        fullString += "Level: " + instance.gameplayManager.level + "\n";
        if (instance.gameplayManager.defenseMultiplier != 0)
            fullString += "Defense: " + instance.player.defense + " (" + (instance.gameplayManager.defenseMultiplier > 0 ? "+" + instance.gameplayManager.defenseMultiplier : instance.gameplayManager.defenseMultiplier) + "%)\n";
        else
            fullString += "Defense: " + instance.player.defense + "\n";
        for (int i = 0; i < instance.gameplayManager.reductionsAdditive.Count; i++)
        {
            if (instance.gameplayManager.reductionsAdditive[i] != 0)
            {
                if (i == 0) //physical
                    fullString += "<color=grey>Physical Reduction</color>: " + (instance.gameplayManager.reductionsAdditive[i] > 0 ? "+" + instance.gameplayManager.reductionsAdditive[i] : instance.gameplayManager.reductionsAdditive[i]) + "\n";
                else if (i == 1) //Fire 
                    fullString += "<color=red>Fire Reduction</color>: " + (instance.gameplayManager.reductionsAdditive[i] > 0 ? "+" + instance.gameplayManager.reductionsAdditive[i] : instance.gameplayManager.reductionsAdditive[i]) + "\n";
                else if (i == 2) //cold
                    fullString += "<color=blue>Cold Reduction</color>: " + (instance.gameplayManager.reductionsAdditive[i] > 0 ? "+" + instance.gameplayManager.reductionsAdditive[i] : instance.gameplayManager.reductionsAdditive[i]) + "\n";
                else if (i == 3) //lightning
                    fullString += "<color=yellow>Lightning Reduction</color>: " + (instance.gameplayManager.reductionsAdditive[i] > 0 ? "+" + instance.gameplayManager.reductionsAdditive[i] : instance.gameplayManager.reductionsAdditive[i]) + "\n";
            }
        }
        if (instance.player.regen <= 0 && (instance.player.baseRegen + instance.gameplayManager.regenAdditive) != 0)
            fullString += "Regen: " + instance.player.regen + " (" + ((instance.player.baseRegen + instance.gameplayManager.regenAdditive) > 0 ? "+" + (instance.player.baseRegen + instance.gameplayManager.regenAdditive) : (instance.player.baseRegen + instance.gameplayManager.regenAdditive)) + ")\n";
        else 
            fullString += "Regen: " + instance.player.regen + "\n"; 
        if (instance.player.degen <= 0 && (instance.player.baseDegen + instance.gameplayManager.degenAdditive) != 0)
            fullString += "Degen: " + instance.player.degen + " (" + ((instance.player.baseDegen + instance.gameplayManager.degenAdditive) > 0 ? "+" + (instance.player.baseRegen + instance.gameplayManager.regenAdditive) : (instance.player.baseRegen + instance.gameplayManager.regenAdditive)) + ")\n";
        else
            fullString += "Degen: " + instance.player.degen + "\n";
        if (instance.gameplayManager.moveSpeedMultiplier != 0)
            fullString += "Movement Speed: " + instance.player.moveSpeed + " (" + (instance.gameplayManager.moveSpeedMultiplier > 0 ? "+" + instance.gameplayManager.moveSpeedMultiplier : instance.gameplayManager.moveSpeedMultiplier) + "%)\n";
        else
            fullString += "Movement Speed: " + instance.player.moveSpeed + "\n";
        if (instance.gameplayManager.dashChargesAdditive != 0)
            fullString += "Dash: " + instance.player.playerMovement.maxDashCharges + " (" + (instance.gameplayManager.dashChargesAdditive > 0 ? "+" + instance.gameplayManager.dashChargesAdditive : instance.gameplayManager.dashChargesAdditive) + ")\n";
        else
            fullString += "Dash: " + instance.player.playerMovement.maxDashCharges + "\n";
        if (instance.gameplayManager.dashCooldownMultiplier != 0)
            fullString += "Dash Cooldown: " + instance.player.playerMovement.dashCooldown + "s (" + (instance.gameplayManager.dashCooldownMultiplier > 0 ? "+" + instance.gameplayManager.dashCooldownMultiplier : instance.gameplayManager.dashCooldownMultiplier) + "%)\n";
        else
            fullString += "Dash Cooldown: " + instance.player.playerMovement.dashCooldown + "s\n";
        if (instance.gameplayManager.dashPowerMultiplier != 0)
            fullString += "Dash Power: " + instance.player.playerMovement.dashPower + " (" + (instance.gameplayManager.dashPowerMultiplier > 0 ? "+" + instance.gameplayManager.dashPowerMultiplier : instance.gameplayManager.dashPowerMultiplier) + "%)\n";
        else
            fullString += "Dash Power: " + instance.player.playerMovement.dashPower + "\n";
        if (instance.gameplayManager.magnetRangeMultiplier != 0)
            fullString += "Magnet Range: " + instance.player.magnetRange + " (" + (instance.gameplayManager.magnetRangeMultiplier > 0 ? "+" + instance.gameplayManager.magnetRangeMultiplier : instance.gameplayManager.magnetRangeMultiplier) + "%)\n";
        else
            fullString += "Magnet Range: " + instance.player.magnetRange + "\n";
        if (instance.gameplayManager.expMultiplier != 0)
            fullString += "Exp: " + instance.gameplayManager.exp + "/" + instance.gameplayManager.expCap + " (" + (instance.gameplayManager.expMultiplier > 0 ? "+" + instance.gameplayManager.expMultiplier : instance.gameplayManager.expMultiplier) + "%)\n";
        else
            fullString += "Exp: " + instance.gameplayManager.exp + "/" + instance.gameplayManager.expCap + "\n";
        if (instance.gameplayManager.coinDropChanceMultiplier != 0)
            fullString += "Coin Drop Chance: " + (instance.gameplayManager.coinDropChanceMultiplier > 0 ? "+" + instance.gameplayManager.coinDropChanceMultiplier : instance.gameplayManager.coinDropChanceMultiplier) + "%\n";
        if (instance.gameplayManager.dropChanceMultiplier != 0)
            fullString += "Drop Chance: " + (instance.gameplayManager.dropChanceMultiplier > 0 ? "+" + instance.gameplayManager.dropChanceMultiplier : instance.gameplayManager.dropChanceMultiplier) + "%\n";
        instance.inventoryManager.playerStats1Text.text = fullString;
        fullString = string.Empty;
        //Second stats
        for (int i = 0; i < instance.gameplayManager.baseDamageTypeAdditive.Count; i++)
        {
            if (instance.gameplayManager.baseDamageTypeAdditive[i] != 0)
            {
                if (i == 0) //physical damage
                    fullString += "<color=grey>Base Physical Damage</color>: " + (instance.gameplayManager.baseDamageTypeAdditive[i] > 0 ? "+" + instance.gameplayManager.baseDamageTypeAdditive[i] : instance.gameplayManager.baseDamageTypeAdditive[i]) + "\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Base Fire Damage</color>: " + (instance.gameplayManager.baseDamageTypeAdditive[i] > 0 ? "+" + instance.gameplayManager.baseDamageTypeAdditive[i] : instance.gameplayManager.baseDamageTypeAdditive[i]) + "\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Base Cold Damage</color>: " + (instance.gameplayManager.baseDamageTypeAdditive[i] > 0 ? "+" + instance.gameplayManager.baseDamageTypeAdditive[i] : instance.gameplayManager.baseDamageTypeAdditive[i]) + "\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Base Lightning Damage</color>: " + (instance.gameplayManager.baseDamageTypeAdditive[i] > 0 ? "+" + instance.gameplayManager.baseDamageTypeAdditive[i] : instance.gameplayManager.baseDamageTypeAdditive[i]) + "\n";
            }
        }
        if (instance.gameplayManager.damageMultiplier != 0)
            fullString += "Damage: " + (instance.gameplayManager.damageMultiplier > 0 ? "+" + instance.gameplayManager.damageMultiplier : instance.gameplayManager.damageMultiplier) + "%\n";
        if (instance.gameplayManager.projectileDamageMultiplier != 0)
            fullString += "Projectile Damage: " + (instance.gameplayManager.projectileDamageMultiplier > 0 ? "+" + instance.gameplayManager.projectileDamageMultiplier : instance.gameplayManager.projectileDamageMultiplier) + "%\n";
        if (instance.gameplayManager.meleeDamageMultiplier != 0)
            fullString += "Melee Damage: " + (instance.gameplayManager.meleeDamageMultiplier > 0 ? "+" + instance.gameplayManager.meleeDamageMultiplier : instance.gameplayManager.meleeDamageMultiplier) + "%\n";
        for (int i = 0; i < instance.gameplayManager.damageTypeMultiplier.Count; i++)
        {
            if (instance.gameplayManager.damageTypeMultiplier[i] != 0)
            {
                if (i == 0) //physical damage
                    fullString += "<color=grey>Physical Damage</color>: " + (instance.gameplayManager.damageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.damageTypeMultiplier[i] : instance.gameplayManager.damageTypeMultiplier[i]) + "%\n";
                else if (i == 1) //Fire damage
                    fullString += "<color=red>Fire Damage</color>: " + (instance.gameplayManager.damageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.damageTypeMultiplier[i] : instance.gameplayManager.damageTypeMultiplier[i]) + "%\n";
                else if (i == 2) //cold damage
                    fullString += "<color=blue>Cold Damage</color>: " + (instance.gameplayManager.damageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.damageTypeMultiplier[i] : instance.gameplayManager.damageTypeMultiplier[i]) + "%\n";
                else if (i == 3) //lightning damage
                    fullString += "<color=yellow>Lightning Damage</color>: " + (instance.gameplayManager.damageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.damageTypeMultiplier[i] : instance.gameplayManager.damageTypeMultiplier[i]) + "%\n";
            }
        }
        for (int i = 0; i < instance.gameplayManager.ailmentsChanceAdditive.Count; i++)
        {
            if (i == 0) //physical damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] != 0)
                    fullString += "Bleed Chance: " + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsChanceAdditive[i] : instance.gameplayManager.ailmentsChanceAdditive[i]) + "%\n";
                if (instance.gameplayManager.ailmentsEffectMultiplier[i] != 0)
                    fullString += "Bleed Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (" + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsEffectMultiplier[i] : instance.gameplayManager.ailmentsEffectMultiplier[i]) + "%)\n";
                else
                    fullString += "Bleed Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "%\n";
            }
            else if (i == 1) //Fire damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] != 0)
                    fullString += "Burn Chance: " + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsChanceAdditive[i] : instance.gameplayManager.ailmentsChanceAdditive[i]) + "%\n";
                if (instance.gameplayManager.ailmentsEffectMultiplier[i] != 0)
                    fullString += "Burn Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (" + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsEffectMultiplier[i] : instance.gameplayManager.ailmentsEffectMultiplier[i]) + "%)\n";
                else
                    fullString += "Burn Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "%\n";
            }
            else if (i == 2) //cold damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] != 0)
                    fullString += "Chill Chance: " + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsChanceAdditive[i] : instance.gameplayManager.ailmentsChanceAdditive[i]) + "%\n";
                if (instance.gameplayManager.ailmentsEffectMultiplier[i] != 0)
                    fullString += "Chill Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (" + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsEffectMultiplier[i] : instance.gameplayManager.ailmentsEffectMultiplier[i]) + "%)\n";
                else
                    fullString += "Chill Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "%\n";
            }
            else if (i == 3) //lightning damage
            {
                if (instance.gameplayManager.ailmentsChanceAdditive[i] != 0)
                    fullString += "Shock Chance: " + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsChanceAdditive[i] : instance.gameplayManager.ailmentsChanceAdditive[i]) + "%\n";
                if (instance.gameplayManager.ailmentsEffectMultiplier[i] != 0)
                    fullString += "Shock Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "% (" + (instance.gameplayManager.ailmentsChanceAdditive[i] > 0 ? "+" + instance.gameplayManager.ailmentsEffectMultiplier[i] : instance.gameplayManager.ailmentsEffectMultiplier[i]) + "%)\n";
                else
                    fullString += "Shock Effect: " + instance.gameplayManager.baseAilmentsEffect[i] * (1 + (instance.gameplayManager.ailmentsEffectMultiplier[i] / 100)) + "%\n";
            }
        }
        if (instance.gameplayManager.criticalChanceAdditive != 0)
            fullString += "Critical Chance: " + (instance.gameplayManager.criticalChanceAdditive > 0 ? "+" + instance.gameplayManager.criticalChanceAdditive : instance.gameplayManager.criticalChanceAdditive) + "%\n";
        if (instance.gameplayManager.projectileCriticalChanceAdditive != 0)
            fullString += "Projectile Critical Chance: " + (instance.gameplayManager.projectileCriticalChanceAdditive > 0 ? "+" + instance.gameplayManager.projectileCriticalChanceAdditive : instance.gameplayManager.projectileCriticalChanceAdditive) + "%\n";
        if (instance.gameplayManager.meleeCriticalChanceAdditive != 0)
            fullString += "Melee Critical Chance: " + (instance.gameplayManager.meleeCriticalChanceAdditive > 0 ? "+" + instance.gameplayManager.meleeCriticalChanceAdditive : instance.gameplayManager.meleeCriticalChanceAdditive) + "%\n";
        if (instance.gameplayManager.criticalDamageAdditive != 0)
            fullString += "Critical Damage: " + (instance.gameplayManager.criticalDamageAdditive > 0 ? "+" + instance.gameplayManager.criticalDamageAdditive : instance.gameplayManager.criticalDamageAdditive) + "%\n";
        if (instance.gameplayManager.projectileCriticalDamageAdditive != 0)
            fullString += "Projectile Critical Damage: " + (instance.gameplayManager.projectileCriticalDamageAdditive > 0 ? "+" + instance.gameplayManager.projectileCriticalDamageAdditive : instance.gameplayManager.projectileCriticalDamageAdditive) + "%\n";
        if (instance.gameplayManager.meleeCriticalDamageAdditive != 0)
            fullString += "Melee Critical Damage: " + (instance.gameplayManager.meleeCriticalDamageAdditive > 0 ? "+" + instance.gameplayManager.meleeCriticalDamageAdditive : instance.gameplayManager.meleeCriticalDamageAdditive) + "%\n";
        if (instance.gameplayManager.cooldownMultiplier != 0)
            fullString += "Cooldown: " + (instance.gameplayManager.cooldownMultiplier > 0 ? "+" + instance.gameplayManager.cooldownMultiplier : instance.gameplayManager.cooldownMultiplier) + "%\n";
        if (instance.gameplayManager.projectileCooldownMultiplier != 0)
            fullString += "Projectile Cooldown: " + (instance.gameplayManager.projectileCooldownMultiplier > 0 ? "+" + instance.gameplayManager.projectileCooldownMultiplier : instance.gameplayManager.projectileCooldownMultiplier) + "%\n";
        if (instance.gameplayManager.meleeCooldownMultiplier != 0)
            fullString += "Melee Cooldown: " + (instance.gameplayManager.meleeCooldownMultiplier > 0 ? "+" + instance.gameplayManager.meleeCooldownMultiplier : instance.gameplayManager.meleeCooldownMultiplier) + "%\n";
        if (instance.gameplayManager.projectileAmountAdditive != 0)
            fullString += "Projectile Amount: " + (instance.gameplayManager.projectileAmountAdditive > 0 ? "+" + instance.gameplayManager.projectileAmountAdditive : instance.gameplayManager.projectileAmountAdditive) + "\n";
        if (instance.gameplayManager.pierceAdditive != 0)
            fullString += "Pierce: " + (instance.gameplayManager.pierceAdditive > 0 ? "+" + instance.gameplayManager.pierceAdditive : instance.gameplayManager.pierceAdditive) + "\n";
        if (instance.gameplayManager.chainAdditive != 0)
            fullString += "Chain: " + (instance.gameplayManager.chainAdditive > 0 ? "+" + instance.gameplayManager.chainAdditive : instance.gameplayManager.chainAdditive) + "\n";
        if (instance.gameplayManager.meleeAmountAdditive != 0)
            fullString += "Melee Amount: " + (instance.gameplayManager.meleeAmountAdditive > 0 ? "+" + instance.gameplayManager.meleeAmountAdditive : instance.gameplayManager.meleeAmountAdditive) + "\n";
        if (instance.gameplayManager.comboAdditive != 0)
            fullString += "Combo: " + (instance.gameplayManager.comboAdditive > 0 ? "+" + instance.gameplayManager.comboAdditive : instance.gameplayManager.comboAdditive) + "\n";
        if (instance.gameplayManager.attackRangeMultiplier != 0)
            fullString += "Attack Range: " + (instance.gameplayManager.attackRangeMultiplier > 0 ? "+" + instance.gameplayManager.attackRangeMultiplier : instance.gameplayManager.attackRangeMultiplier) + "%\n";
        if (instance.gameplayManager.projectileAttackRangeMultiplier != 0)
            fullString += "Projectile Attack Range: " + (instance.gameplayManager.projectileAttackRangeMultiplier > 0 ? "+" + instance.gameplayManager.projectileAttackRangeMultiplier : instance.gameplayManager.projectileAttackRangeMultiplier) + "%\n";
        if (instance.gameplayManager.meleeAttackRangeMultiplier != 0)
            fullString += "Melee Attack Range: " + (instance.gameplayManager.meleeAttackRangeMultiplier > 0 ? "+" + instance.gameplayManager.meleeAttackRangeMultiplier : instance.gameplayManager.meleeAttackRangeMultiplier) + "%\n";
        if (instance.gameplayManager.lifeStealChanceAdditive != 0)
            fullString += "LifeSteal Chance: " + (instance.gameplayManager.lifeStealChanceAdditive > 0 ? "+" + instance.gameplayManager.lifeStealChanceAdditive : instance.gameplayManager.lifeStealChanceAdditive) + "%\n";
        if (instance.gameplayManager.projectileLifeStealChanceAdditive != 0)
            fullString += "Projectile LifeSteal Chance: " + (instance.gameplayManager.projectileLifeStealChanceAdditive > 0 ? "+" + instance.gameplayManager.projectileLifeStealChanceAdditive : instance.gameplayManager.projectileLifeStealChanceAdditive) + "%\n";
        if (instance.gameplayManager.meleeLifeStealChanceAdditive != 0)
            fullString += "Melee LifeSteal Chance: " + (instance.gameplayManager.meleeLifeStealChanceAdditive > 0 ? "+" + instance.gameplayManager.meleeLifeStealChanceAdditive : instance.gameplayManager.meleeLifeStealChanceAdditive) + "%\n";
        if (instance.gameplayManager.lifeStealAdditive != 0)
            fullString += "LifeSteal: " + (instance.gameplayManager.lifeStealAdditive > 0 ? "+" + instance.gameplayManager.lifeStealAdditive : instance.gameplayManager.lifeStealAdditive) + "\n";
        if (instance.gameplayManager.projectileLifeStealAdditive != 0)
            fullString += "Projectile LifeSteal: " + (instance.gameplayManager.projectileLifeStealAdditive > 0 ? "+" + instance.gameplayManager.projectileLifeStealAdditive : instance.gameplayManager.projectileLifeStealAdditive) + "\n";
        if (instance.gameplayManager.meleeLifeStealAdditive != 0)
            fullString += "Melee LifeSteal: " + (instance.gameplayManager.meleeLifeStealAdditive > 0 ? "+" + instance.gameplayManager.meleeLifeStealAdditive : instance.gameplayManager.meleeLifeStealAdditive) + "\n";
        if (instance.gameplayManager.travelRangeMultiplier != 0)
            fullString += "Travel Range: " + (instance.gameplayManager.travelRangeMultiplier > 0 ? "+" + instance.gameplayManager.travelRangeMultiplier : instance.gameplayManager.travelRangeMultiplier) + "%\n";
        if (instance.gameplayManager.projectileTravelRangeMultiplier != 0)
            fullString += "Projectile Travel Range: " + (instance.gameplayManager.projectileTravelRangeMultiplier > 0 ? "+" + instance.gameplayManager.projectileTravelRangeMultiplier : instance.gameplayManager.projectileTravelRangeMultiplier) + "%\n";
        if (instance.gameplayManager.meleeTravelRangeMultiplier != 0)
            fullString += "Melee Travel Range: " + (instance.gameplayManager.meleeTravelRangeMultiplier > 0 ? "+" + instance.gameplayManager.meleeTravelRangeMultiplier : instance.gameplayManager.meleeTravelRangeMultiplier) + "%\n";
        if (instance.gameplayManager.travelSpeedMultiplier != 0)
            fullString += "Travel Speed: " + (instance.gameplayManager.travelSpeedMultiplier > 0 ? "+" + instance.gameplayManager.travelSpeedMultiplier : instance.gameplayManager.travelSpeedMultiplier) + "%\n";
        if (instance.gameplayManager.projectileTravelSpeedMultiplier != 0)
            fullString += "Projectile Travel Range: " + (instance.gameplayManager.projectileTravelSpeedMultiplier > 0 ? "+" + instance.gameplayManager.projectileTravelSpeedMultiplier : instance.gameplayManager.projectileTravelSpeedMultiplier) + "%\n";
        if (instance.gameplayManager.meleeTravelSpeedMultiplier != 0)
            fullString += "Melee Travel Range: " + (instance.gameplayManager.meleeTravelSpeedMultiplier > 0 ? "+" + instance.gameplayManager.meleeTravelSpeedMultiplier : instance.gameplayManager.meleeTravelSpeedMultiplier) + "%\n";
        if (instance.gameplayManager.aoeMultiplier != 0)
            fullString += "Size: " + (instance.gameplayManager.aoeMultiplier > 0 ? "+" + instance.gameplayManager.aoeMultiplier : instance.gameplayManager.aoeMultiplier) + "%\n";
        if (instance.gameplayManager.projectileAoeMultiplier != 0)
            fullString += "Projectile Size: " + (instance.gameplayManager.projectileAoeMultiplier > 0 ? "+" + instance.gameplayManager.projectileAoeMultiplier : instance.gameplayManager.projectileAoeMultiplier) + "%\n";
        if (instance.gameplayManager.meleeAoeMultiplier != 0)
            fullString += "Melee Size: " + (instance.gameplayManager.meleeAoeMultiplier > 0 ? "+" + instance.gameplayManager.meleeAoeMultiplier : instance.gameplayManager.meleeAoeMultiplier) + "%\n";
        instance.inventoryManager.playerStats2Text.text = fullString;
    }

    public static void FormatEnemyStatsToString()
    {
        instance.inventoryManager.enemyStatsText.text = string.Empty;
        string fullString = "";
        fullString += "Max Health: " + (instance.gameplayManager.enemyMaxHealthMultiplier > 0 ? "+" + instance.gameplayManager.enemyMaxHealthMultiplier : instance.gameplayManager.enemyMaxHealthMultiplier) + "%\n";
        for (int i = 0; i < instance.gameplayManager.enemyReductions.Count; i++)
        {
            if (i == 0) //physical Reduction
            {
                fullString += "<color=grey>Physical Reduction</color>: " + (instance.gameplayManager.enemyReductions[i] > 0 ? "+" + instance.gameplayManager.enemyReductions[i] : instance.gameplayManager.enemyReductions[i]) + "%\n";
            }
            else if (i == 1) //Fire Reduction
            {
                fullString += "<color=red>Fire Reduction</color>: " + (instance.gameplayManager.enemyReductions[i] > 0 ? "+" + instance.gameplayManager.enemyReductions[i] : instance.gameplayManager.enemyReductions[i]) + "%\n";
            }
            else if (i == 2) //cold Reduction
            {
                fullString += "<color=blue>Cold Resistance</color>: " + (instance.gameplayManager.enemyReductions[i] > 0 ? "+" + instance.gameplayManager.enemyReductions[i] : instance.gameplayManager.enemyReductions[i]) + "%\n";
            }
            else if (i == 3) //lightning Reduction
            {
                fullString += "<color=yellow>Lightning Resistance</color>: " + (instance.gameplayManager.enemyReductions[i] > 0 ? "+" + instance.gameplayManager.enemyReductions[i] : instance.gameplayManager.enemyReductions[i]) + "%\n";
            }
        }
        fullString += "Damage: " + (instance.gameplayManager.enemyDamageMultiplier > 0 ? "+" + instance.gameplayManager.enemyDamageMultiplier : instance.gameplayManager.enemyDamageMultiplier) + "%\n";
        for (int i = 0; i < instance.gameplayManager.enemyDamageTypeMultiplier.Count; i++)
        {
            if (i == 0) //physical damage
            {
                fullString += "<color=grey>Physical Damage</color>: " + (instance.gameplayManager.enemyDamageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.enemyDamageTypeMultiplier[i] : instance.gameplayManager.enemyDamageTypeMultiplier[i]) + "%\n";
            }
            else if (i == 1) //Fire damage
            {
                fullString += "<color=red>Fire Damage</color>: " + (instance.gameplayManager.enemyDamageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.enemyDamageTypeMultiplier[i] : instance.gameplayManager.enemyDamageTypeMultiplier[i]) + "%\n";
            }
            else if (i == 2) //cold damage
            {
                fullString += "<color=blue>Cold Damage</color>: " + (instance.gameplayManager.enemyDamageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.enemyDamageTypeMultiplier[i] : instance.gameplayManager.enemyDamageTypeMultiplier[i]) + "%\n";
            }
            else if (i == 3) //lightning damage
            {
                fullString += "<color=yellow>Lightning Damage</color>: " + (instance.gameplayManager.enemyDamageTypeMultiplier[i] > 0 ? "+" + instance.gameplayManager.enemyDamageTypeMultiplier[i] : instance.gameplayManager.enemyDamageTypeMultiplier[i]) + "%\n";
            }
        }
        fullString += "Movement Speed: " + (instance.gameplayManager.enemyMoveSpeedMultiplier > 0 ? "+" + instance.gameplayManager.enemyMoveSpeedMultiplier : instance.gameplayManager.enemyMoveSpeedMultiplier) + "%\n";
        fullString += "Attack Cooldown: " + (instance.gameplayManager.enemyAttackCooldownMultiplier > 0 ? "+" + instance.gameplayManager.enemyAttackCooldownMultiplier : instance.gameplayManager.enemyAttackCooldownMultiplier) + "%\n";
        fullString += "Projectile: " + (instance.gameplayManager.enemyProjectileAdditive > 0 ? "+" + instance.gameplayManager.enemyProjectileAdditive : instance.gameplayManager.enemyProjectileAdditive) + "%\n";
        //fullString += "Projectile Size: " + (instance.gameplayManager.enemyProjectileSizeMultiplier > 0 ? "+" + instance.gameplayManager.enemyProjectileSizeMultiplier : instance.gameplayManager.enemyProjectileSizeMultiplier) + "%\n";
        //fullString += "Projectile Travel Range: " + instance.gameplayManager.enemyProjectileTravelRangeMultiplier + "%\n";
        fullString += "Projectile Travel Speed: " + (instance.gameplayManager.enemyProjectileTravelSpeedMultiplier > 0 ? "+" + instance.gameplayManager.enemyProjectileTravelSpeedMultiplier : instance.gameplayManager.enemyProjectileTravelSpeedMultiplier) + "%\n";
        instance.inventoryManager.enemyStatsText.text = fullString;
    }
    public static string FormatEnemyUpgradeToString(EnemyUpgradeManager.EnemyUpgrades mod)
    {
        string fullString = "";
        for (int i = 0; i < mod.modifier.Count; i++)
        {
            switch (mod.modifier[i])
            {
                case EnemyModifier.Attack_Cooldown: fullString += "Attack Cooldown: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Cold_Reduction: fullString += "Cold Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Damage: fullString += "Damage: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Fire_Reduction: fullString += "Fire Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Lightning_Reduction: fullString += "Lightning Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Max_Health: fullString += "Max Health: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Move_Speed: fullString += "Movement Speed: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Physical_Reduction: fullString += "Physical Resistance: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Projectile: fullString += "Projectile: +" + mod.amt[i] + "\n"; break;
                //case EnemyModifier.Projectile_size: fullString += "Projectile Size: +" + mod.amt[i] + "%\n"; break;
                //case EnemyModifier.Projectile_Travel_Range: fullString += "Projectile Travel Range: +" + mod.amt[i] + "%\n"; break;
                case EnemyModifier.Projectile_Travel_Speed: fullString += "Projectile Travel Speed: +" + mod.amt[i] + "%\n"; break;
                default: Debug.Log("FormatEnemyUpgradeToString has no switch case for " + mod.modifier[i]); break;
            }
        }
        return fullString;
    }
    public static void UpdateAllActiveSkills()
    {
        for (int i = 0; i < instance.gameplayManager.inventory.activeSkillList.Count; i++) //update all active skills
        {
            if (instance.gameplayManager.inventory.activeSkillList[i].skillController != null)
            {
                instance.gameplayManager.inventory.activeSkillList[i].skillController.UpdateSkillStats();
            }
        }
    }
    public bool CheckPercentModifier(Modifier mod) //Check if modifier is percent or flat value
    {
        //check for additive modifiers
        if (mod == Modifier.Melee_Amount || mod == Modifier.Projectile_Amount || mod == Modifier.Pierce || mod == Modifier.Chain || mod == Modifier.Combo ||
            mod == Modifier.Regen || mod == Modifier.Degen || mod == Modifier.Life_Steal || mod == Modifier.knockback ||
            mod == Modifier.Projectile_Life_Steal || mod == Modifier.Melee_Life_Steal || 
            mod ==Modifier.Base_Cold_Damage || mod == Modifier.Base_Fire_Damage || mod == Modifier.Base_Lightning_Damage || mod == Modifier.Base_Physical_Damage)
        {
            return true;
        }
        else return false;
    }
}
