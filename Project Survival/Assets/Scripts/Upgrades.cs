using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Upgrades : MonoBehaviour
{
    [System.Serializable]
    public class LevelModifiers
    {
        //Modifier name will be shown to ui text.
        public enum Modifier
        {
            //additive
            strike, projectile, pierce, chain,
            critical_chance, projectile_critical_chance, melee_critical_chance, critical_damage, projectile_critical_damage, melee_critical_damage,
            regen, degen, life_steal_chance, life_steal,
            bleed_chance, burn_chance, chill_chance, shock_chance,
            bleed_effect, burn_effect, chill_effect, shock_effect,
            //multiplier
            damage, projectile_damage, melee_damage, physical_damage, fire_damage, cold_damage, lightning_damage,
            travel_speed,
            attack_range, projectile_attack_range, melee_attack_range,
            travel_range,
            cooldown, projectile_cooldown, melee_cooldown,
            movement_speed,
            max_health,
            defense,
            magnet_range,
            size, projectile_size, melee_size,
        }
        public List<Modifier> modifier;
        public List<float> amt;
    }
    public Sprite sprite;
    public int currentLevel;
    public string description;
    public bool destroyed;
    public List<LevelModifiers> levelModifiersList = new();
}
