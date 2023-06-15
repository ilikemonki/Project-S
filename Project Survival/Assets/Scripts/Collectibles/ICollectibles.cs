using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ICollectibles : MonoBehaviour
{
    public Rigidbody2D rb;
    public TextMeshProUGUI text;
    public bool isCollecting;
    public SpriteRenderer spriteRenderer;
    public void PullCollectible(float pullspeed, Transform moveTo)
    {
            rb.MovePosition(transform.position + (pullspeed * Time.fixedDeltaTime * (moveTo.position - transform.position).normalized));
    }
    public IEnumerator StartDuration()
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
        StartCoroutine(StartDuration());
    }
}
