using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform playerTransform;
    float moveSpeed;

    public void MoveEnemy()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.transform.position, moveSpeed * Time.deltaTime);
    }

    public void SetMoveSpeed(float mvs)
    {
        moveSpeed = mvs;
    }

}
