using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Title : MonoBehaviour
{
    string jumpSceneName;

    // Start is called before the first frame update
    void Start()
    {
        // 가비지 컬렉션 실행 ------------------------------------------------------
        System.GC.Collect();
        // --------------------------------------------------------------------

        if ( !SaveData.CheckGamePlayData() )
        {
            GameObject.Find("MenuButton_Continue").SetActive(false);
        }
        else
        {
            GameObject.Find("MenuButton_New").transform.localScale = Vector3.one * 1.0f;
        }

        zFoxFadeFilter.instance.FadeIn(Color.black, 1.0f);
    }

    void Button_Play()
    {
        SaveData.continuePlay = false;
        PlayerController.initParam = true;
        PlayerController.checkPointEnabled = false;

        zFoxFadeFilter.instance.FadeOut(Color.white, 1.0f);
        jumpSceneName = "StageA";
        Invoke("SceneJump", 1.2f);
    }

    void Button_Continue()
    {
        SaveData.continuePlay = true;
        PlayerController.initParam = false;

        zFoxFadeFilter.instance.FadeOut(Color.white, 1.0f);
        jumpSceneName = SaveData.LoadContinueSceneName();
        Invoke("SceneJump", 1.2f);
    }

    void Button_HiScore()
    {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        jumpSceneName = "Menu_HiScore";
        Invoke("SceneJump", 1.2f);
    }

    void Button_Option()
    {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        jumpSceneName = "Menu_Option";
        Invoke("SceneJump", 1.2f);
    }

    void SceneJump()
    {
        Debug.Log(string.Format("Start Game : {0}", jumpSceneName));
        UnityEngine.SceneManagement.SceneManager.LoadScene(jumpSceneName);
    }
}
