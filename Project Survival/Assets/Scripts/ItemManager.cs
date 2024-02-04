using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Dictionary<string, int> skillLevelDict = new(); //Saves skill level here to remember.
    public Dictionary<string, int> skillExpDict = new(); //Saves the exp of skills here.
    public Dictionary<DraggableItem, int> skillOrbList = new(); //skill orb inventory
    public Dictionary<DraggableItem, int> skillGemList = new(); //skill gem inventory

    public List<SkillController> skillControllerPrefabsList = new();
    public List<DraggableItem> orbPrefabList;
    public List<DraggableItem> t1GemPrefabList, t2GemPrefabList, t3GemPrefabList;

}
