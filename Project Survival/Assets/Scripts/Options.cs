using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject skillsUI, statsUI, passiveItemsUI, playerEnemyStatsUI;
    public GameObject menuUI;
    public InventoryManager inventoryManager;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseUI();
        }
    }
    public void OpenCloseUI()
    {
        if (menuUI.activeSelf == false)
        {
            GameManager.PauseGame();
            menuUI.SetActive(true);
            SkillsUIButton();
            inventoryManager.UpdateGeneralStatistics();
            for (int i = 0; i < inventoryManager.activeSkillList.Count; i++) //Save exp and level of skill controller.
            {
                if (inventoryManager.activeSkillList[i].skillController != null)
                {
                    if (inventoryManager.gameplayManager.skillExpDict.ContainsKey(inventoryManager.activeSkillList[i].skillController.skillOrbName))
                    {
                        inventoryManager.gameplayManager.skillExpDict[inventoryManager.activeSkillList[i].skillController.skillOrbName] = inventoryManager.activeSkillList[i].skillController.exp;
                        inventoryManager.gameplayManager.skillLevelDict[inventoryManager.activeSkillList[i].skillController.skillOrbName] = inventoryManager.activeSkillList[i].skillController.level;
                        GameManager.DebugLog("Saved exp/lv for: " + inventoryManager.activeSkillList[i].skillController.skillOrbName);
                    }
                }
            }
        }
        else
        {
            GameManager.UnpauseGame();
            menuUI.SetActive(false);
        }
    }
    public void SkillsUIButton()
    {
        if (statsUI.activeSelf) statsUI.SetActive(false);
        if (passiveItemsUI.activeSelf) passiveItemsUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        skillsUI.SetActive(true);
    }
    public void StatsUIButton()
    {
        if (skillsUI.activeSelf) skillsUI.SetActive(false);
        if (passiveItemsUI.activeSelf) passiveItemsUI.SetActive(false);
        playerEnemyStatsUI.SetActive(false);
        statsUI.SetActive(true);
    }
    public void PassiveItemsUIButton()
    {
        if (statsUI.activeSelf) statsUI.SetActive(false);
        if (skillsUI.activeSelf) skillsUI.SetActive(false);
        playerEnemyStatsUI.SetActive(true);
        passiveItemsUI.SetActive(true);
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
