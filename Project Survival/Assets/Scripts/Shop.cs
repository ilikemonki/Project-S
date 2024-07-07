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
        public ItemDescription itemDesc;
        public PassiveItemEffect pItemEffect;
        public TextMeshProUGUI nameText, tagText, behaviorText, descriptionText, priceText, quantityText;
        public Image itemImage;
        public Toggle lockToggle;
        public GameObject itemUIGameObject, newGameObject;
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
    public int freeReroll;
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
        if (freeReroll > 0)
        {
            freeReroll--;
            if (freeReroll > 0)
                rerollText.text = freeReroll + " Free Reroll";
            else
                rerollText.text = rerollPrice.ToString() + "\n Reroll";
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
        else if (rerollPrice <= gameplayManager.coins)
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
        freeReroll = gameplayManager.freeShopRerollAdditive;
        if (freeReroll > 0)
        {
            rerollText.text = freeReroll + " Free Reroll";
        }
        coinText.text = gameplayManager.coins.ToString();
        PopulatePItemShop();
        PopulateOrbShop();
        PopulateGemShop();
        SetUI();
    }
    public void PopulatePItemShop()
    {
        if (itemManager.availablePItemList.Count > itemUIList.Count) //is pItem has enough to populate
        {
            for (int j = 0; j < itemUIList.Count; j++)
            {
                if (pItemShopList[j] != null)
                {
                    if (!pItemShopList[j].shopLock)
                    {
                        for (int i = 0; i < 1000; i++) //Get random pItems
                        {
                            rand = Random.Range(0, itemManager.availablePItemList.Count);
                            if (!pItemShopList.Contains(itemManager.availablePItemList[rand]))
                            {
                                pItemShopList[j] = (itemManager.availablePItemList[rand]);
                                pItemShopList[j].shopLock = false;
                            }
                        }
                    }
                }
                else //if itemUI is empty, add item to shop.
                {
                    for (int i = 0; i < 1000; i++) //Get random pItems
                    {
                        rand = Random.Range(0, itemManager.availablePItemList.Count);
                        if (!pItemShopList.Contains(itemManager.availablePItemList[rand]))
                        {
                            pItemShopList[j] = itemManager.availablePItemList[rand];
                            pItemShopList[j].shopLock = false;
                        }
                    }
                }
            }
        }
        else if (itemManager.availablePItemList.Count <= itemUIList.Count) //if pItem dont have enough to populate
        {
            pItemShopList.Clear();
            for (int i = 0; i < itemManager.availablePItemList.Count; i++) //Get random items
            {
                if (!pItemShopList.Contains(itemManager.availablePItemList[i]))
                {
                    pItemShopList.Add(itemManager.availablePItemList[i]);
                    pItemShopList[i].shopLock = false;
                }
            }
        }
    }
    public void PopulateOrbShop()
    {
        for (int j = 0; j < itemUIList.Count; j++)
        {
            if (orbShopList[j] != null)
            {
                if (!orbShopList[j].shopLock)
                {
                    for (int i = 0; i < 1000; i++) //Get random orbs
                    {
                        rand = Random.Range(0, itemManager.orbPrefabList.Count);
                        ItemDescription itemDesc = itemManager.orbPrefabList[rand].itemDescription;
                        if (!orbShopList.Contains(itemDesc))
                        {
                            orbShopList[j] = itemDesc;
                            orbShopList[j].shopLock = false;
                        }
                    }
                }
            }
            else //if itemUI is empty, add item to shop.
            {
                for (int i = 0; i < 1000; i++) //Get random orbs
                {
                    rand = Random.Range(0, itemManager.orbPrefabList.Count);
                    ItemDescription itemDesc = itemManager.orbPrefabList[rand].itemDescription;
                    if (!orbShopList.Contains(itemDesc))
                    {
                        orbShopList[j] = itemDesc;
                        orbShopList[j].shopLock = false;
                    }
                }
            }
        }
    }
    public void PopulateGemShop()
    {
        for (int j = 0; j < itemUIList.Count; j++)
        {
            if (gemShopList[j] != null)
            {
                if (!gemShopList[j].shopLock)
                {
                    for (int i = 0; i < 1000; i++) //Get random gems
                    {
                        rand = Random.Range(0, itemManager.t1GemPrefabList.Count); //t1 gems
                        ItemDescription itemDesc = itemManager.t1GemPrefabList[rand].itemDescription;
                        if (!gemShopList.Contains(itemDesc))
                        {
                            gemShopList[j] = itemDesc;
                            gemShopList[j].shopLock = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 1000; i++) //Get random gems
                {
                    rand = Random.Range(0, itemManager.t1GemPrefabList.Count); //t1 gems
                    ItemDescription itemDesc = itemManager.t1GemPrefabList[rand].itemDescription;
                    if (!gemShopList.Contains(itemDesc))
                    {
                        gemShopList[j] = itemDesc;
                        gemShopList[j].shopLock = false;
                    }
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
                if (pItemShopList[i] != null)
                {
                    if (pItemShopList[i].shopLock)
                        itemUIList[i].lockToggle.isOn = true;
                    else
                        itemUIList[i].lockToggle.isOn = false;
                    itemUIList[i].lockToggle.interactable = true;
                }
                itemUIList[i].itemDesc.Clone(pItemShopList[i]); 
                itemUIList[i].itemImage.sprite = pItemShopList[i].itemSprite;
                itemUIList[i].nameText.text = pItemShopList[i].itemName;
                itemUIList[i].tagText.text = pItemShopList[i].itemTags;
                itemUIList[i].priceText.text = pItemShopList[i].price.ToString();
                itemUIList[i].quantityText.text = "Qty: " + pItemShopList[i].quantityInInventory.ToString() + "/" + pItemShopList[i].maxQuantity;
                if (pItemShopList[i].quantityInInventory == 0)
                {
                    itemUIList[i].newGameObject.SetActive(true);
                }
                if (!string.IsNullOrWhiteSpace(pItemShopList[i].description)) //if there is description.
                    itemUIList[i].descriptionText.text = pItemShopList[i].description;
                if (pItemShopList[i].pItemEffect != null) //if has passive effect
                {
                    itemUIList[i].pItemEffect.damage = pItemShopList[i].pItemEffect.damage;
                    itemUIList[i].pItemEffect.cooldown = pItemShopList[i].pItemEffect.cooldown;
                    itemUIList[i].pItemEffect.chance = pItemShopList[i].pItemEffect.chance;
                    itemUIList[i].pItemEffect.totalrecordedString = pItemShopList[i].pItemEffect.totalrecordedString;
                    if (itemUIList[i].pItemEffect.damage > 0)
                        itemUIList[i].descriptionText.text += "\n<color=orange>Damage: </color>" + itemUIList[i].pItemEffect.damage;
                    if (itemUIList[i].pItemEffect.chance > 0)
                        itemUIList[i].descriptionText.text += "\n<color=orange>Chance: </color>" + itemUIList[i].pItemEffect.chance + "%";
                    if (itemUIList[i].pItemEffect.cooldown > 0)
                        itemUIList[i].descriptionText.text += "\n<color=orange>Cooldown: </color>" + itemUIList[i].pItemEffect.cooldown + "s";
                    if (!string.IsNullOrWhiteSpace(itemUIList[i].pItemEffect.totalrecordedString))
                        itemUIList[i].descriptionText.text += "\n\n" + itemUIList[i].pItemEffect.totalrecordedString + " " + itemUIList[i].pItemEffect.totalRecorded.ToString();
                }
                if (pItemShopList[i].upgrade.levelModifiersList.Count > 0) //if there are modifiers
                    itemUIList[i].descriptionText.text += "\n\n" + UpdateStats.FormatItemUpgradeStatsToString(pItemShopList[i].upgrade.levelModifiersList[0]);

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
                if (orbShopList[i] != null)
                {
                    if (orbShopList[i].shopLock)
                        itemUIList[i].lockToggle.isOn = true;
                    else
                        itemUIList[i].lockToggle.isOn = false;
                    itemUIList[i].lockToggle.interactable = true;
                }
                itemUIList[i].itemDesc.Clone(orbShopList[i]);
                itemUIList[i].itemImage.sprite = orbShopList[i].itemSprite;
                itemUIList[i].nameText.text = orbShopList[i].itemName;
                itemUIList[i].tagText.text = orbShopList[i].itemTags;
                itemUIList[i].behaviorText.text = "Behavior: " + orbShopList[i].behavior;
                itemUIList[i].priceText.text = orbShopList[i].price.ToString(); 
                foreach (DraggableItem dItem in itemManager.skillOrbList.Keys) //if orb exist, show exp orb gained.
                {
                    if (dItem.itemDescription.itemName.Equals(orbShopList[i].itemName))
                    {
                        itemUIList[i].quantityText.text = "+" + gameplayManager.expOrbBonus.ToString() + " e" +
                            "xp to skill.";
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(itemUIList[i].quantityText.text))
                {
                    itemUIList[i].newGameObject.SetActive(true);
                }
                itemUIList[i].descriptionText.text = orbShopList[i].description + "\n\n";
                itemUIList[i].itemUIGameObject.SetActive(true);
            }
        }
        else if (isGemShop) //Set Gem Shop
        {
            for (int i = 0; i < gemShopList.Count; i++)
            {
                if (gemShopList[i] != null)
                {
                    if (gemShopList[i].shopLock)
                        itemUIList[i].lockToggle.isOn = true;
                    else
                        itemUIList[i].lockToggle.isOn = false;
                    itemUIList[i].lockToggle.interactable = true;
                }
                itemUIList[i].itemDesc.Clone(gemShopList[i]);
                itemUIList[i].itemImage.sprite = gemShopList[i].itemSprite;
                itemUIList[i].nameText.text = gemShopList[i].itemName;
                itemUIList[i].tagText.text = gemShopList[i].itemTags;
                itemUIList[i].priceText.text = gemShopList[i].price.ToString();
                foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
                {
                    if (dItem.itemDescription.itemName.Equals(gemShopList[i].itemName))
                    {
                        itemUIList[i].quantityText.text = "Qty: " + (itemManager.skillGemList[dItem]).ToString();
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(itemUIList[i].quantityText.text))
                {
                    itemUIList[i].quantityText.text = "Qty: 0";
                    itemUIList[i].newGameObject.SetActive(true);
                }
                if (string.IsNullOrWhiteSpace(gemShopList[i].description))
                    itemUIList[i].descriptionText.text = UpdateStats.FormatItemUpgradeStatsToString(gemShopList[i].upgrade.levelModifiersList[gemShopList[i].upgrade.itemDescription.currentLevel - 1]);
                else
                    itemUIList[i].descriptionText.text = gemShopList[i].description + "\n\n" + UpdateStats.FormatItemUpgradeStatsToString(gemShopList[i].upgrade.levelModifiersList[gemShopList[i].upgrade.itemDescription.currentLevel - 1]);
                itemUIList[i].itemUIGameObject.SetActive(true);
            }
        }
    }
    public void ClearUI() //Clear all shop ui
    {
        for (int i = 0; i < itemUIList.Count; i++)
        {
            itemUIList[i].newGameObject.SetActive(false);
            itemUIList[i].nameText.text = "";
            itemUIList[i].tagText.text = "";
            itemUIList[i].behaviorText.text = "";
            itemUIList[i].priceText.text = "";
            itemUIList[i].quantityText.text = "";
            itemUIList[i].descriptionText.text = "";
            itemUIList[i].pItemEffect.damage = 0;
            itemUIList[i].pItemEffect.cooldown = 0;
            itemUIList[i].pItemEffect.chance = 0;
            itemUIList[i].pItemEffect.totalrecordedString = "";
        }
    }

    public void BuyItem(ItemDescription item, ItemUI itemUI)
    {
        if (item.price <= gameplayManager.coins && itemUI.priceText.text != "Sold") //check if you can buy it.
        {
            gameplayManager.coins -= item.price;
            coinText.text = gameplayManager.coins.ToString();
            itemUI.newGameObject.SetActive(false);
            itemUI.priceText.text = "Sold";
            itemUI.lockToggle.interactable = false;
            itemUI.lockToggle.isOn = false;
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
                    if (item.pItemSlotUI.itemDescription.quantityDisabledInInventory > 0) //if item is disabled, adjust imageDisable fill amount
                    {
                        item.pItemSlotUI.imageDisable.fillAmount = (float)item.pItemSlotUI.itemDescription.quantityDisabledInInventory / (float)item.quantityInInventory;
                    }
                    UpdateStats.ApplyGlobalUpgrades(item.upgrade, false);
                }
                if (item.pItemEffect != null) //if has passive effect
                {
                    PItemEffectManager.AcquireItem(item.pItemEffect);
                }
                if (item.quantityInInventory >= item.maxQuantity) //Remove item from available shop item list
                {
                    itemManager.availablePItemList.Remove(item);
                }
                inventoryManager.UpdatePassiveItemsInventory(item); //Adds item to Passive Items Inventory UI or updates it.
                itemUI.quantityText.text = "Qty: " + item.quantityInInventory.ToString() + "/" + item.maxQuantity;
            }
            else //Send gem and orb to inventory
            {
                inventoryManager.AddCollectibleIntoInventory(item.itemName);
                if (isGemShop)
                {
                    foreach (DraggableItem dItem in itemManager.skillGemList.Keys)
                    {
                        if (dItem.itemDescription.itemName.Equals(item.itemName))
                        {
                            itemUI.quantityText.text = "Qty: " + (itemManager.skillGemList[dItem]).ToString();
                            break;
                        }
                    }
                }
                else
                    itemUI.quantityText.text = "+" + gameplayManager.expOrbBonus.ToString() + " exp to skill.";
            }
        }
        UpdateStats.FormatPlayerStatsToString();
    }
    public void BuyButton1()
    {
        if (isPItemShop) BuyItem(pItemShopList[0], itemUIList[0]);
        else if (isOrbShop) BuyItem(orbShopList[0], itemUIList[0]);
        else if (isGemShop) BuyItem(gemShopList[0], itemUIList[0]);
    }
    public void BuyButton2()
    {
        if (isPItemShop) BuyItem(pItemShopList[1], itemUIList[1]);
        else if (isOrbShop) BuyItem(orbShopList[1], itemUIList[1]);
        else if (isGemShop) BuyItem(gemShopList[1], itemUIList[1]);
    }
    public void BuyButton3()
    {
        if (isPItemShop) BuyItem(pItemShopList[2], itemUIList[2]);
        else if (isOrbShop) BuyItem(orbShopList[2], itemUIList[2]);
        else if (isGemShop) BuyItem(gemShopList[2], itemUIList[2]);
    }
    public void BuyButton4()
    {
        if (isPItemShop) BuyItem(pItemShopList[3], itemUIList[3]);
        else if (isOrbShop) BuyItem(orbShopList[3], itemUIList[3]);
        else if (isGemShop) BuyItem(gemShopList[3], itemUIList[3]);
    }
    public void Lock1()
    {
        if (isPItemShop)
        {
            if (pItemShopList[0].shopLock)
                pItemShopList[0].shopLock = false;
            else
                pItemShopList[0].shopLock = true;
        }
        else if (isOrbShop)
        {
            if (orbShopList[0].shopLock)
                orbShopList[0].shopLock = false;
            else
                orbShopList[0].shopLock = true;
        }
        else if (isGemShop)
        {
            if (gemShopList[0].shopLock)
                gemShopList[0].shopLock = false;
            else
                gemShopList[0].shopLock = true;
        }
    }
    public void Lock2()
    {
        if (isPItemShop)
        {
            if (pItemShopList[1].shopLock)
                pItemShopList[1].shopLock = false;
            else
                pItemShopList[1].shopLock = true;
        }
        else if (isOrbShop)
        {
            if (orbShopList[1].shopLock)
                orbShopList[1].shopLock = false;
            else
                orbShopList[1].shopLock = true;
        }
        else if (isGemShop)
        {
            if (gemShopList[1].shopLock)
                gemShopList[1].shopLock = false;
            else
                gemShopList[1].shopLock = true;
        }
    }
    public void Lock3()
    {
        if (isPItemShop)
        {
            if (pItemShopList[2].shopLock)
                pItemShopList[2].shopLock = false;
            else
                pItemShopList[2].shopLock = true;
        }
        else if (isOrbShop)
        {
            if (orbShopList[2].shopLock)
                orbShopList[2].shopLock = false;
            else
                orbShopList[2].shopLock = true;
        }
        else if (isGemShop)
        {
            if (gemShopList[2].shopLock)
                gemShopList[2].shopLock = false;
            else
                gemShopList[2].shopLock = true;
        }
    }
    public void Lock4()
    {
        if (isPItemShop)
        {
            if (pItemShopList[3].shopLock)
                pItemShopList[3].shopLock = false;
            else
                pItemShopList[3].shopLock = true;
        }
        else if (isOrbShop)
        {
            if (orbShopList[3].shopLock)
                orbShopList[3].shopLock = false;
            else
                orbShopList[3].shopLock = true;
        }
        else if (isGemShop)
        {
            if (gemShopList[3].shopLock)
                gemShopList[3].shopLock = false;
            else
                gemShopList[3].shopLock = true;
        }
    }
}
