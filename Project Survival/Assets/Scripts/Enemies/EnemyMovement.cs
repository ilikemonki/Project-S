using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyStats enemyStats;
    private void Start()
    {
        InvokeRepeating(nameof(AlwaysFacePlayer), 0, 0.25f);
    }
    public void MoveEnemy()
    {
        enemyStats.rb.MovePosition(transform.position + (enemyStats.moveSpeed * Time.fixedDeltaTime * (enemyStats.enemyManager.player.transform.position - transform.position).normalized));
    }

    public void AlwaysFacePlayer()
    {
        if (enemyStats.enemyManager.player.transform.position.x > transform.position.x)
        {
            enemyStats.spriteRenderer.flipX = false;
        }
        else enemyStats.spriteRenderer.flipX = true;
    }

}
