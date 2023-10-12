using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpUI;
    public List<Levels> upgradesList;
    public GameplayManager gamePlayManager;
    public int numOfAvailableUpgrades;
    int rand;
    public List<int> randUpgrades = new();
    [System.Serializable]
    public class UpgradeUIs
    {
        public Image image;
        public TextMeshProUGUI nameText, levelText, descriptionText, upgradeText;
    }
    public List<UpgradeUIs> upgradeUIs;
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
                        GameManager.DebugLog("Random # of rolls for upgrades: " + i.ToString());
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < randUpgrades.Count; i++) //Set upgrades
        {
            upgradeUIs[i].image.sprite = upgradesList[randUpgrades[i]].sprite;
            upgradeUIs[i].nameText.text = upgradesList[randUpgrades[i]].name;
            upgradeUIs[i].levelText.text = upgradesList[randUpgrades[i]].currentLevel.ToString() + "/" + upgradesList[randUpgrades[i]].levelModifiersList.Count;
            upgradeUIs[i].descriptionText.text = upgradesList[randUpgrades[i]].description;
            upgradeUIs[i].upgradeText.text = "";
            for (int j = 0; j < upgradesList[randUpgrades[i]].levelModifiersList[upgradesList[randUpgrades[i]].currentLevel].modifier.Count; j++)
            {
                upgradeUIs[i].upgradeText.text = upgradeUIs[i].upgradeText.text + "" +
                    "+" + upgradesList[randUpgrades[i]].levelModifiersList[upgradesList[randUpgrades[i]].currentLevel].amt[j] + " " + upgradesList[randUpgrades[i]].levelModifiersList[upgradesList[randUpgrades[i]].currentLevel].modifier[j];
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
