using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public PlayerStats player;
    public Rigidbody2D rb;
    public TrailRenderer trailRend;
    public SpriteRenderer spriteRenderer;
    public List<Image> dashImageList = new();
    public GameObject dashImageParent;
    public Image dashImagePrefab;
    public Image dashImageOnCooldown;
    public Vector3 moveDirection;
    public float dashIFrameSeconds, baseDashCooldown, dashCooldown;
    public float baseDashPower, dashPower;
    public int baseDashCharges, maxDashCharges, currentDashCharges;
    public bool isDashing;
    float moveX, moveY, dashDirectionX, dashDirectionY;
    public float timer;
    private Vector3 smoothInput, smoothVelocity;
    public float afterIFrames; //the time the IFrame to stay for after dash ends
    public bool dashEnds, inIFrames;

    private void Start()
    {
        UpdateDashStats();
        currentDashCharges = maxDashCharges;
        timer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale >= 1)
        {
            InputManagement();
            if (currentDashCharges < maxDashCharges && dashImageOnCooldown != null)
            {
                timer += Time.deltaTime;
                dashImageOnCooldown.fillAmount = timer / dashCooldown;
                if (timer > dashCooldown)
                {
                    dashImageOnCooldown.fillAmount = 1;
                    dashImageOnCooldown = null;
                    currentDashCharges++;
                    timer = 0;
                    FindImageOnCooldown();
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
    public void FindImageOnCooldown()
    {
        if (currentDashCharges >= maxDashCharges) return;
        if (dashImageOnCooldown == null)
        {
            for (int i = dashImageList.Count - 1; i >= 0; i--)
            {
                if (!dashImageList[i].gameObject.activeSelf)
                {
                    dashImageOnCooldown = dashImageList[i];
                    dashImageOnCooldown.fillAmount = 0;
                    dashImageList.Remove(dashImageList[i]);
                    dashImageList.Add(dashImageOnCooldown);
                    dashImageOnCooldown.transform.SetAsLastSibling();
                    dashImageOnCooldown.gameObject.SetActive(true);
                    return;
                }
            }
        }
    }
    public void InactivateLastDashImage()
    {
        for (int i = dashImageList.Count - 1; i >= 0; i--)
        {
            if (dashImageList[i].gameObject.activeSelf)
            {
                if (dashImageOnCooldown != null)
                {
                    if (!dashImageList[i].Equals(dashImageOnCooldown))
                    {
                        dashImageList[i].gameObject.SetActive(false);
                        return;
                    }
                }
                else
                {
                    dashImageList[i].gameObject.SetActive(false);
                    return;
                }
            }
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
            if (!isDashing && currentDashCharges > 0)
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
        //rb.velocity = new Vector2(moveDirection.x * player.moveSpeed, moveDirection.y * player.moveSpeed);
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
        currentDashCharges--;
        isDashing = true;
        inIFrames = true;
        trailRend.emitting = true;
        InactivateLastDashImage();
        FindImageOnCooldown();
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
        maxDashCharges = baseDashCharges + player.gameplayManager.dashChargesAdditive;
        dashCooldown = baseDashCooldown * (1 + (player.gameplayManager.dashCooldownMultiplier / 100));
        SetDashImages();
    }
    public void SetDashImages()
    {
        if (dashImageList.Count < maxDashCharges)
        {
            int p = maxDashCharges - dashImageList.Count;
            for (int i = 0; i < p; i++)
            {
                Image img = Instantiate(dashImagePrefab, dashImageParent.transform);
                dashImageList.Add(img);
            }
        }
        if (dashImageList.Count > maxDashCharges)
        {
            int p = dashImageList.Count - maxDashCharges;
            for (int i = 0; i < p; i++)
            {
                Destroy(dashImageList[i]);
                dashImageList.RemoveAt(i);
            }
        }
    }
}

