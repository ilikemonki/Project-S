using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Stores the items and skillControllers here.
public class ItemManager : MonoBehaviour
{
    public Dictionary<string, int> skillLevelDict = new(); //Saves skill level here to remember.
    public Dictionary<string, int> skillExpDict = new(); //Saves the exp of skills here.
    public Dictionary<DraggableItem, int> skillOrbList = new(); //skill orb inventory
    public Dictionary<DraggableItem, int> skillGemList = new(); //skill gem inventory
    public List<ItemDescription> pItemShopList = new(); //Passive Item shop inventory
    public List<ItemDescription> pItemInventoryList = new(); //Player's Passive Item inventory

    public List<SkillController> skillControllerPrefabsList = new();
    public List<DraggableItem> orbPrefabList = new();
    public List<DraggableItem> t1GemPrefabList = new(), t2GemPrefabList = new(), t3GemPrefabList = new();

    public Transform pItemParent;

    public void Start()
    {
        foreach (Transform child in pItemParent)
        {
            ItemDescription pItem = child.GetComponent<ItemDescription>();
            pItemShopList.Add(pItem);
        }
        pItemParent.gameObject.SetActive(false);
    }
}
