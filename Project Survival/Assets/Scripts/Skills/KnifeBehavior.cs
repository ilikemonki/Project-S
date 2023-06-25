using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : SkillBehavior
{
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (speed * Time.fixedDeltaTime * direction));
    }

}
