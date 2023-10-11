using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Levels : MonoBehaviour
{
    [System.Serializable]
    public class LevelModifiers
    {
        public enum Modifier
        {
            damageMultiplier, projectileDamageMultiplier, meleeDamageMultiplier, physicalMultiplier, fireMultiplier, iceMultiplier, lightningMultiplier,
            strikeAdditive, projectileAdditive, pierceAdditive, chainAdditive,
            travelSpeedMultiplier,
            attackRangeMultiplier, projectileAttackRangeMultiplier, meleeAttackRangeMultiplier,
            travelRangeMultiplier,
            cooldownMultiplier, projectileCooldownMultiplier, meleeCooldownMultiplier,
            moveSpeedMultiplier,
            maxHealthMultiplier,
            defenseMultiplier,
            criticalChanceAdditive, projectileCriticalChanceAdditive, meleeCriticalChanceAdditive, criticalDamageAdditive, projectileCriticalDamageAdditive, meleeCriticalDamageAdditive,
            sizeAdditive, projectileSizeAdditive, meleeSizeAdditive,
            regenAdditive, degenAdditive, lifeStealChanceAdditive, lifeStealAdditive,
            magnetRangeMultiplier,
            bleedChanceAdditive, burnChanceAdditive, chillChanceAdditive, shockChanceAdditive,
            bleedEffectAdditive, burnEffectAdditive, chillEffectAdditive, shockEffectAdditive,
        }
        public List<Modifier> modifier;
        public List<float> amt;
    }
    public Sprite sprite;
    public int currentLevel;
    public string description;
    public bool destroy;
    public List<LevelModifiers> levelModifiersList = new();
}
