using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : SkillBehavior
{
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (currentSpeed * Time.fixedDeltaTime * direction));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillController.chainRange);
    }

}
