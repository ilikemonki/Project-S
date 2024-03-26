using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    public enum ItemType
    {
        PassiveItem,
        SkillOrb,
        SkillGem,
    }
    public ItemType itemType;
    public string itemName, itemTags, description;
    public Sprite itemSprite;
    public int maxQuantity, rarity;
    public int currentLevel; //Used for gem tiers as well
    public int price;
    public int quantityInInventory;
    public Upgrades upgrade;
    public PItemSlotUI pItemSlotUI; //only for passive item
    public bool shopLock;

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
}
