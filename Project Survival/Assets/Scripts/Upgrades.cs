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
            regen, degen, life_steal_chance, projectile_life_steal_chance, melee_life_steal_chance, life_steal, projectile_life_steal, melee_life_steal,
            bleed_chance, burn_chance, chill_chance, shock_chance,
            bleed_effect, burn_effect, chill_effect, shock_effect,
            knockback,
            //multiplier
            damage, projectile_damage, melee_damage, physical_damage, fire_damage, cold_damage, lightning_damage,
            attack_range, projectile_attack_range, melee_attack_range,
            travel_range, projectile_travel_range, melee_travel_range,
            travel_speed, projectile_travel_speed, melee_travel_speed,
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
    public ItemDescription itemDescription;
    public List<LevelModifiers> levelModifiersList = new();
    public bool equiped; //whether player wants the item equiped or not.
    public void Start()
    {
        //Check if gameobject has itemDescription script, then set it.
        if (itemDescription == null)
        {
            ItemDescription itemDesc = gameObject.GetComponent<ItemDescription>();
            if (itemDesc != null)
            {
                itemDescription = itemDesc;
            }
        }
    }
}
