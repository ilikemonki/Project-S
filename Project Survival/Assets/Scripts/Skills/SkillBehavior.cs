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
    protected EnemyStats nearestEnemy;
    protected Transform target;
    protected float shortestDistance, distanceToEnemy;
    public Rigidbody2D rb;
    public List<int> enemyIndexChain;    //remember the index of enemies hit by chain, will not hit the same enemy again.

    public void SetStats(List<float> damages, float speed, int pierce, int chain, float despawnTime)
    {
        this.damages = damages;
        this.speed = speed;
        this.pierce = pierce;
        this.chain = chain;
        this.despawnTime = despawnTime;
    }

    protected virtual void Update()
    {
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
        float totalDamage = Mathf.Ceil(damages.Sum());
        Color textColor = Color.white;
        if (skillController.highestDamageType.Equals(1)) textColor = Color.red;
        else if (skillController.highestDamageType.Equals(2)) textColor = Color.cyan;
        else if(skillController.highestDamageType.Equals(3)) textColor = Color.yellow;
        if (Random.Range(0, 100) <= skillController.criticalChance)  //Crit damage
        {
            totalDamage *= (skillController.criticalDamage / 100);
            enemy.TakeDamage(totalDamage);
            skillController.floatingTextController.DisplayFloatingCritText(enemy.transform, totalDamage, textColor);
        }
        else
        {
            enemy.TakeDamage(totalDamage);
            skillController.floatingTextController.DisplayFloatingText(enemy.transform, totalDamage, textColor);
        }
    }

    //Do damage when in contact w/ enemy
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            DoDamage(enemy);
            if (pierce <= 0 && chain > 0) //check if there are chains, add enemy to list to not chain again.
            {
                if (!enemyIndexChain.Contains(skillController.enemyController.enemyList.IndexOf(enemy)))    //if enemy is not in list, add it.
                {
                    enemyIndexChain.Add(skillController.enemyController.enemyList.IndexOf(enemy));
                }
            }
            ProjectileBehavior();
            if (skillController.knockBack >= 1 && !enemy.knockedBack)
            {
                enemy.KnockBack((col.transform.position - skillController.player.transform.position).normalized * skillController.knockBack);
            }

        }
    }

    //How projectile act after hitting enemy
    void ProjectileBehavior()
    {
        if (pierce <= 0 && chain <= 0)
        {
            gameObject.SetActive(false);
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
        shortestDistance = Mathf.Infinity;
        nearestEnemy = null;
        for (int i = 0; i < skillController.enemyController.enemyList.Count; i++)
        {
            bool dontChain = false;
            if (skillController.enemyController.enemyList[i].isActiveAndEnabled)
            {
                for (int j = 0; j < enemyIndexChain.Count; j++)
                {
                    if (enemyIndexChain[j] == i)
                    {
                        dontChain = true;
                        break;
                    }
                }
                if (!dontChain)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyController.enemyList[i].transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = skillController.enemyController.enemyList[i];
                    }
                }
            }
        }
        if (nearestEnemy != null && shortestDistance <= skillController.chainRange)
        {
            target = nearestEnemy.transform;
        }
        else target = null;
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

    private void OnDisable()
    {
        enemyIndexChain.Clear();
    }

}
