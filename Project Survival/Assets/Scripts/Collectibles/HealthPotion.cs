using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : ICollectibles
{
    public float hp;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.Heal(hp);
            gameObject.SetActive(false);
        }
    }
}
