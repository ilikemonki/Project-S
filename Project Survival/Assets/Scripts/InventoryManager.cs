using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<SkillController> skillSlots = new();
    public List<PassiveItem> passiveItemSlots = new();

    public void AddSkill(SkillController skill)
    {
        skillSlots.Add(skill);
    }
    public void AddPassiveItem(PassiveItem pItem)
    {
        passiveItemSlots.Add(pItem);
    }
}
