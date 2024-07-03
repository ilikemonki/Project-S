using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class PlayerMovement : MonoBehaviour
{
    public PlayerStats player;
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public SpriteRenderer spriteRenderer;
    public Vector3 moveDirection;
    public float dashIFrameSeconds, baseDashCooldown, dashCooldown;
    public float baseDashPower, dashPower;
    public int baseCharges, maxCharges, currentCharges;
    public bool isDashing;
    float moveX, moveY, dashDirectionX, dashDirectionY;
    public float timer;
    private Vector3 smoothInput, smoothVelocity;
    public float afterIFrames; //the time the IFrame to stay for after dash ends
    public bool dashEnds, inIFrames;

    private void Start()
    {
        UpdateDashStats();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale >= 1)
        {
            InputManagement();
            if (currentCharges < maxCharges)
            {
                timer -= Time.deltaTime;
                player.gameplayManager.UpdateDashTime(timer);
                if (timer <= 0)
                {
                    currentCharges++;
                    player.gameplayManager.UpdateDashText();
                    timer = dashCooldown;
                    if (currentCharges == maxCharges)
                    {
                        player.gameplayManager.dashTimerText.text = "";
                    }
                }
            }
            if (dashEnds)
            {
                afterIFrames += Time.deltaTime;
                if (afterIFrames >= 0.15)
                {
                    dashEnds = false;
                    inIFrames = false;
                    afterIFrames = 0;
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
        smoothInput = Vector3.SmoothDamp(smoothInput, new Vector2(moveX, moveY).normalized, ref smoothVelocity, 0.1f);
        rb.MovePosition(transform.position + (player.moveSpeed * Time.fixedDeltaTime * smoothInput));        
        //rb.velocity = new Vector2(moveDirection.x * playertats.moveSpeed, moveDirection.y * playertats.moveSpeed);
    }

    public IEnumerator<float> Dash()
    {
        GameManager.totalDashes++;
        foreach (InventoryManager.Skill sc in player.gameplayManager.inventory.activeSkillList) //Check trigger skill condition
        {
            if (sc.skillController != null)
            {
                if (sc.skillController.skillTrigger.useDashTrigger)
                {
                    sc.skillController.UseSkill();
                }
            }
        }
        dashEnds = false;
        afterIFrames = 0;
        currentCharges--;
        player.gameplayManager.UpdateDashText();
        isDashing = true;
        inIFrames = true;
        trailRend.emitting = true;
        if (moveDirection.x > 0)
            dashDirectionX = player.moveSpeed;
        else if (moveDirection.x < 0)
            dashDirectionX = -player.moveSpeed;
        else dashDirectionX = 0;
        if (moveDirection.y > 0)
            dashDirectionY = player.moveSpeed;
        else if (moveDirection.y < 0)
            dashDirectionY = -player.moveSpeed;
        else dashDirectionY = 0;
        rb.velocity = new Vector2(moveDirection.x * dashPower + dashDirectionX, moveDirection.y * dashPower + dashDirectionY);
        yield return Timing.WaitForSeconds(dashIFrameSeconds);
        dashEnds = true;
        isDashing = false;
        trailRend.emitting = false;
        player.dodgeList.Clear();
    }
    public void UpdateDashStats()
    {
        dashPower = baseDashPower * (1 + (player.gameplayManager.dashPowerMultiplier / 100)); ;
        maxCharges = baseCharges + player.gameplayManager.dashChargesAdditive;
        currentCharges = maxCharges;
        dashCooldown = baseDashCooldown * (1 + (player.gameplayManager.dashCooldownMultiplier / 100));
        timer = dashCooldown;
    }
}

