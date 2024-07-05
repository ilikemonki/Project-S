using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    public enum ItemType
    {
        PassiveItem,
        SkillOrb,
        SkillGem,
    }
        // Tags: Melee, Projectile, Physical, Fire, Cold, Lightning, Duration, Trigger, Buff, Debuff, Summon
    public ItemType itemType;
    public string itemName, itemTags, behavior, description;
    public Sprite itemSprite;
    public int maxQuantity, rarity;
    public int currentLevel; //Used for gem tiers as well
    public int price;
    public int quantityInInventory, quantityDisabledInInventory;
    public Upgrades upgrade;
    public PItemSlotUI pItemSlotUI; //only for passive item
    public PassiveItemEffect pItemEffect;
    public bool shopLock;

    public void Awake()
    {
        if (gameObject.GetComponent<Image>() != null)
            itemSprite = gameObject.GetComponent<Image>().sprite;
    }
    public void Start()
    {
        if (upgrade == null)
        {
            Upgrades upg = gameObject.GetComponent<Upgrades>();
            if (upg != null)
            {
                upgrade = upg;
            }
        }
    }
    public void Clone(ItemDescription cloneFrom)
    {
        itemType = cloneFrom.itemType;
        itemName = cloneFrom.itemName;
        itemTags = cloneFrom.itemTags;
        behavior = cloneFrom.behavior;
        description = cloneFrom.description;
        maxQuantity = cloneFrom.maxQuantity;
        rarity = cloneFrom.rarity;
        currentLevel = cloneFrom.currentLevel;
        price = cloneFrom.price;
        quantityInInventory = cloneFrom.quantityInInventory;
        upgrade = cloneFrom.upgrade;
    }
}
