using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public Vector2 moveDirection;
    public float dashIFrameSeconds, dashCooldown;
    public float dashPower;
    public bool isDashing, canDash;
    float moveX, moveY;

    private void Start()
    {
        canDash = true;
    }
    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            PlayerMove();
        }
    }

    void InputManagement()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canDash)
            {
                if (moveX == 0 && moveY == 0)
                {

                }
                else Timing.RunCoroutine(Dashing());
            }
        }
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void PlayerMove() 
    { 
        rb.velocity = new Vector2(moveDirection.x * playerStats.moveSpeed, moveDirection.y * playerStats.moveSpeed);
    }

    public IEnumerator<float> Dashing()
    {
        Debug.Log(moveX + " " + moveY);
        canDash = false;
        isDashing = true;
        trailRend.emitting = true;
        rb.velocity = new Vector2(moveDirection.x * playerStats.moveSpeed * dashPower, moveDirection.y * playerStats.moveSpeed * dashPower);
        yield return Timing.WaitForSeconds(dashIFrameSeconds);
        isDashing = false;
        trailRend.emitting = false;
        yield return Timing.WaitForSeconds(dashCooldown);
        canDash = true;
    }
}

