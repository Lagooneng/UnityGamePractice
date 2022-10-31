using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Pause : MonoBehaviour
{
    PlayerController playerCtrl;
    GameObject pauseButton;
    GameObject exitButton;
    float activeTime;

    private void Start()
    {
        playerCtrl = PlayerController.GetController();
        pauseButton = transform.Find("MenuButton_Pause").gameObject;
        exitButton = transform.Find("MenuButton_Exit").gameObject;
        exitButton.SetActive(false);
    }

    private void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x,
            Camera.main.transform.position.y, 0.0f);

        if( !Input.anyKey &&
            (Mathf.Abs(Input.GetAxisRaw("Vertical")) < 0.05f) &&
            (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.05f) )
        {
            if( Time.time > activeTime + 2.0f )
            {
                pauseButton.SetActive(true);
            }
        }
        else
        {
            Invoke("Check", 1.0f);
        }
    }

    void Check()
    {
        if( Time.timeScale > 0.0f )
        {
            pauseButton.SetActive(false);
            activeTime = Time.time;
        }
    }

    void Button_Pause()
    {
        Time.timeScale = (Time.timeScale > 0.0f) ? 0.0f : 1.0f;
        playerCtrl.activeSts = (Time.timeScale > 0.0f) ? true : false;
        exitButton.SetActive((Time.timeScale <= 0.0f) ? true : false);
    }

    void Button_Exit()
    {
        Time.timeScale = 1.0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Title");
    }
}
