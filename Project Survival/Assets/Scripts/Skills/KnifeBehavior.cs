using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : SkillBehavior
{

    //protected override void Update()
    //{
    //    base.Update();
    //    transform.position += skillController.speed * Time.deltaTime * direction;
    //}
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (skillController.speed * Time.fixedDeltaTime * direction));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillController.chainRange);
    }

}
