using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PItem1 : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.moveSpeed += multiplier;
        player.currentMoveSpeed = player.moveSpeed;
    }

    protected override void UnApplyModifier()
    {
        player.moveSpeed -= multiplier;
        player.currentMoveSpeed = player.moveSpeed;
    }
}
