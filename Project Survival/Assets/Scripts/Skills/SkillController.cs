using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public GameObject prefab;
    public int level;
    public float damage;
    public float speed;
    public float attackRange;
    public float chainRange;
    public float cooldown;
    public float currentCooldown;
    public float despawnTime;
    public int strike = 1, projectile, pierce, chain;
    public List<SkillBehavior> poolList = new();
    public float shortestDistance, distanceToEnemy;
    public EnemyController enemyController;
    public FloatingTextController floatingTextController;
    public EnemyStats nearestEnemy;
    public Transform target;
    int maxLoops = 10000;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentCooldown = cooldown;
        PopulatePool((projectile * strike) * 4);
        InvokeRepeating(nameof(UpdateTarget), 0, 0.25f);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            UseSkill();
        }
    }
    //Check for closest enemy to target
    protected virtual void UpdateTarget()
    {
        if (maxLoops <= 0)
        {
            Debug.Log("max update target reached");
            return;
        }
        maxLoops--;
        shortestDistance = Mathf.Infinity;
        nearestEnemy = null;
        for (int i = 0; i < enemyController.enemyList.Count; i++)
        {
            if (enemyController.enemyList[i].isActiveAndEnabled)
            {
                distanceToEnemy = Vector3.Distance(transform.position, enemyController.enemyList[i].transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemyController.enemyList[i];
                }

            }
        }
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else target = null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    //Activate skill
    protected virtual void UseSkill()
    {
        if (target == null) return;
        CheckPoolAmount();
        currentCooldown = cooldown;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (!poolList[i].isActiveAndEnabled)
            {
                //Put this in the other controllers later
                poolList[i].transform.position = transform.position;    //set starting position on player
                poolList[i].SetDirection((target.position - transform.position).normalized);   //Set direction
                poolList[i].gameObject.SetActive(true);
                return;
            }
        }
    }

    protected virtual void CheckPoolAmount()
    {
        int inactive = 0;
        for (int i = 0; i < poolList.Count; i++)
        {
            if (!poolList[i].isActiveAndEnabled) inactive++;    //Calculate how many inactives there are
        }
        if (inactive <= (projectile * strike) * 2) //if inactives are close to the number of prefabs needed, spawn more.
        {
            PopulatePool((projectile * strike) * 2);
        }
    }

    protected virtual void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject skill = Instantiate(prefab, gameObject.transform);    //Spawn, add to list, and initialize prefabs
            skill.SetActive(false);
            SkillBehavior sb = skill.GetComponent<SkillBehavior>();
            poolList.Add(sb);
        }
    }
}