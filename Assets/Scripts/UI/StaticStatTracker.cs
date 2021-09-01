using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StaticStatTracker : MonoBehaviour
{
    public static int playerHealth;
    public static float playerMaxHealth;

    public static int playerScore;

    public static int playerAmmoCount;
    public static int playerMaxAmmoCount;

    public static void UpdatePlayerHealth(int newHealth)
    {
        playerHealth = newHealth;
    }

    public static void UpdatePlayerMaxHealth(float maxHealth)
    {
        playerMaxHealth = maxHealth;
    }

    public static void UpdatePlayerScore(int addedScore)
    {
        playerScore += addedScore;
    }

    public static void UpdatePlayerAmmoCount(int newAmmoCount)
    {
        if (newAmmoCount < 0)
            newAmmoCount = 0;

        playerAmmoCount = newAmmoCount;
    }

    public static void UpdatePlayerMaxAmmoCount(int maxAmmoCount)
    {
        playerMaxAmmoCount = maxAmmoCount;
    }
}
