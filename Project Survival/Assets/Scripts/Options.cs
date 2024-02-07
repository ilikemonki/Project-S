using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject skillsUI, statsUI, passiveItemsUI, playerEnemyStatsUI, storeUI;
    public GameObject menuUI, pauseUI, storeBtn;
    public InventoryManager inventoryManager;
    public ItemManager itemManager;
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
            inventoryManager.UpdateGeneralStatistics();
            //Save data and set text when opening inventory.
            for (int i = 0; i < inventoryManager.activeSkillList.Count; i++) //Save exp and level of skill controller.
            {
                if (inventoryManager.activeSkillList[i].skillController != null)
                {
                    inventoryManager.activeSkillList[i].activeSkillDrop.nameText.text = "Lv. " + inventoryManager.activeSkillList[i].skillController.level.ToString() + " " + inventoryManager.activeSkillList[i].activeSkillDrop.draggableItem.itemName;
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
                    if (dItem.itemName.Equals(orbName))
                    {
                        dItem.slotUI.levelText.text = "Lv. " + itemManager.skillLevelDict[orbName].ToString();
                    }
                }
            }
        }
        else //close
        {
            GameManager.UnpauseGame();
            menuUI.SetActive(false);
        }
    }
    public void SkillsUIButton()
    {
        statsUI.SetActive(false);
        passiveItemsUI.SetActive(false);
        storeUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        skillsUI.SetActive(true);
    }
    public void StatsUIButton()
    {
        skillsUI.SetActive(false);
        passiveItemsUI.SetActive(false);
        storeUI.SetActive(false);
        playerEnemyStatsUI.SetActive(false);
        statsUI.SetActive(true);
    }
    public void PassiveItemsUIButton()
    {
        statsUI.SetActive(false);
        skillsUI.SetActive(false);
        storeUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        passiveItemsUI.SetActive(true);
    }
    public void StoreUIButton()
    {
        statsUI.SetActive(false);
        skillsUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        storeUI.SetActive(true);
    }
    public void InventoryButton()
    {
        storeBtn.SetActive(false);
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
