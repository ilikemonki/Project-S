using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : ICollectibles
{
    public int coins;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameplayManager.GainCoins(coins); 
            isCollecting = false;
            gameObject.SetActive(false);
        }
    }
}
