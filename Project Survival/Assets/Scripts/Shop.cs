using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public ItemManager itemManager;
    public GameplayManager gameplayManager;
    public InventoryManager inventoryManager;
    [System.Serializable]
    public class ItemUI
    {
        public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText, priceText, quantityText;
        public Image itemImage;
        public Button buyButton;
        public Toggle lockToggle;
        public GameObject itemUIGameObject;
    }
    public List<ItemUI> itemUIList;
    public float rerollPrice, rerollIncrement; //Reroll price keeps incrementing and doesn't reset. Used for all shops.
    public Button pItemButton, skillOrbButton, skillGemButton;
    public TextMeshProUGUI rerollText, coinText;
    int rand;
    public List<ItemDescription> pItemShopList; //Current pItem shop items
    public List<ItemDescription> orbShopList; //Current orb shop items
    public List<ItemDescription> gemShopList; //Current gem shop items
    public bool isPItemShop, isOrbShop, isGemShop;
    public void Start()
    {
        rerollText.text = rerollPrice.ToString() + "\n Reroll";
        isPItemShop = true;
        PopulateAllShop();
    }
    public void PassiveItemsButton()
    {
        isPItemShop = true; isOrbShop = false; isGemShop = false;
        SetUI();
    }
    public void SkillOrbsButton()
    {
        isPItemShop = false; isOrbShop = true; isGemShop = false;
        SetUI();
    }
    public void SkillGemsButton()
    {
        isPItemShop = false; isOrbShop = false; isGemShop = true;
        SetUI();
    }
    public void RerollButton()
    {
        if (rerollPrice <= gameplayManager.coins)
        {
            gameplayManager.coins -= (int)rerollPrice;
            rerollPrice += rerollIncrement;
            rerollText.text = rerollPrice.ToString() + "\n Reroll";
            coinText.text = gameplayManager.coins.ToString();
            if (isPItemShop)
            {
                PopulatePItemShop();
                SetUI();
            }
            else if (isOrbShop)
            {
                PopulateOrbShop();
                SetUI();
            }
            else if (isGemShop)
            {
                PopulateGemShop();
                SetUI();
            }
        }
    }
    public void PopulateAllShop()
    {
        coinText.text = gameplayManager.coins.ToString();
        PopulatePItemShop();
        PopulateOrbShop();
        PopulateGemShop();
        SetUI();
    }
    public void PopulatePItemShop()
    {
        pItemShopList.Clear();
        if (itemManager.availablePItemList.Count >= itemUIList.Count)
        {
            for (int i = 0; i < 1000; i++) //Get random pItems
            {
                rand = Random.Range(0, itemManager.availablePItemList.Count);
                if (!pItemShopList.Contains(itemManager.availablePItemList[rand]))
                {
                    pItemShopList.Add(itemManager.availablePItemList[rand]);
                    if (pItemShopList.Count == itemUIList.Count)
                    {
                        break;
                    }
                }
            }
        }
        else if (itemManager.availablePItemList.Count < itemUIList.Count)
        {
            for (int i = 0; i < itemManager.availablePItemList.Count; i++) //Get random items
            {
                if (!pItemShopList.Contains(itemManager.availablePItemList[i]))
                {
                    pItemShopList.Add(itemManager.availablePItemList[i]);
                }
            }
        }
    }
    public void PopulateOrbShop()
    {
        orbShopList.Clear();
        for (int i = 0; i < 1000; i++) //Get random orbs
        {
            rand = Random.Range(0, itemManager.orbPrefabList.Count);
            ItemDescription itemDesc = itemManager.orbPrefabList[rand].itemDescription;
            if (!orbShopList.Contains(itemDesc))
            {
                orbShopList.Add(itemDesc);
                if (orbShopList.Count == itemUIList.Count)
                {
                    break;
                }
            }
        }
    }
    public void PopulateGemShop()
    {
        gemShopList.Clear();
        for (int i = 0; i < 1000; i++) //Get random gems
        {
            rand = Random.Range(0, itemManager.t1GemPrefabList.Count); //t1 gems
            ItemDescription itemDesc = itemManager.t1GemPrefabList[rand].itemDescription;
            if (!gemShopList.Contains(itemDesc))
            {
                gemShopList.Add(itemDesc);
                if (gemShopList.Count == itemUIList.Count)
                {
                    break;
                }
            }
        }
    }
    public void SetUI() //Show ui of which shop is chosen.
    {
        ClearUI();
        if (isPItemShop) //Set pItem Shop
        {
            for (int i = 0; i < pItemShopList.Count; i++)
            {
                itemUIList[i].itemImage.sprite = pItemShopList[i].itemSprite;
                itemUIList[i].nameText.text = pItemShopList[i].itemName;
                itemUIList[i].tagText.text = pItemShopList[i].itemTags;
                itemUIList[i].priceText.text = pItemShopList[i].price.ToString();
                itemUIList[i].quantityText.text = pItemShopList[i].quantityInInventory.ToString() + "/" + pItemShopList[i].maxQuantity;
                if (string.IsNullOrWhiteSpace(pItemShopList[i].description)) //if there is no description. set statOnlyText.text
                {
                    itemUIList[i].statOnlyText.text = UpdateStats.FormatItemUpgradeStatsToString(pItemShopList[i].upgrade.levelModifiersList[pItemShopList[i].upgrade.itemDescription.currentLevel]);
                }
                else
                {
                    itemUIList[i].descriptionText.text = pItemShopList[i].description;
                    itemUIList[i].statText.text = UpdateStats.FormatItemUpgradeStatsToString(pItemShopList[i].upgrade.levelModifiersList[pItemShopList[i].upgrade.itemDescription.currentLevel]);
                }
                itemUIList[i].itemUIGameObject.SetActive(true);
            }
            if (pItemShopList.Count < itemUIList.Count) //deactivate other itemUI's that is not used.
            {
                for (int i = pItemShopList.Count; i < itemUIList.Count; i++)
                {
                    if (itemUIList[i].nameText.text == "" && itemUIList[i].itemUIGameObject.activeSelf == true)
                    {
                        itemUIList[i].itemUIGameObject.SetActive(false);
                    }
                }
            }
        }
        else if (isOrbShop) //Set Orb Shop
        {
            for (int i = 0; i < orbShopList.Count; i++)
            {
                itemUIList[i].itemImage.sprite = orbShopList[i].itemSprite;
                itemUIList[i].nameText.text = orbShopList[i].itemName;
                itemUIList[i].tagText.text = orbShopList[i].itemTags;
                itemUIList[i].priceText.text = orbShopList[i].price.ToString(); 
                foreach (DraggableItem dItem in itemManager.skillOrbList.Keys) //if orb exist, show exp orb gained.
                {
                    if (dItem.itemDescription.itemName.Equals(orbShopList[i].itemName))
                    {
                        itemUIList[i].quantityText.text = "+" + gameplayManager.expOrbBonus.ToString() + " Exp";
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(itemUIList[i].quantityText.text))
                {
                    itemUIList[i].quantityText.text = "0";
                }
                itemUIList[i].descriptionText.text = orbShopList[i].description;
                itemUIList[i].statText.text = "Stats Here";
                itemUIList[i].itemUIGameObject.SetActive(true);
            }
        }
        else if (isGemShop) //Set Gem Shop
        {
            for (int i = 0; i < gemShopList.Count; i++)
            {
                itemUIList[i].itemImage.sprite = gemShopList[i].itemSprite;
                itemUIList[i].nameText.text = gemShopList[i].itemName;
                itemUIList[i].tagText.text = gemShopList[i].itemTags;
                itemUIList[i].priceText.text = gemShopList[i].price.ToString();
                foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
                {
                    if (dItem.itemDescription.itemName.Equals(gemShopList[i].itemName))
                    {
                        itemUIList[i].quantityText.text = (itemManager.skillGemList[dItem]).ToString();
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(itemUIList[i].quantityText.text))
                {
                    itemUIList[i].quantityText.text = "0";
                }
                if (string.IsNullOrWhiteSpace(gemShopList[i].description))
                {
                    itemUIList[i].statOnlyText.text = UpdateStats.FormatItemUpgradeStatsToString(gemShopList[i].upgrade.levelModifiersList[gemShopList[i].upgrade.itemDescription.currentLevel]);
                }
                else
                {
                    itemUIList[i].descriptionText.text = gemShopList[i].description;
                    itemUIList[i].statText.text = UpdateStats.FormatItemUpgradeStatsToString(gemShopList[i].upgrade.levelModifiersList[gemShopList[i].upgrade.itemDescription.currentLevel]);
                }
                itemUIList[i].itemUIGameObject.SetActive(true);
            }
        }
    }
    public void ClearUI() //Clear all shop ui
    {
        for (int i = 0; i < itemUIList.Count; i++)
        {
            itemUIList[i].nameText.text = "";
            itemUIList[i].tagText.text = "";
            itemUIList[i].priceText.text = "";
            itemUIList[i].quantityText.text = "";
            itemUIList[i].statOnlyText.text = "";
            itemUIList[i].descriptionText.text = "";
            itemUIList[i].statText.text = "";
        }
    }
    public void BuyButton1()
    {
        if (isPItemShop) BuyItem(pItemShopList[0], itemUIList[0]);
        if (isOrbShop) BuyItem(orbShopList[0], itemUIList[0]);
        if (isGemShop) BuyItem(gemShopList[0], itemUIList[0]);
    }
    public void BuyButton2()
    {
        if (isPItemShop) BuyItem(pItemShopList[1], itemUIList[1]);
        if (isOrbShop) BuyItem(orbShopList[1], itemUIList[1]);
        if (isGemShop) BuyItem(gemShopList[1], itemUIList[1]);
    }
    public void BuyButton3()
    {
        if (isPItemShop) BuyItem(pItemShopList[2], itemUIList[2]);
        if (isOrbShop) BuyItem(orbShopList[2], itemUIList[2]);
        if (isGemShop) BuyItem(gemShopList[2], itemUIList[2]);
    }
    public void BuyButton4()
    {
        if (isPItemShop) BuyItem(pItemShopList[3], itemUIList[3]);
        if (isOrbShop) BuyItem(orbShopList[3], itemUIList[3]);
        if (isGemShop) BuyItem(gemShopList[3], itemUIList[3]);
    }
    public void BuyItem(ItemDescription item, ItemUI itemUI)
    {
        if (item.price <= gameplayManager.coins && itemUI.priceText.text != "Sold") //check if you can buy it.
        {
            gameplayManager.coins -= item.price;
            coinText.text = gameplayManager.coins.ToString();
            itemUI.priceText.text = "Sold";
            if (isPItemShop)
            {
                item.quantityInInventory++;
                if (!itemManager.pItemInventoryList.Contains(item)) //add item to inventory list
                {
                    itemManager.pItemInventoryList.Add(item);
                    UpdateStats.ApplyGlobalUpgrades(item.upgrade, false); //apply the pItem upgrade
                }
                else //if item is already in inventory, apply the upgrades.
                {
                    if (item.pItemSlotUI.quantityDisabled > 0) //if item is disabled, adjust imageDisable fill amount
                    {
                        item.pItemSlotUI.imageDisable.fillAmount = (float)item.pItemSlotUI.quantityDisabled / (float)item.quantityInInventory;
                    }
                    UpdateStats.ApplyGlobalUpgrades(item.upgrade, false);
                }
                if (item.quantityInInventory >= item.maxQuantity) //Remove item from available shop item list
                {
                    itemManager.availablePItemList.Remove(item);
                }
                inventoryManager.UpdatePassiveItemsInventory(item); //Adds item to Passive Items Inventory UI or updates it.
            }
            else if (isOrbShop || isGemShop) //Send gem and orb to inventory
            {
                inventoryManager.AddCollectibleIntoInventory(item.itemName);
            }
        }
        UpdateStats.FormatPlayerStatsToString();
    }
}
