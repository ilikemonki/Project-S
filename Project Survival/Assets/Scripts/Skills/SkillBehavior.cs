using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    public SkillController skillController;
    protected Vector3 direction;
    public List<float> damages;
    protected float speed;
    protected int pierce, chain;
    protected float despawnTime;
    public List<float> ailmentsChance;
    public List<float> ailmentsEffect;
    protected EnemyStats nearestEnemy;
    protected Transform target;
    protected float shortestDistance, distanceToEnemy;
    public Rigidbody2D rb;
    public List<EnemyStats> enemyChainList;    //remember the index of enemies hit by chain, will not hit the same enemy again.

    public void SetStats(List<float> damages, float speed, int pierce, int chain, float despawnTime, List<float> ailmentsChance, List<float> ailmentsEffect)
    {
        this.damages = damages;
        this.speed = speed;
        this.pierce = pierce;
        this.chain = chain;
        this.despawnTime = despawnTime;
        this.ailmentsChance = ailmentsChance;
        this.ailmentsEffect = ailmentsEffect;
    }

    protected virtual void Update()
    {
        if (target != null && target.gameObject.activeSelf)     //Have skill keep following target and change its angle.
        {
            SetDirection((target.position - transform.position).normalized);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        despawnTime -= Time.deltaTime;
        if (despawnTime <= 0f)
        {
            gameObject.SetActive(false);
        }
    }


    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void DoDamage(EnemyStats enemy)
    {
        float totalDamage = damages.Sum();
        bool isCrit = false;
        if (Random.Range(1, 101) <= skillController.criticalChance)  //Crit damage
        {
            isCrit = true;
            totalDamage *= (skillController.criticalDamage / 100);
        }
        Color textColor = Color.white;
        if (skillController.highestDamageType.Equals(1))    //fire, burn
        {
            textColor = Color.red; 
            if (Random.Range(1, 101) <= ailmentsChance[1] + skillController.gameplayManager.ailmentsChanceAdditive[1])
            {
                if (isCrit)
                    enemy.ApplyBurn(damages[1] * (skillController.criticalDamage / 100) * ((ailmentsEffect[1] + skillController.gameplayManager.ailmentsEffectAdditive[1]) / 100));
                else
                    enemy.ApplyBurn(damages[1] * ((ailmentsEffect[1] + skillController.gameplayManager.ailmentsEffectAdditive[1]) / 100));
            }
        }
        else if (skillController.highestDamageType.Equals(2))   //cold, chill
        {
            textColor = Color.cyan;
            if (Random.Range(1, 101) <= ailmentsChance[2] + skillController.gameplayManager.ailmentsChanceAdditive[2])
            {
                enemy.ApplyChill(ailmentsEffect[2] + skillController.gameplayManager.ailmentsEffectAdditive[2]);
            }
        }
        else if (skillController.highestDamageType.Equals(3))   //lightning, shock
        {
            textColor = Color.yellow;
            if (Random.Range(1, 101) <= ailmentsChance[3] + skillController.gameplayManager.ailmentsChanceAdditive[3])
            {
                enemy.ApplyShock(ailmentsEffect[3] + skillController.gameplayManager.ailmentsEffectAdditive[3]);
            }
        }
        else //physical, bleed
        {
            if (Random.Range(1, 101) <= ailmentsChance[0] + skillController.gameplayManager.ailmentsChanceAdditive[0])
            {
                if (isCrit)
                    enemy.ApplyBleed(damages[0] * (skillController.criticalDamage / 100) * ((ailmentsEffect[0] + skillController.gameplayManager.ailmentsEffectAdditive[0]) / 100));
                else
                    enemy.ApplyBleed(damages[0] * ((ailmentsEffect[0] + skillController.gameplayManager.ailmentsEffectAdditive[0]) / 100));
            }
        }
        enemy.TakeDamage(totalDamage, isCrit, textColor);
    }

    //Do damage when in contact w/ enemy
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("Rare Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (skillController.useMelee && enemyChainList.Contains(enemy)) //Melee will hit enemies only once
            {
                return;
            }
            DoDamage(enemy);
            if (skillController.knockBack > 0 && !enemy.knockedBack && !enemy.knockBackImmune)
            {
                enemy.KnockBack((col.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
            }
            if (pierce <= 0 && chain > 0 || skillController.useMelee) //check if there are chains, add enemy to list to not chain again.
            {
                if (!enemyChainList.Contains(enemy))    //if enemy is not in list, add it.
                {
                    enemyChainList.Add(enemy);
                }
            }
            if (!skillController.useMelee)
            {
                ProjectileBehavior();
            }

        }
    }

    //How projectile act after hitting enemy
    void ProjectileBehavior()
    {
        if (pierce <= 0 && chain <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        if (pierce > 0)  //behavior for pierce
        {
            pierce--;
        }
        else if (chain > 0)   //behavior for chain
        {
            chain--;
            ChainToEnemy();
        }
    }
    //Find new enemy to target.
    void  ChainToEnemy()
    {
        speed = skillController.chainSpeed;
        target = FindTarget(true);
        if (target != null)
        {
            despawnTime = skillController.despawnTime;
            SetDirection((target.position - transform.position).normalized); //if target is found, set direction
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else
        {
            gameObject.SetActive(false);    //despawn if no targets found
        }
    }

    public Transform FindTarget(bool closest)
    {
        if (closest) shortestDistance = Mathf.Infinity;
        else shortestDistance = 0;
        nearestEnemy = null;
        for (int i = 0; i < skillController.enemyManager.enemyList.Count; i++)  //find target in normal enemy list
        {
            bool dontChain = false;
            if (skillController.enemyManager.enemyList[i].isActiveAndEnabled)
            {
                for (int j = 0; j < enemyChainList.Count; j++)
                {
                    if (enemyChainList.Contains(skillController.enemyManager.enemyList[i]))
                    {
                        dontChain = true;
                        break;
                    }
                }
                if (!dontChain)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyManager.enemyList[i].transform.position);
                    if (!closest)
                    {
                        if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.chainRange)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = skillController.enemyManager.enemyList[i];
                        }
                    }
                    else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.chainRange)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyManager.enemyList[i];
                    }
                }
            }
        }
        for (int i = 0; i < skillController.enemyManager.rareEnemyList.Count; i++)  //find target in rare enemy list
        {
            bool dontChain = false;
            if (skillController.enemyManager.rareEnemyList[i].isActiveAndEnabled)
            {
                for (int j = 0; j < enemyChainList.Count; j++)
                {
                    if (enemyChainList.Contains(skillController.enemyManager.enemyList[i]))
                    {
                        dontChain = true;
                        break;
                    }
                }
                if (!dontChain)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyManager.rareEnemyList[i].transform.position); 
                    if (!closest)
                    {
                        if (distanceToEnemy > shortestDistance && distanceToEnemy <= skillController.chainRange)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = skillController.enemyManager.rareEnemyList[i];
                        }
                    }
                    else if (distanceToEnemy < shortestDistance && distanceToEnemy <= skillController.chainRange)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyManager.rareEnemyList[i];
                    }
                }
            }
        }
        if (nearestEnemy != null)
        {
            return nearestEnemy.transform;
        }
        else return null;
    }

    private void OnDisable()
    {
        enemyChainList.Clear();
    }

}
