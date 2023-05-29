using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemyMovement enemyMovement;
    public float moveSpeed;
    public float maxHealth;
    public float damage;
    public float currentMoveSpeed;
    public float currentHealth;
    public int exp;
    public bool spawnInPool;
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        //player = GetComponentInParent<PlayerStats>();
        //enemyMovement.playerTransform = player.transform;
        enemyMovement.SetMoveSpeed(currentMoveSpeed);
    }



    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    public void Die()
    {
        gameObject.SetActive(false);
    }


    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        enemyController.player.TakeDamage(damage);  //Do damage to player
    //    }
    //}
    //Becareful if destorying gameobject, this may get called.
    private void OnDisable()
    {
        if (!spawnInPool)
        {
            enemyManager.gameplayMananger.GainExp(exp);
            enemyManager.gameplayMananger.CalculateKillCounter();
            enemyManager.enemiesAlive--;
        }
    }
}
