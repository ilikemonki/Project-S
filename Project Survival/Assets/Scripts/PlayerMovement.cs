using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public SpriteRenderer spriteRenderer;
    public Vector3 moveDirection;
    public float dashIFrameSeconds, baseDashCooldown, dashCooldown;
    public float baseDashPower, dashPower;
    public int baseCharges, maxCharges, currentCharges;
    public bool isDashing;
    float moveX, moveY;
    public float timer;

    private void Start()
    {
        UpdateDashStats();
    }
    // Update is called once per frame
    void Update()
    {
        InputManagement();
        if (currentCharges < maxCharges)
        {
            timer -= Time.deltaTime;
            playerStats.gameplayManager.UpdateDashTime(timer);
            if (timer <= 0)
            {
                currentCharges++;
                playerStats.gameplayManager.UpdateDashText();
                timer = dashCooldown;
                if (currentCharges == maxCharges)
                {
                    playerStats.gameplayManager.dashTimerText.text = "";
                }
            }
        }
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
        if (moveX > 0)
            spriteRenderer.flipX = false;
        else if (moveX < 0)
            spriteRenderer.flipX = true;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isDashing && currentCharges > 0)
            {
                if (moveX == 0 && moveY == 0)   //do not dash when standing still
                {

                }
                else Timing.RunCoroutine(Dash());
            }
        }
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void PlayerMove() 
    {
        rb.MovePosition(transform.position + (playerStats.moveSpeed * Time.fixedDeltaTime * moveDirection));        //rb.velocity = new Vector2(moveDirection.x * playerStats.moveSpeed, moveDirection.y * playerStats.moveSpeed);
    }

    public IEnumerator<float> Dash()
    {
        currentCharges--;
        playerStats.gameplayManager.UpdateDashText();
        isDashing = true;
        trailRend.emitting = true;
        rb.velocity = new Vector2(moveDirection.x * playerStats.moveSpeed * dashPower, moveDirection.y * playerStats.moveSpeed * dashPower);
        yield return Timing.WaitForSeconds(dashIFrameSeconds);
        isDashing = false;
        trailRend.emitting = false;
    }
    public void UpdateDashStats()
    {
        dashPower = baseDashPower * (1 + (playerStats.gameplayManager.dashPowerMultiplier / 100)); ;
        maxCharges = baseCharges + playerStats.gameplayManager.dashChargesAdditive;
        currentCharges = maxCharges;
        dashCooldown = baseDashCooldown * (1 + (playerStats.gameplayManager.dashCooldownMultiplier / 100));
        timer = dashCooldown;
    }
}

