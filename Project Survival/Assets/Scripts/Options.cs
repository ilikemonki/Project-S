using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject skillsUI, statsUI, craftingUI;
    public GameObject menuUI;
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
        if (craftingUI.activeSelf) craftingUI.SetActive(false);
        skillsUI.SetActive(true);
    }
    public void StatsUIButton()
    {
        if (skillsUI.activeSelf) skillsUI.SetActive(false);
        if (craftingUI.activeSelf) craftingUI.SetActive(false);
        statsUI.SetActive(true);
    }
    public void CraftingUIButton()
    {
        if (statsUI.activeSelf) statsUI.SetActive(false);
        if (skillsUI.activeSelf) skillsUI.SetActive(false);
        craftingUI.SetActive(true);
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
