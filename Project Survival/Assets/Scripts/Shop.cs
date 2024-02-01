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
        public TextMeshProUGUI nameText, tagText, descriptionText, statText, statOnlyText, amountText;
        public Image itemImage;
    }
    public List<Item> itemList;
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
