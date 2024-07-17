using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VampireBatBehavior : SkillBehavior
{
    public override void Update()
    {
        base.Update();
    }
    public override void DoDamage(Enemy enemy, float damageEffectiveness)
    {
        totalDamage = damageTypes.Sum() * (damageEffectiveness / 100);
        if (applyCrit)  //Crit damage
        {
            totalDamage *= (skillController.criticalDamage / 100);
        }
        if (applyAilment[1])    //fire, burn
        {
            enemy.enemyStats.ApplyBurn(enemy, (damageTypes[1] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[1] / 100));
        }
        else if (applyAilment[2])   //cold, chill
        {
            enemy.enemyStats.ApplyChill(enemy, skillController.ailmentsEffect[2]);
        }
        else if (applyAilment[3])   //lightning, shock
        {
            enemy.enemyStats.ApplyShock(enemy, skillController.ailmentsEffect[3]);
        }
        else if (applyAilment[0]) //physical, bleed
        {
            enemy.enemyStats.ApplyBleed(enemy, (damageTypes[0] * (damageEffectiveness / 100)) * (skillController.ailmentsEffect[0] / 100));
        }
        enemy.enemyStats.TakeDamage(enemy, totalDamage, applyCrit);
        if (skillController.damageCooldown > 0)
        {
            rememberEnemyList.Add(enemy);
        }
        if (skillController.knockBack > 0 && !enemy.knockedBack && !enemy.enemyStats.knockBackImmune)  //Apply knockback
        {
            enemy.enemyStats.KnockBack(enemy, (enemy.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
        }
        if (skillController.isMelee && skillController.combo > 1 && !hasHitEnemy && skillController.comboCounter < skillController.combo)
        {
            skillController.comboCounter++;
        }
        hasHitEnemy = true;
        if (applyCrit)
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
            Enemy enemy = col.GetComponentInParent<Enemy>(); if (enemy == null || !enemy.enemyStats.gameObject.activeSelf) return;
            //Will damage the target only.
            if (hasHitEnemy) return;
            if (enemy.enemyStats.transform.Equals(target))
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
        if (returnSkill && !target.Equals(skillController.player.transform))
        {
            SetReturn();
        }
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
}
