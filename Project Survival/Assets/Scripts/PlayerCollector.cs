using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class PlayerCollector : MonoBehaviour
{
    public PlayerStats player;
    public CircleCollider2D magnet;
    public float pullSpeed;
    public List<ICollectibles> collectiblesList= new();
    float magnetTimer;
    bool useMagnet;

    private void FixedUpdate()
    {
        if (collectiblesList.Count > 0)
        {
            for (int i = 0; i < collectiblesList.Count; ++i)
            {
                if (collectiblesList[i] == null)
                {
                    collectiblesList.RemoveAt(i);
                }
                else
                {
                    collectiblesList[i].PullCollectible(pullSpeed, player.transform);
                }
            }
        }
        if (useMagnet)
        {
            magnetTimer += Time.deltaTime;
            if (magnetTimer >= 0.5f)
            {
                useMagnet = false;
                magnetTimer = 0;
                magnet.radius = player.magnetRange;
            }
        }
    }

    //When magnet radius hits collectible, pull them in
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICollectibles collectibles))
        {
            if (!collectibles.isCollecting && !collision.CompareTag("Magnet") && !collision.CompareTag("Health Potion"))
            {
                collectibles.isCollecting = true;
                collectiblesList.Add(collectibles);
                collectibles.PullCollectible(pullSpeed, player.transform);
            }

        }
    }
    public void SetMagnetRange(float magnetRange)
    {
        magnet.radius = magnetRange;
    }
    public void MagnetCollectible()
    {
        magnet.radius = 400;
        magnetTimer = 0;
        useMagnet = true;
    }
}
