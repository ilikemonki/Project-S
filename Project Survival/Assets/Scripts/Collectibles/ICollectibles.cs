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
        transform.position = Vector2.MoveTowards(transform.position, gameplayManager.player.transform.position, pullspeed * Time.deltaTime);
    }

    public void OnDisable()
    {
        isCollecting = false;
    }

}
