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
        description = cloneFrom.description;
        maxQuantity = cloneFrom.maxQuantity;
        rarity = cloneFrom.rarity;
        currentLevel = cloneFrom.currentLevel;
        price = cloneFrom.price;
        quantityInInventory = cloneFrom.quantityInInventory;
        upgrade = cloneFrom.upgrade;
    }
}
