using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class PlayerCollector : MonoBehaviour
{
    public PlayerStats player;
    public CircleCollider2D magnet;
    public float pullSpeed;
    public List<ICollectibles> collectibles= new();

    private void Awake()
    {
        SetMagnetRange(player.magnetRange);
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
                    collectibles[i].PullCollectible(pullSpeed, player.transform);
                }
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
                this.collectibles.Add(collectibles);
            }

        }
    }
    public void SetMagnetRange(float magnetRange)
    {
        magnet.radius = magnetRange;
    }
    public void MagnetCollectible()
    {
        Timing.RunCoroutine(CollectibleDuration());
    }
    public IEnumerator<float> CollectibleDuration()
    {
        magnet.radius = 300;
        yield return Timing.WaitForSeconds(0.5f);
        magnet.radius = player.magnetRange;

    }
}
