using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    public float multiplier;
    public PlayerStats player;

    protected virtual void ApplyModifier()
    {

    }

    protected virtual void UnApplyModifier()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        ApplyModifier();
    }
}
