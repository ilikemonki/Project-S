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
    public List<Upgrades> maxedUpgradesList;
    public GameplayManager gamePlayManager;
    int rand;
    public List<int> randUpgrades = new();
    public List<UpgradeUI> upgradeUIList;
    public void SetUpgrades()
    {
        randUpgrades.Clear();
        if (upgradesList.Count >= 6)
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
                upgradeUIList[i].upgrade = upgradesList[randUpgrades[i]];
                upgradeUIList[i].SetUI();
            }
        }
        else if (upgradesList.Count < 6 && upgradesList.Count > 0)
        {
            for (int i = 0; i < 1000; i++) //Get random upgrades
            {
                rand = Random.Range(0, upgradesList.Count);
                if (!randUpgrades.Contains(rand))
                {
                    randUpgrades.Add(rand);
                    if (randUpgrades.Count == upgradesList.Count)
                    {
                        GameManager.DebugLog("Rolls for upgrades until " + upgradesList.Count + ": " + i.ToString());
                        break;
                    }
                }
            }
            for (int i = 0; i < upgradeUIList.Count; i++)
            {
                if (i < upgradesList.Count)
                {
                    upgradeUIList[i].upgrade = upgradesList[randUpgrades[i]];
                    upgradeUIList[i].SetUI();
                }
                else upgradeUIList[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenUI()
    {
        if (upgradesList.Count > 0)
        {
            if (levelUpUI.activeSelf == false)
            {
                GameManager.PauseGame();
                levelUpUI.SetActive(true);
                SetUpgrades();
            }
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
