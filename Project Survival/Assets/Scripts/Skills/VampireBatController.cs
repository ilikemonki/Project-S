using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBatController : SkillController
{
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!stopFiring && !useOrbit)
        {
            currentCooldown += Time.deltaTime;
            if (currentCooldown >= cooldown)
            {
                if (skillTrigger.isTriggerSkill == false)   //Use skill if it is not a skill trigger
                {
                    SummonBehavior();
                }
            }
        }
    }
    public void SummonBehavior()
    {

    }
}
