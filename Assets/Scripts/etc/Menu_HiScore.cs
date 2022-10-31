using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_HiScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveData.LoadHiScore();
        zFoxFadeFilter.instance.FadeIn(Color.black, 0.5f);

        for( int i = 1; i <= 10; i++ )
        {
            TextMesh tm = GameObject.Find("Rank" + i).GetComponent<TextMesh>();

            if( i == SaveData.newRecord )
            {
                tm.color = Color.red;
            }
            tm.text = string.Format("{0, 2}:{1, 10}", i, SaveData.HiScore[(i - 1)]);
        }
    }
    void Button_Prev()
    {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        Invoke("SceneJump", 0.7f);
    }

    void SceneJump()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Title");
    }
}
