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
    public float dashIFrameSeconds, baseDashCooldown, dashCooldown;
    public float dashPower;
    public int charges;
    public bool isDashing, canDash;
    float moveX, moveY;

    private void Start()
    {
        dashCooldown = baseDashCooldown;
        UpdateDashStats();
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
            if (!isDashing && charges > 0)
            {
                if (moveX == 0 && moveY == 0)   //do not dash
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
        charges--;
        canDash = false;
        isDashing = true;
        trailRend.emitting = true;
        rb.velocity = new Vector2(moveDirection.x * playerStats.moveSpeed * dashPower, moveDirection.y * playerStats.moveSpeed * dashPower);
        yield return Timing.WaitForSeconds(dashIFrameSeconds);
        isDashing = false;
        trailRend.emitting = false;
        yield return Timing.WaitForSeconds(dashCooldown);
        charges++;
        canDash = true;
    }
    public void UpdateDashStats()
    {
        charges += playerStats.gameplayManager.dashChargesAdditive;
        dashCooldown = baseDashCooldown * (1 + (playerStats.gameplayManager.dashCooldownMultiplier / 100));
    }
}

