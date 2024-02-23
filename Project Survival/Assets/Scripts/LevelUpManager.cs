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
    public List<Upgrades> maxedUpgradesList; // move upgrade to this list when it is maxed.
    public GameplayManager gameplayManager;
    int rand;
    public List<int> randUpgrades = new();
    public List<UpgradeUI> upgradeUIList;
    public Transform upgradeListParent;
    public void Start()
    {
        foreach(Transform child in upgradeListParent)
        {
            Upgrades upg = child.GetComponent<Upgrades>();
            upgradesList.Add(upg);
        }
        upgradeListParent.gameObject.SetActive(false);
    }
    public void SetUpgrades()
    {
        randUpgrades.Clear();
        if (upgradesList.Count >= upgradeUIList.Count)
        {
            for (int i = 0; i < 1000; i++) //Get random upgrades
            {
                rand = Random.Range(0, upgradesList.Count);
                if (!randUpgrades.Contains(rand))
                {
                    randUpgrades.Add(rand);
                    if (randUpgrades.Count == upgradeUIList.Count)
                    {
                        GameManager.DebugLog("Rolls for upgrades until " + upgradeUIList.Count + ": " + i.ToString());
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
        else if (upgradesList.Count < upgradeUIList.Count && upgradesList.Count > 0)
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
                SetUpgrades();
                levelUpUI.SetActive(true);
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
