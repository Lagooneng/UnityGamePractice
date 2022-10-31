using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SaveData
{
    // Info
    const float SaveDataVersion = 0.30f;

    // 외부 파라미터
    public static string SaveDate = "(non)";

    // HiScore
    static int[] HiScoreInitData = new int[10]
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };
    public static int[] HiScore = new int[10]
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    // Option
    public static float SoundBGMVolume = 1.0f;
    public static float SoundSEVolume = 1.0f;
    public static bool VRPadEnabled = false;

    // Etc(Don't Save)
    public static bool continuePlay = false;
    public static int newRecord = -1;
    public static bool debug_Invicible = false;

    // 코드: 저장 데이터 검사
    static void SaveDataHeader(string dataGroupName)
    {
        PlayerPrefs.SetFloat("SaveDataVersion", SaveDataVersion);
        SaveDate = System.DateTime.Now.ToString("G");
        PlayerPrefs.SetString("SaveDataDate", SaveDate);
        PlayerPrefs.SetString(dataGroupName, "on");
    }

    static bool CheckSaveDataHeader(string dataGroupName)
    {
        if( !PlayerPrefs.HasKey("SaveDataVersion") )
        {
            Debug.Log("SaveData.CheckData : No Save Data");
            return false;
        }

        if( PlayerPrefs.GetFloat("SaveDataVersion") != SaveDataVersion )
        {
            Debug.Log("SaveData.CheckData : Version Error");
            return false;
        }

        if( !PlayerPrefs.HasKey(dataGroupName) )
        {
            Debug.Log("SaveData.CheckData : No Group Data");
            return false;
        }

        SaveDate = PlayerPrefs.GetString("SaveDataDate");
        return true;
    }

    public static bool CheckGamePlayData()
    {
        return CheckSaveDataHeader("SDG_GamePlay");
    }

    // 코드 플레이 데이터 저장, 불러오기
    public static bool SaveGamePlay()
    {
        try
        {
            Debug.Log("SaveData.SaveGamePlay : Start");

            // SaveDataInfo
            SaveDataHeader("SDG_GamePlay");

            {
                // PlayerData
                zFoxDataPackString playerData = new zFoxDataPackString();
                playerData.Add("Player_HPMax", PlayerController.nowHpMax);
                playerData.Add("Player_HP", PlayerController.nowHp);
                playerData.Add("Player_Score", PlayerController.score);
                playerData.Add("Player_checkPointEnabled", PlayerController.checkPointEnabled);
                playerData.Add("Player_checkPointSceneName", PlayerController.checkPointSceneName);
                playerData.Add("Player_checkPointLabelName", PlayerController.checkPointLabelName);
          
                playerData.PlayerPrefsSetStringUTF8("PlayerData", playerData.EncodeDataPacking());
            }
            {
                // StageData
                zFoxDataPackString stageData = new zFoxDataPackString();
                zFoxUID[] uidList = GameObject.Find("Stage").GetComponentsInChildren<zFoxUID>();

                foreach( zFoxUID uidItem in uidList )
                {
                    if( uidItem.uid != null && uidItem.uid != "(non)" )
                    {
                        stageData.Add(uidItem.uid, true);
                    }
                }

                stageData.PlayerPrefsSetStringUTF8(
                    "StageData_" + SceneManager.GetActiveScene().name,
                    stageData.EncodeDataPacking());
            }
            {
                // EventData
                zFoxDataPackString eventData = new zFoxDataPackString();
                eventData.Add("Event_KeyItem_A", PlayerController.itemKeyA);
                eventData.Add("Event_KeyItem_B", PlayerController.itemKeyA);
                eventData.Add("Event_KeyItem_C", PlayerController.itemKeyA);

                eventData.PlayerPrefsSetStringUTF8(
                    "EventData", eventData.EncodeDataPacking());
            }

            // Save
            PlayerPrefs.Save();

            Debug.Log("SaveData.SaveGamePlay: End");
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogWarning("SaveData.SaveGamePlay : Failed(" + e.Message + ")");
        }
        return false;
    }

    public static bool LoadGamePlay(bool allData)
    {
        try
        {
            // SaveDataInfo
            if( CheckSaveDataHeader("SDG_GamePlay") )
            {
                Debug.Log("SaveData.LoadGamePlay : Start");
                SaveDate = PlayerPrefs.GetString("SaveDataDate");

                if( allData )   // PlayerData
                {
                    zFoxDataPackString playerData = new zFoxDataPackString();

                    playerData.DecodeDataPackString(
                        playerData.PlayerPrefsGetStringUTF8("PlayerData"));

                    PlayerController.nowHpMax = (float)playerData.GetData("Player_HPMax");
                    PlayerController.nowHp = (float)playerData.GetData("Player_HP");
                    PlayerController.score = (int)playerData.GetData("Player_Score");
                    PlayerController.checkPointEnabled = (bool)playerData.GetData("Player_checkPointEnabled");
                    PlayerController.checkPointSceneName = (string)playerData.GetData("Player_checkPointSceneName");
                    PlayerController.checkPointLabelName = (string)playerData.GetData("Player_checkPointLabelName");
                }

                // StageData
                if( PlayerPrefs.HasKey("StageData_" + SceneManager.GetActiveScene().name ) )
                {
                    zFoxDataPackString stageData = new zFoxDataPackString();
                    stageData.DecodeDataPackString(
                        stageData.PlayerPrefsGetStringUTF8("StageData_" + SceneManager.GetActiveScene().name));

                    zFoxUID[] uidList = GameObject.Find("Stage").GetComponentsInChildren<zFoxUID>();

                    foreach( zFoxUID uidItem in uidList )
                    {
                        if( uidItem.uid != null && uidItem.uid != "(non)" )
                        {
                            if( stageData.GetData(uidItem.uid) == null )
                            {
                                uidItem.gameObject.SetActive(false);
                            }
                        }
                    }
                }

                if( allData ) {     // EventData
                    zFoxDataPackString eventData = new zFoxDataPackString();
                    eventData.DecodeDataPackString(
                        eventData.PlayerPrefsGetStringUTF8("EventData"));
                    PlayerController.itemKeyA = (bool)eventData.GetData("Event_KeyItem_A");
                    PlayerController.itemKeyB = (bool)eventData.GetData("Event_KeyItem_B");
                    PlayerController.itemKeyC = (bool)eventData.GetData("Event_KeyItem_C");

                    Debug.Log("SaveData.LoadGamePlay : End");
                    return true;
                }
            }
        }
        catch( System.Exception e )
        {
            Debug.LogWarning("SaveData.LoadGamePlay : Failed(" + e.Message + ")");
        }
        return false;
    }

    public static string LoadContinueSceneName()
    {
        if( CheckSaveDataHeader("SDG_GamePlay") )
        {
            zFoxDataPackString playerData = new zFoxDataPackString();
            playerData.DecodeDataPackString(
                playerData.PlayerPrefsGetStringUTF8("PlayerData"));

            return (string)playerData.GetData("Player_checkPointSceneName");
        }

        continuePlay = false;
        return "StageA";
    }

    // 코드 : 랭킹 데이터 저장, 불러오기
    public static bool SaveHiScore(int playerScore)
    {
        LoadHiScore();

        try
        {
            Debug.Log("SaveData.SaveHiScore : Start");

            // HiScore Set & Sort
            newRecord = 0;
            int[] scoreList = new int[11];
            HiScore.CopyTo(scoreList, 0);
            scoreList[10] = playerScore;
            System.Array.Sort(scoreList);
            System.Array.Reverse(scoreList);

            for( int i = 0; i < 10; i++ )
            {
                HiScore[i] = scoreList[i];
                if( playerScore == HiScore[i] )
                {
                    newRecord = i + 1;
                }
            }

            // HiScoreSave
            SaveDataHeader("SDG_HiScore");
            zFoxDataPackString hiScoreData = new zFoxDataPackString();

            for( int i = 0; i < 10; i++ )
            {
                hiScoreData.Add("Rank" + (i + 1), HiScore[i]);
            }

            hiScoreData.PlayerPrefsSetStringUTF8("HiScoreData", hiScoreData.EncodeDataPacking());

            PlayerPrefs.Save();
            Debug.Log("SaveData.SaveHiScore : End");
            return true;
        }
        catch( System.Exception e )
        {
            Debug.LogWarning("SaveData.SaveHiScore : Failed(" + e.Message + ")");
        }
        return false;
    }

    public static bool LoadHiScore()
    {
        try
        {
            if( CheckSaveDataHeader("SDG_HiScore") ) {
                Debug.Log("SaveData.LoadHiScore : Start");
                zFoxDataPackString hiScoreData = new zFoxDataPackString();
                hiScoreData.DecodeDataPackString(hiScoreData.PlayerPrefsGetStringUTF8("HiScoreData"));

                for( int i = 0; i < 10; i++ )
                {
                    HiScore[i] = (int)hiScoreData.GetData("Rank" + (i + 1));
                }

                Debug.Log("SaveData.LoadHiScore : End");
            }
            return true;
        }
        catch( System.Exception e )
        {
            Debug.LogWarning("SaveData.LoadHiScore : Failed(" + e.Message + ")");
        }
        return false;
    }

    public static bool SaveOption()
    {
        try
        {
            Debug.Log("SaveData.SaveOption : Start");

            // Option Data
            SaveDataHeader("SDG_Option");

            PlayerPrefs.SetFloat("SoundBGMVolume", SoundBGMVolume);
            PlayerPrefs.SetFloat("SoundSEVolume", SoundSEVolume);

            // Save
            PlayerPrefs.Save();
            Debug.Log("SaveData.SaveOption : End");
            return true;
        }
        catch( System.Exception e )
        {
            Debug.LogWarning("SaveData.SaveOption : Failed(" + e.Message + ")");
        }
        return false;
    }

    public static bool LoadOption()
    {
        try
        {
            if( CheckSaveDataHeader("SDG_Option") )
            {
                Debug.Log("SaveData.LoadOption : Start");

                SoundBGMVolume = PlayerPrefs.GetFloat("SoundBGMVolume");
                SoundSEVolume = PlayerPrefs.GetFloat("SoundSEVolume");
                VRPadEnabled = (PlayerPrefs.GetInt("VRPadEnabled") > 0) ? true : false;

                Debug.Log("SaveData.LoadOption : End");
                return true;
            }
        }
        catch( System.Exception e )
        {
            Debug.LogWarning("SaveData.LoadOption : Failed(" + e.Message + ")");
        }
        return false;
    }

    // 코드 : 저장, 불러오기 / 삭제 / 초기화
    public static void DeleteAndInit(bool init)
    {
        Debug.Log("SaveData.DeleteAndInit : DeleteAll");
        PlayerPrefs.DeleteAll();

        if(init)
        {
            Debug.Log("SaveData.DeleteAndInit : Init");
            SaveDate = "(non)";
            SoundBGMVolume = 1.0f;
            SoundSEVolume = 1.0f;
            VRPadEnabled = false;
            HiScoreInitData.CopyTo(HiScore, 0);
        }
    }
}
