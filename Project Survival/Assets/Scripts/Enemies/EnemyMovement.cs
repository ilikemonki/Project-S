using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public void MoveEnemy(Enemy enemy)
    {
        enemy.rb.MovePosition(enemy.transform.position + (4 * Time.fixedDeltaTime * (enemy.enemyStats.enemyManager.player.transform.position - enemy.transform.position).normalized));
        //Face player
        if (enemy.enemyStats.enemyManager.player.transform.position.x > enemy.transform.position.x)
        {
            enemy.spriteRenderer.flipX = false;
        }
        else enemy.spriteRenderer.flipX = true;
    }

}
