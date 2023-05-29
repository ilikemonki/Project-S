using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    public Rigidbody2D rb;
    public Vector2 moveDirection;

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void PlayerMove() 
    { 
        rb.velocity = new Vector2(moveDirection.x * playerStats.currentMoveSpeed, moveDirection.y * playerStats.currentMoveSpeed);
    }
}

