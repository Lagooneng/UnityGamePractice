using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Option : MonoBehaviour
{
    void Start()
    {
        zFoxFadeFilter.instance.FadeIn(Color.black, 0.5f);
        SaveData.LoadOption();
        MenuObject_Button.FindMessage(
            GameObject.Find("MenuFormA"), "Button_VRPad").
            SetLabelText((SaveData.VRPadEnabled ? "ON" : "OFF"));
    }

    void Update()
    {
        GameObject.Find("SaveData_Date").GetComponent<TextMesh>().text =
            SaveData.SaveDate;
    }

    void Button_VRPad(MenuObject_Button button)
    {
        SaveData.VRPadEnabled = SaveData.VRPadEnabled ? false : true;
        button.SetLabelText((SaveData.VRPadEnabled ? "ON" : "OFF"));
    }

    void Button_SaveDataReset()
    {
        GameObject.Find("MenuFormA").transform.position = new Vector3(-100.0f, 0.0f, 0.0f);
        GameObject.Find("MenuFormB").transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void Button_Prev()
    {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        Invoke("SceneJump", 0.7f);
    }

    void SceneJump()
    {
        SaveData.SaveOption();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Title");
    }

    void Button_SaveDataReset_Yes()
    {
        GameObject.Find("MenuFormA").transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject.Find("MenuFormB").transform.position = new Vector3(100.0f, 0.0f, 0.0f);

        SaveData.DeleteAndInit(true);
    }

    void Button_SaveDataReset_No()
    {
        GameObject.Find("MenuFormA").transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject.Find("MenuFormB").transform.position = new Vector3(100.0f, 0.0f, 0.0f);
    }

    void Button_Debug()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Debug");
    }
}
