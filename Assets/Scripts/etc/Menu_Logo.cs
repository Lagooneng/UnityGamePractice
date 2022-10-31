using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Logo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LogoMark");
    }

    IEnumerator LogoMark()
    {
        zFoxFadeFilter.instance.FadeIn(Color.black, 1.0f);
        yield return new WaitForSeconds(3.0f);
        zFoxFadeFilter.instance.FadeOut(Color.black, 1.0f);
        yield return new WaitForSeconds(1.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Title");
    }
}
