using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Stores the items and skillControllers here.
public class ItemManager : MonoBehaviour
{
    public Dictionary<string, int> skillLevelDict = new(); //Saves skill level here to remember.
    public Dictionary<string, float> skillExpDict = new(); //Saves the exp of skills here.
    public Dictionary<DraggableItem, int> skillOrbList = new(); //skill orb inventory
    public Dictionary<DraggableItem, int> skillGemList = new(); //skill gem inventory. Holds the gem in inventory and the total amount.
    public List<ItemDescription> availablePItemList = new(); //Passive Item shop inventory. Item is removed from list when there is no more quantity left.
    public List<ItemDescription> pItemInventoryList = new(); //Player's Current Passive Item inventory

    public List<SkillController> skillControllerPrefabsList = new();
    public List<DraggableItem> orbPrefabList = new();
    public List<DraggableItem> t1GemPrefabList = new(), t2GemPrefabList = new(), t3GemPrefabList = new();

    public Transform pItemParent;
    public void Start()
    {
        foreach (Transform child in pItemParent)
        {
            ItemDescription pItem = child.GetComponent<ItemDescription>();
            availablePItemList.Add(pItem);
        }
    }
}
