using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuObject_Button : MonoBehaviour
{
    public GameObject menuScript;
    public string messege;

    SpriteRenderer menuButton;
    Vector3 orgLocalScale;
    Color orgColor;

    // Start is called before the first frame update
    void Awake()
    {
        menuButton = transform.Find("Menu_Button").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool touchOn = false;
        Vector3 touchPos = Vector3.zero;

        Touch[] tcall = Input.touches;

        foreach( Touch tc in tcall )
        {
            if( tc.phase == TouchPhase.Began )
            {
                touchOn = true;
                touchPos = tc.position;
                break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchOn = true;
            touchPos = Input.mousePosition;
        }

        if( touchOn )
        {
            touchPos.z = transform.position.z - Camera.main.transform.position.z;
            touchPos = Camera.main.ScreenToWorldPoint(touchPos);
            Collider2D col2D = Physics2D.OverlapPoint(touchPos);

            if(col2D != null)
            {
                if( col2D.gameObject == menuButton.gameObject )
                {
                    orgLocalScale = transform.localScale;
                    orgColor = menuButton.color;
                    transform.localScale = transform.localScale * 1.1f;
                    menuButton.color = Color.red;

                    if( Time.timeScale <= 0.0f )
                    {
                        ButtonClick();
                    }
                    else
                    {
                        Invoke("ButtonClick", 0.2f);
                    }
                }
            }
        }
    }

    void ButtonClick()
    {
        transform.localScale = orgLocalScale;
        menuButton.color = orgColor;
        menuScript.SendMessage(messege, this);
    }

    public void SetLabelText(string text)
    {
        transform.Find("Label").GetComponent<TextMesh>().text = text;
    }

    public static MenuObject_Button FindMessage(GameObject form, string message)
    {
        MenuObject_Button[] buttonList =
            form.transform.GetComponentsInChildren<MenuObject_Button>();

        foreach( MenuObject_Button button in buttonList )
        {
            if( message == button.messege )
            {
                return button;
            }
        }

        return null;
    }
}
