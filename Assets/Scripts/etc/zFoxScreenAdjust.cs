using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zFoxScreenAdjust : MonoBehaviour
{
    public float aspectWH = 1.6f;
    public float aspectAdd = 0.05f;

    public bool StartScreenAdjust = true;
    public bool UpdateScreenAdjust = false;

    Vector3 localScale;

    private void Start()
    {
        localScale = transform.localScale;

        if( StartScreenAdjust )
        {
            ScreenAdjsut();
        }
    }

    private void Update()
    {
        if( UpdateScreenAdjust )
        {
            ScreenAdjsut();
        }
    }

    void ScreenAdjsut()
    {
        float wh = (float)Screen.width / (float)Screen.height;
        Debug.Log(string.Format("aspectWH: {0} wh:{1}", aspectWH, wh));

        if( wh < aspectWH )
        {
            transform.localScale = new Vector3(
                localScale.x - (aspectWH - wh) + aspectAdd,
                localScale.y, localScale.z);
        }
        else
        {
            transform.localScale = localScale;
        }
    }
}
