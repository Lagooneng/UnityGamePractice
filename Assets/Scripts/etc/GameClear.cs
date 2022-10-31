using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    public static void SaveHiSCore()
    {
        SaveData.SaveHiScore(PlayerController.score);
    }
}
