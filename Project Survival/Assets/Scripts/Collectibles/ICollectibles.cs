using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICollectibles : MonoBehaviour
{
    public GameplayManager gameplayManager;
    public Rigidbody2D rb;
    public bool isCollecting;
    public void PullCollectible(float pullspeed)
    {
        if (gameplayManager != null)
        {
            rb.MovePosition(transform.position + (pullspeed * Time.fixedDeltaTime * (gameplayManager.player.transform.position - transform.position).normalized));
        }
    }
    public void OnEnable()
    {
        isCollecting = false;
    }

    public void OnDisable()
    {
        isCollecting = false;
    }

}
