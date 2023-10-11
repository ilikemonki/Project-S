using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpUI;
    public List<Levels> upgradeList;
    public GameplayManager gamePlayManager;
    [System.Serializable]
    public class UpgradeUIs
    {
        public Image spriteImage;
        public TextMeshProUGUI nameText, levelText, descriptionText, upgradeText;
    }
    public List<UpgradeUIs> upgradeUIs;
    public void UpgradeData()
    {

    }

    public void OpenUI()
    {
        if (levelUpUI.activeSelf == false)
        {
            GameManager.PauseGame();
            levelUpUI.SetActive(true);
        }
    }
    public void CloseUI()
    {
        if (levelUpUI.activeSelf == true)
        {
            GameManager.UnpauseGame();
            levelUpUI.SetActive(false);
        }
    }

}
