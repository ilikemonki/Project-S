using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PItemSlotUI : MonoBehaviour
{
    public ItemDescription itemDescription;
    public Image image, imageDisable;
    public TextMeshProUGUI quanityText;
    public void TogglePItem()
    {
        if (itemDescription.quantityInInventory == 1)
        {
            if (itemDescription.quantityDisabledInInventory == 0) //Disable pItem and unapply upgrades.
            {
                itemDescription.quantityDisabledInInventory++;
                imageDisable.gameObject.SetActive(true);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, true);
                if (itemDescription.pItemEffect != null)
                {
                    itemDescription.pItemEffect.RemoveEffect();
                    itemDescription.pItemEffect.checkCondition = false;
                }
            }
            else //Enable pItem and apply upgrades.
            {
                itemDescription.quantityDisabledInInventory--;
                imageDisable.gameObject.SetActive(false);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, false);
                if (itemDescription.pItemEffect != null)
                {
                    itemDescription.pItemEffect.checkCondition = true;
                    itemDescription.pItemEffect.CheckCondition();
                }
            }
        }
        else //more than one pItem
        {
            if (itemDescription.quantityDisabledInInventory < itemDescription.quantityInInventory) //disable till you hit the max.
            {
                itemDescription.quantityDisabledInInventory++;
                imageDisable.fillAmount = (float)itemDescription.quantityDisabledInInventory / (float)itemDescription.quantityInInventory;
                imageDisable.gameObject.SetActive(true);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, true);
                if (itemDescription.pItemEffect != null)
                {
                    itemDescription.pItemEffect.RemoveEffect();
                    itemDescription.pItemEffect.checkCondition = false;
                }
            }
            else //maxed reached, reset and apply all upgrades.
            {
                itemDescription.quantityDisabledInInventory = 0;
                imageDisable.fillAmount = 0;
                imageDisable.gameObject.SetActive(false);
                for (int i = 0; i < itemDescription.quantityInInventory; i++)
                {
                    UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, false);
                }
                if (itemDescription.pItemEffect != null)
                {
                    itemDescription.pItemEffect.checkCondition = true;
                    itemDescription.pItemEffect.CheckCondition();
                }
            }
        }
    }
}
