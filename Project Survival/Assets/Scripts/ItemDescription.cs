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
}
