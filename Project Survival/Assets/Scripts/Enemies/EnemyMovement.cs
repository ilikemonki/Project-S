using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody2D rb;
    float moveSpeed;

    public void MoveEnemy()
    {
        rb.MovePosition(transform.position + (moveSpeed * Time.deltaTime * (playerTransform.transform.position - transform.position).normalized));
        //transform.position = Vector2.MoveTowards(transform.position, playerTransform.transform.position, moveSpeed * Time.deltaTime);
    }

    public void SetMoveSpeed(float mvs)
    {
        moveSpeed = mvs;
    }

}
