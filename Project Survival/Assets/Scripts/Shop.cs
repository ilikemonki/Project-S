using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public ItemManager itemManager;
    public GameplayManager gameplayManager;
    public UpdateStats updateStats;
    [System.Serializable]
    public class ItemUI
    {
        public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText, priceText, quantityText;
        public Image itemImage;
        public Button buyButton;
        public Toggle lockToggle;
    }
    public List<ItemUI> itemUIList;
    public float rerollPrice;
    public Button pItemButton, skillOrbButton, skillGemButton;
    public TextMeshProUGUI rerollText, coinText;
    int rand;
    public List<ItemDescription> itemShopList; //Current shop items
    public void PassiveItemsButton()
    {

    }
    public void SkillOrbsButton()
    {

    }
    public void SkillGemsButton()
    {

    }
    public void RerollButton()
    {

    }
    public void PopulateShop()
    {
        coinText.text = gameplayManager.coins.ToString();
        itemShopList.Clear();
        if(itemManager.pItemShopList.Count >= itemUIList.Count)
        {
            for (int i = 0; i < 1000; i++) //Get random items
            {
                rand = Random.Range(0, itemManager.pItemShopList.Count);
                if (!itemShopList.Contains(itemManager.pItemShopList[rand]))
                {
                    itemShopList.Add(itemManager.pItemShopList[rand]);
                    if (itemShopList.Count == itemUIList.Count)
                    {
                        GameManager.DebugLog("# of rolls to populate shop items " + itemUIList.Count + ": " + i.ToString());
                        break;
                    }
                }
            }
            SetUI();
        }
    }
    public void SetUI()
    {
        ClearUI();
        for (int i = 0; i < itemShopList.Count; i++)
        {
            itemUIList[i].itemImage.sprite = itemShopList[i].itemSprite;
            itemUIList[i].nameText.text = itemShopList[i].itemName;
            itemUIList[i].tagText.text = itemShopList[i].itemTags;
            itemUIList[i].priceText.text = itemShopList[i].price.ToString();
            itemUIList[i].quantityText.text = itemShopList[i].quantityInInventory.ToString() + "/" + itemShopList[i].maxQuantity;
            if (string.IsNullOrWhiteSpace(itemShopList[i].description)) //if there is no description. set statOnlyText.text
            {
                itemUIList[i].statOnlyText.text = updateStats.FormatStatsToString(itemShopList[i].upgrade.levelModifiersList[itemShopList[i].upgrade.itemDescription.currentLevel]);
            }
            else
            {
                itemUIList[i].descriptionText.text = itemShopList[i].description;
                itemUIList[i].statText.text = updateStats.FormatStatsToString(itemShopList[i].upgrade.levelModifiersList[itemShopList[i].upgrade.itemDescription.currentLevel]);
            }
        }
    }
    public void ClearUI() //Clear all shop ui
    {
        for (int i = 0; i < itemShopList.Count; i++)
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
        if (itemShopList[0].price <= gameplayManager.coins && itemUIList[0].priceText.text != "Sold") //check if you can buy it.
        {
            UpdateTransaction(itemShopList[0].price);
            itemUIList[0].priceText.text = "Sold";
        }
    }
    public void BuyButton2()
    {

    }
    public void BuyButton3()
    {

    }
    public void BuyButton4()
    {

    }
    public void UpdateTransaction(int price)
    {
        gameplayManager.coins -= price;
        coinText.text = gameplayManager.coins.ToString();
    }
}
