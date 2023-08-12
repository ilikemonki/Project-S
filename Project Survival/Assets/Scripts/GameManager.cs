using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int totalCoins, totalClassStars;
    public static int totalKills, totalSkillsUsed, totalCrits, totalDashes, totalBleed, totalBurn, totalChill, totalShock;
    public static float totalDamageDealt, totalDamageTaken, totalHealing, totalBleedDamage, totalBurnDamage, totalTime;
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
