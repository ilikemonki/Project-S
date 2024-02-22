using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText, priceText, quantityText;
        public Image itemImage;
        public Button buyButton;
        public Toggle lockToggle;
    }
    public List<Item> itemList;
    public float rerollPrice;
    public Button pItemButton, skillOrbButton, skillGemButton;
    public TextMeshProUGUI rerollText;
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
}
