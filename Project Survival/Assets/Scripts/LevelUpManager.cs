using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public bool stopLevelUp;
    public GameObject levelUpUI;
    public List<Upgrades> upgradesList;
    public GameplayManager gamePlayManager;
    public int numOfAvailableUpgrades;
    int rand;
    public List<int> randUpgrades = new();
    public List<UpgradeUI> upgradeUIList;
    private void Awake()
    {
        numOfAvailableUpgrades = upgradesList.Count;
    }
    public void SetUpgrades()
    {
        if (numOfAvailableUpgrades >= 6)
        {
            for (int i = 0; i < 1000; i++) //Get random upgrades
            {
                rand = Random.Range(0, upgradesList.Count);
                if (!randUpgrades.Contains(rand))
                {
                    randUpgrades.Add(rand);
                    if (randUpgrades.Count == 6)
                    {
                        GameManager.DebugLog("Rolls for upgrades until 6:  " + i.ToString());
                        break;
                    }
                }
            }
            for (int i = 0; i < upgradeUIList.Count; i++)
            {
                upgradeUIList[i].upgrade = upgradesList[i];
                upgradeUIList[i].SetUI();
            }
        }
    }

    public void OpenUI()
    {
        if (levelUpUI.activeSelf == false)
        {
            GameManager.PauseGame();
            levelUpUI.SetActive(true);
            SetUpgrades();
        }
    }
    public void CloseUI()
    {
        if (levelUpUI.activeSelf == true)
        {
            GameManager.UnpauseGame();
            levelUpUI.SetActive(false);
            randUpgrades.Clear();
        }
    }

}
