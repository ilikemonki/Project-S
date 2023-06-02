using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    public SkillController skillController;
    protected Vector3 direction;
    protected float currentDamage, currentSpeed;
    protected int currentPierce, currentChain;
    protected float currentDespawnTime;
    protected EnemyStats nearestEnemy;
    protected Transform target;
    protected float shortestDistance, distanceToEnemy;
    public List<int> enemyIndexChain;    //remember the index of enemies hit by chain, will not hit the same enemy again.
    // Start is called before the first frame update

    public void SetStats(float damage, float speed, int pierce, int chain, float despawnTime)
    {
        currentDamage = damage;
        currentSpeed = speed;
        currentPierce = pierce;
        currentChain = chain;
        currentDespawnTime = despawnTime;
    }

    protected virtual void Update()
    {
        currentDespawnTime -= Time.deltaTime;
        if (currentDespawnTime <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    //Do damage when in contact w/ enemy
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>(); 
            if (skillController.knockBackForce >= 1 && !enemy.knockedBack)
            {
                enemy.KnockBack((col.transform.position - skillController.player.transform.position).normalized * skillController.knockBackForce);
            }
            enemy.TakeDamage(currentDamage);
            skillController.floatingTextController.DisplayDamageText(enemy.transform, currentDamage);
            if (currentPierce <= 0 && currentChain > 0) //check if there are chains, add enemy to list to not chain again.
            {
                if (!enemyIndexChain.Contains(skillController.enemyController.enemyList.IndexOf(enemy)))    //if enemy is not in list, add it.
                {
                    enemyIndexChain.Add(skillController.enemyController.enemyList.IndexOf(enemy));
                }
            }
            ProjectileBehavior();

        }
    }

    //How projectile act after hitting enemy
    void ProjectileBehavior()
    {
        if (currentPierce <= 0 && currentChain <= 0)
        {
            gameObject.SetActive(false);
        }
        if (currentPierce > 0)  //behavior for pierce
        {
            currentPierce--;
        }
        else if (currentChain > 0)   //behavior for chain
        {
            currentChain--;
            ChainToEnemy();
        }
    }
    
    void  ChainToEnemy()
    {
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
            currentDespawnTime = skillController.despawnTime;   //reset despawntime
            SetDirection((target.position - transform.position).normalized); //if target is found, set direction
        }
        else
        {
            gameObject.SetActive(false);    //despawn if no targets found
        }

    }


    private void OnEnable()
    {
        if (skillController != null)
        {
            currentPierce = skillController.pierce;
            currentChain = skillController.chain;
            currentDespawnTime = skillController.despawnTime;
        }
    }
    private void OnDisable()
    {
        enemyIndexChain.Clear();
    }

}
