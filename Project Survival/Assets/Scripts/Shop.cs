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
    public UpdateStats updateStats;
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
    public float rerollPrice;
    public Button pItemButton, skillOrbButton, skillGemButton;
    public TextMeshProUGUI rerollText, coinText;
    int rand;
    public List<ItemDescription> pItemShopList; //Current pItem shop items
    public List<ItemDescription> orbShopList; //Current orb shop items
    public List<ItemDescription> gemShopList; //Current gem shop items
    public bool isPItemShop, isOrbShop, isGemShop;
    public void Start()
    {
        isPItemShop = true;
    }
    public void PassiveItemsButton()
    {
        isPItemShop = true; isOrbShop = false; isGemShop = false;
    }
    public void SkillOrbsButton()
    {
        isPItemShop = false; isOrbShop = true; isGemShop = false;
    }
    public void SkillGemsButton()
    {

        isPItemShop = false; isOrbShop = false; isGemShop = true;
    }
    public void RerollButton()
    {

    }
    public void PopulateShop()
    {
        coinText.text = gameplayManager.coins.ToString();
        pItemShopList.Clear();
        if(itemManager.availablePItemList.Count >= itemUIList.Count)
        {
            for (int i = 0; i < 1000; i++) //Get random items
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
            SetUI();
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
            SetUI();
        }
    }
    public void SetUI()
    {
        ClearUI();
        for (int i = 0; i < pItemShopList.Count; i++)
        {
            itemUIList[i].itemImage.sprite = pItemShopList[i].itemSprite;
            itemUIList[i].nameText.text = pItemShopList[i].itemName;
            itemUIList[i].tagText.text = pItemShopList[i].itemTags;
            itemUIList[i].priceText.text = pItemShopList[i].price.ToString();
            itemUIList[i].quantityText.text = pItemShopList[i].quantityInInventory.ToString() + "/" + pItemShopList[i].maxQuantity;
            if (string.IsNullOrWhiteSpace(pItemShopList[i].description)) //if there is no description. set statOnlyText.text
            {
                itemUIList[i].statOnlyText.text = updateStats.FormatStatsToString(pItemShopList[i].upgrade.levelModifiersList[pItemShopList[i].upgrade.itemDescription.currentLevel]);
            }
            else
            {
                itemUIList[i].descriptionText.text = pItemShopList[i].description;
                itemUIList[i].statText.text = updateStats.FormatStatsToString(pItemShopList[i].upgrade.levelModifiersList[pItemShopList[i].upgrade.itemDescription.currentLevel]);
            }
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
        if(isPItemShop)
            BuyItem(pItemShopList[0], itemUIList[0]);
    }
    public void BuyButton2()
    {

        if (isPItemShop)
            BuyItem(pItemShopList[1], itemUIList[1]);
    }
    public void BuyButton3()
    {

        if (isPItemShop)
            BuyItem(pItemShopList[2], itemUIList[2]);
    }
    public void BuyButton4()
    {

        if (isPItemShop)
            BuyItem(pItemShopList[3], itemUIList[3]);
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
                    item.upgrade.equiped = true;
                    updateStats.ApplyGlobalUpgrades(item.upgrade); //apply the pItem upgrade
                }
                else //if item is already in inventory, apply the upgrades  if it is still equiped.
                {
                    if (item.upgrade.equiped == true)
                    {
                        updateStats.ApplyGlobalUpgrades(item.upgrade);
                    }
                }
                if (item.quantityInInventory >= item.maxQuantity) //Remove item from available item list
                {
                    itemManager.availablePItemList.Remove(item);
                }
                inventoryManager.UpdatePassiveItemsInventory(item); //Adds item to Passive Items Inventory UI or updates it.
            }
        }
    }
}
