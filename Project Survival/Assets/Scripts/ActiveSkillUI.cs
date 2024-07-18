using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static InventoryManager;

public class ActiveSkillUI : MonoBehaviour
{
    public SkillController skillController;
    public Image skillImage;
    public Image fillImage;

    public void Update()
    {
        UpdateCooldown();
    }
    public void UpdateCooldown()
    {
        if (skillController != null)
        {
            if (skillController.currentCooldown <= 0)
            {
                fillImage.fillAmount = 0;
            }
            else
            {
                fillImage.fillAmount = skillController.currentCooldown / skillController.cooldown;
            }
        }
    }
    public void SetSkill(SkillController skill)
    {
        skillController = skill;
        skillImage.sprite = skill.skillOrbDescription.itemSprite;
        skillImage.enabled = true;
        fillImage.fillAmount = 0;
    }
    public void RemoveSkill()
    {
        skillImage.enabled = false;
        skillController = null;
        skillImage.sprite = null;
        fillImage.fillAmount = 0;
    }
}
