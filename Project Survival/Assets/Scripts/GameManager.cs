using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int totalCoins;
    public static int totalKills, totalSkillsUsed, totalCrits, totalDashes, totalBleed, totalBurn, totalChill, totalShock, totalCoinsCollected, totalSkillGemsCollected;
    public static int totalExp, totalPassiveItems;
    public static float totalDamageDealt, totalPhysicalDamage, totalFireDamage, totalColdDamage, totalLightningDamage, totalBleedDamage, totalBurnDamage, TotalDotDamage;
    public static float totalDamageTaken, totalHealing, totalRegen, totalDegen, totalLifeStealProc, totalLifeSteal;
    public static float totalTime; //add Player Level and Wave to Statistics.
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
    }
    public static void DebugLog(string print)
    {
        Debug.Log(print);
    }
}
