using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneShardEffect : PassiveItemEffect
{
    //Enemies killed will shoot 2 bone shards that deal x melee base physical damage. Damage is affected by your stats.
    public SimpleProjectile bonePrefab;
    public List<SimpleProjectile> boneList;
    public float numOfBoneShards;
    public List<Transform> deadEnemyList;
    public void Update()
    {
        if (deadEnemyList.Count > 0)
        {
            for (int i = deadEnemyList.Count - 1; i >= 0; i--)
            {
                int count = 0;
                for (int j = 0; j < boneList.Count; j++)
                {
                    if (j > boneList.Count - 4)
                    {
                        PopulatePool(20);
                    }
                    if (!boneList[j].isActiveAndEnabled)
                    {
                        //Set bone shard info
                        boneList[j].direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
                        boneList[j].damageTypes[0] = damage;
                        boneList[j].travelRange = 3;
                        boneList[j].travelSpeed = 8;
                        boneList[j].transform.position = deadEnemyList[i].transform.position;
                        boneList[j].gameObject.SetActive(true);
                        count++;
                    }
                    if (count == numOfBoneShards) break;
                }
                deadEnemyList.Remove(deadEnemyList[i]);
            }
        }
    }
    public void FixedUpdate()
    {
        for (int i = 0; i < boneList.Count; i++)  //Loop through all projectiles and move them.
        {
            if (boneList[i].isActiveAndEnabled)
            {
                boneList[i].currentRange = Vector3.Distance(boneList[i].transform.position, boneList[i].startingPos);
                if (boneList[i].currentRange >= boneList[i].travelRange)
                {
                    boneList[i].gameObject.SetActive(false);
                }
                boneList[i].rb.MovePosition(boneList[i].transform.position + (boneList[i].travelSpeed * Time.deltaTime * boneList[i].direction));
            }
        }
    }
    public override void CheckCondition(GameObject obj)
    {
        if (checkCondition)
        {
            deadEnemyList.Add(obj.transform);
        }
    }
    public override void RemoveEffect()
    {
        if (boneList.Count > 0)
        {
            for (int i = 0; i < boneList.Count; ++i)
            {
                Destroy(boneList[i]);
            }
            boneList.Clear();
        }
        deadEnemyList.Clear();
        checkCondition = false;
        gameObject.SetActive(false);
    }
    public override void WhenAcquired()
    {
        UpdateItemStats();
        PopulatePool(20);
        checkCondition = true;
        gameObject.SetActive(true);
    }
    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SimpleProjectile obj = Instantiate(bonePrefab, transform);    //Spawn, add to list, and initialize prefabs
            for(int k = 0; k < 4; k++)
            {
                obj.damageTypes.Add(0);
            }
            obj.passiveItemEffect = this;
            obj.gameObject.SetActive(false);
            boneList.Add(obj);
        }
    }
    public override void UpdateItemStats()
    {
        damage = (baseDamage + gameplayManager.baseDamageTypeAdditive[0]) * (1 + (gameplayManager.damageTypeMultiplier[0] + gameplayManager.damageMultiplier + gameplayManager.meleeDamageMultiplier) / 100);
    }
}
