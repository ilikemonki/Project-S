using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ICollectibles : MonoBehaviour
{
    public int coinAmount;
    public Rigidbody2D rb;
    public TextMeshProUGUI text;
    public bool isCollecting;
    public SpriteRenderer spriteRenderer;
    public float pullSpeed;

    public void PullCollectible(Transform moveTo)
    {
        pullSpeed += 0.1f;
        rb.MovePosition(transform.position + (pullSpeed * Time.fixedDeltaTime * (moveTo.position - transform.position).normalized));
    }
    public IEnumerator StartCountdown()
    {
        int timer = 15;
        while (timer > 0)
        {
            text.text = timer.ToString() + "s";
            yield return new WaitForSeconds(1);
            timer--;
        }
        gameObject.SetActive(false);
    }
    public void OnEnable()
    {
        pullSpeed = 4;
        isCollecting = false;
    }

    public void OnDisable()
    {
        StopAllCoroutines();
        if (text != null) text.text = "";
        isCollecting = false;
    }
    public void MagnetDuration()
    {
        StartCoroutine(StartCountdown());
    }
}
