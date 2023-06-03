using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    public PlayerStats player;
    public CircleCollider2D magnet;
    public float pullSpeed;
    public List<ICollectibles> collectibles= new();

    private void Awake()
    {
        magnet.radius = player.magnetRange;
    }

    private void FixedUpdate()
    {
        if(collectibles.Count > 0)
        {
            for (int i = 0; i < collectibles.Count; ++i)
            {
                if (collectibles[i] == null)
                { 
                    collectibles.RemoveAt(i); 
                }
                else
                {
                    collectibles[i].PullCollectible(pullSpeed);
                    if (!collectibles[i].isActiveAndEnabled)
                    {
                        collectibles.Remove(collectibles[i]);
                    }
                }
            }
        }
    }

    //When magnet radius hits collectible, pull them in
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICollectibles collectibles))
        {
            if (!collectibles.isCollecting)
            {
                this.collectibles.Add(collectibles);
                collectibles.isCollecting = true;
            }

        }
    }
}
