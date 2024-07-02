using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VampireBatBehavior : SkillBehavior
{

    public override void DoDamage(EnemyStats enemy, float damageEffectiveness)
    {
        totalDamage = damageTypes.Sum() * (damageEffectiveness / 100);
        isCrit = false;
        if (Random.Range(1, 101) <= skillController.criticalChance)  //Crit damage
        {
            isCrit = true;
            totalDamage *= (skillController.criticalDamage / 100);
        }
        if (skillController.highestDamageType.Equals(1))    //fire, burn
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[1])
            {
                enemy.ApplyBurn((damageTypes[1] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[1] / 100));
            }
        }
        else if (skillController.highestDamageType.Equals(2))   //cold, chill
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[2])
            {
                enemy.ApplyChill(skillController.ailmentsEffect[2]);
            }
        }
        else if (skillController.highestDamageType.Equals(3))   //lightning, shock
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[3])
            {
                enemy.ApplyShock(skillController.ailmentsEffect[3]);
            }
        }
        else //physical, bleed
        {
            if (Random.Range(1, 101) <= skillController.ailmentsChance[0])
            {
                enemy.ApplyBleed((damageTypes[0] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[0] / 100));
            }
        }
        enemy.TakeDamage(totalDamage, isCrit);
        if (skillController.damageCooldown > 0)
        {
            rememberEnemyList.Add(enemy);
        }
        if (skillController.knockBack > 0 && !enemy.knockedBack && !enemy.knockBackImmune)  //Apply knockback
        {
            enemy.KnockBack((enemy.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
        }
        if (skillController.isMelee && skillController.combo > 1 && !hasHitEnemy && skillController.comboCounter < skillController.combo)
        {
            skillController.comboCounter++;
        }
        hasHitEnemy = true;
        if (isCrit)
        {
            foreach (InventoryManager.Skill sc in skillController.player.gameplayManager.inventory.activeSkillList) //Check crit trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useCritTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
    }
    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponentInParent<EnemyStats>(); if (enemy == null || !enemy.isActiveAndEnabled) return;
            //Will damage the target only.
            if (hasHitEnemy) return;
            if (enemy.transform.Equals(target))
            {
                DoDamage(enemy, 100); 
                SetReturn();
                return;
            }
            else return;
        }
    }
    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && returnSkill)
        {
            if (Random.Range(1, 101) <= skillController.lifeStealChance && hasHitEnemy)  //Life Steal
            {
                if (skillController.player.currentHealth < skillController.player.maxHealth)
                {
                    skillController.player.Heal(skillController.lifeSteal, true);
                    GameManager.totalLifeStealProc++;
                    if (skillController.lifeSteal + skillController.player.currentHealth > skillController.player.maxHealth)
                        GameManager.totalLifeSteal += skillController.player.maxHealth - skillController.player.currentHealth;
                    else
                        GameManager.totalLifeSteal += skillController.lifeSteal;
                }
            }
            gameObject.SetActive(false);
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
}
