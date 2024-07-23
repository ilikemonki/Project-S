using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehavior : SkillBehavior
{
    public SkillBehavior secondarySkillBehavior; //second effect. Ex) Fireball explosion.
    public override void SetStats(float physical, float fire, float cold, float lightning, float travelSpeed, int pierce, int chain, float aoe)
    {
        this.enabled = true;
        base.SetStats(physical, fire, cold, lightning, travelSpeed, pierce, chain, aoe); 
        transform.localScale = new Vector3(skillController.prefabBehavior.transform.localScale.x, skillController.prefabBehavior.transform.localScale.y, 1);
        //set up secondary behavior
        secondarySkillBehavior.skillController = skillController;
        secondarySkillBehavior.SetStats(physical, fire, cold, lightning, travelSpeed, pierce, chain, aoe);
    }
    public override void ProjectileBehavior()
    {
        if (skillController.continuous || skillController.pierceAll || skillController.cannotChain || skillController.cannotPierce) return;
        if (pierce <= 0 && chain <= 0)
        {
            if (skillController.useReturn)
            {
                SetReturn();
            }
            else //Spawn explosion.
            {
                this.enabled = false;
                hitboxCollider.enabled = false;
                spriteRend.enabled = false;
                secondarySkillBehavior.gameObject.SetActive(true);
            }
            return;
        }
        if (pierce > 0)  //behavior for pierce
        {
            pierce--;
        }
        else if (chain > 0)   //behavior for chain
        {
            if (isOrbitSkill) //if projectile is an orbit object, despawn it and spawn in skill from poolList
            {
                target = FindTarget(true);
                if (target != null)
                {
                    skillController.SpawnChainProjectile(rememberEnemyList, target, transform);
                }
                gameObject.SetActive(false);
                return;
            }
            chain--;
            ChainToEnemy();
        }
    }

}
