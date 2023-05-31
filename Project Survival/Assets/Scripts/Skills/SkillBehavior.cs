using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBehavior : MonoBehaviour
{
    protected SkillController skillController;
    protected Vector3 direction;

    protected float currentDamage, currentSpeed;
    protected int currentPierce, currentChain;
    protected float currentDespawnTime;
    protected EnemyStats nearestEnemy;
    protected Transform target;
    protected float shortestDistance, distanceToEnemy;
    // Start is called before the first frame update
    private void Awake()
    {
        skillController = GetComponentInParent<SkillController>();
        currentDamage = skillController.damage;
        currentSpeed = skillController.speed;
        currentPierce = skillController.pierce;
        currentChain = skillController.chain;
        currentDespawnTime = skillController.despawnTime;
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
            enemy.TakeDamage(currentDamage);
            skillController.floatingTextController.DisplayDamageText(enemy.transform, currentDamage);
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
            StartCoroutine(ChainToEnemy());
        }
    }

    IEnumerator ChainToEnemy()
    {
        yield return new WaitForSeconds(0.1f);
        shortestDistance = Mathf.Infinity;
        nearestEnemy = null;
        for (int i = 0; i < skillController.enemyController.enemyList.Count; i++)
        {
            if (skillController.enemyController.enemyList[i].isActiveAndEnabled)
            {
                distanceToEnemy = Vector3.Distance(transform.position, skillController.enemyController.enemyList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = skillController.enemyController.enemyList[i];
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
        currentPierce = skillController.pierce;
        currentChain = skillController.chain;
        currentDespawnTime = skillController.despawnTime;
    }

}
