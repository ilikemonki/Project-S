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
    public int quantityDisabled; //number of quantity disabled.
    public void TogglePItem()
    {
        if (itemDescription.quantityInInventory == 1)
        {
            if (quantityDisabled == 0) //Disable pItem and unapply upgrades.
            {
                quantityDisabled++;
                imageDisable.gameObject.SetActive(true);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, true);
            }
            else //Enable pItem and apply upgrades.
            {
                quantityDisabled--;
                imageDisable.gameObject.SetActive(false);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, false);
            }
        }
        else //more than one pItem
        {
            if (quantityDisabled < itemDescription.quantityInInventory) //disable till you hit the max.
            {
                quantityDisabled++;
                imageDisable.fillAmount = (float)quantityDisabled / (float)itemDescription.quantityInInventory;
                imageDisable.gameObject.SetActive(true);
                UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, true);
            }
            else //maxed reached
            {
                quantityDisabled = 0;
                imageDisable.fillAmount = 0;
                imageDisable.gameObject.SetActive(false);
                for (int i = 0; i < itemDescription.quantityInInventory; i++)
                {
                    UpdateStats.ApplyGlobalUpgrades(itemDescription.upgrade, false);
                }
            }
        }
    }
}
