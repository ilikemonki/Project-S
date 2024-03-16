using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject skillsUI, statsUI, passiveItemsUI, playerEnemyStatsUI, shopUI;
    public GameObject menuUI, pauseUI, shopBtn;
    public InventoryManager inventoryManager;
    public ItemManager itemManager;
    public LevelUpManager levelUpManager;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseUI();
        }
    }
    public void OpenCloseUI() //Opens and close inventory
    {
        if (menuUI.activeSelf == false) //open
        {
            GameManager.PauseGame();
            menuUI.SetActive(true);
            SkillsUIButton();
            UpdateStats.FormatPlayerStatsToString();
            UpdateStats.FormatEnemyStatsToString();
            inventoryManager.UpdateGeneralStatistics();
            //Save data and set text when opening inventory.
            for (int i = 0; i < inventoryManager.activeSkillList.Count; i++) //Save exp and level of skill controller.
            {
                if (inventoryManager.activeSkillList[i].skillController != null)
                {
                    inventoryManager.activeSkillList[i].activeSkillDrop.nameText.text = "Lv. " + inventoryManager.activeSkillList[i].skillController.level.ToString() + " " + inventoryManager.activeSkillList[i].activeSkillDrop.draggableItem.itemDescription.itemName;
                    if (itemManager.skillExpDict.ContainsKey(inventoryManager.activeSkillList[i].skillController.skillOrbName))
                    {
                        itemManager.skillExpDict[inventoryManager.activeSkillList[i].skillController.skillOrbName] = inventoryManager.activeSkillList[i].skillController.exp;
                        itemManager.skillLevelDict[inventoryManager.activeSkillList[i].skillController.skillOrbName] = inventoryManager.activeSkillList[i].skillController.level;
                    }
                }
            }
            foreach (DraggableItem dItem in itemManager.skillOrbList.Keys) //Set level text
            {
                foreach (string orbName in itemManager.skillLevelDict.Keys)
                {
                    if (dItem.itemDescription.itemName.Equals(orbName))
                    {
                        dItem.slotUI.levelText.text = "Lv. " + itemManager.skillLevelDict[orbName].ToString();
                    }
                }
            }
        }
        else //close
        {
            if (levelUpManager.levelUpUI.activeSelf == false) //if level up menu is open, keep game paused.
            {
                GameManager.UnpauseGame();
            }
            ToolTipManager.HideToolTip();
            menuUI.SetActive(false);
        }
    }
    public void SkillsUIButton()
    {
        statsUI.SetActive(false);
        passiveItemsUI.SetActive(false);
        shopUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        skillsUI.SetActive(true);
    }
    public void StatsUIButton()
    {
        skillsUI.SetActive(false);
        passiveItemsUI.SetActive(false);
        shopUI.SetActive(false);
        playerEnemyStatsUI.SetActive(false);
        statsUI.SetActive(true);
    }
    public void PassiveItemsUIButton()
    {
        statsUI.SetActive(false);
        skillsUI.SetActive(false);
        shopUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        passiveItemsUI.SetActive(true);
    }
    public void ShopUIButton()
    {
        statsUI.SetActive(false);
        skillsUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        shopUI.SetActive(true);
        passiveItemsUI.SetActive(false);
    }
    public void InventoryButton()
    {
        shopBtn.SetActive(false);
        pauseUI.SetActive(false);
        OpenCloseUI();
    }
    public void RestartButton()
    {

    }
    public void MainMenuButton()
    {

    }
    public void QuitButton()
    {

    }
    public void SettingsButton()
    {

    }
}
