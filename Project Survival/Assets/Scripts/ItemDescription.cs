using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    public string itemName, itemTags, description;
    public Sprite itemSprite;
    public int maxQuantity, rarity, currentLevel;
    public int price;
    public int quantityInInventory;
    public Upgrades upgrade;
    public PItemSlotUI pItemSlotUI; //only for passive item

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
