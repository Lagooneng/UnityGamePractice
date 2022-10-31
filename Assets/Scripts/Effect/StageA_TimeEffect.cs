using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageA_TimeEffect : MonoBehaviour
{
    GameObject player;

    SpriteRenderer CameraFilter;
    Color paperColor_A = Color.black;
    Color paperColor_B = new Color(1.0f, 0.0f, 0.0f, 0.22f);

    LineRenderer Stage_BackColor;
    Color backColorST_A = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 낮
    Color backColorED_A = new Color(0.0f, 0.916f, 1.0f, 1.0f);  
    Color backColorST_B = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 저녁
    Color backColorED_B = new Color(1.0f, 1.0f, 1.0f, 1.0f);


    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.GetGameObject();
        CameraFilter = GameObject.Find("Filter_Paper").GetComponent<SpriteRenderer>();
        Stage_BackColor = GameObject.Find("StageA_BackColor").GetComponent<LineRenderer>();
        paperColor_A = CameraFilter.color;
    }

    // Update is called once per frame
    void Update()
    {
        float t = player.transform.position.x / 380.0f;
        CameraFilter.color = Color.Lerp(paperColor_A, paperColor_B, t);
        Color st = Color.Lerp(backColorST_A, backColorST_B, t);
        Color ed = Color.Lerp(backColorED_A, backColorED_B, t);
        Stage_BackColor.startColor = st;
        Stage_BackColor.endColor = ed;
    }
}
