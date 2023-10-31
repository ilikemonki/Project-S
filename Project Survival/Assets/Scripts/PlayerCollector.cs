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
        Timing.RunCoroutine(CollectibleDuration());
    }
    public IEnumerator<float> CollectibleDuration()
    {
        magnet.radius = 300;
        yield return Timing.WaitForSeconds(0.5f);
        magnet.radius = player.magnetRange;

    }
}
