using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_Elevator : MonoBehaviour
{
    public float switchingTime = 5.0f;
    SliderJoint2D slide;

    float changeTime;

    // Start is called before the first frame update
    void Start()
    {
        slide = GetComponent<SliderJoint2D>();
        changeTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if( Time.fixedTime > changeTime + switchingTime )
        {
            slide.useMotor = (slide.useMotor) ? false : true;
            changeTime = Time.fixedTime;
        }
    }
}
