using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGem : MonoBehaviour
{
    [System.Serializable]
    public class GemModifier
    {
        public enum Modifier
        {
            Damage,
            Projectile,
        }
        public Modifier modifier;
        public float amt;
    }
    public List<GemModifier> gemModifierList = new();
}
