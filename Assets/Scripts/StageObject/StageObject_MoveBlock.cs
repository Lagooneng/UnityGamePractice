using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_MoveBlock : MonoBehaviour
{
    public Vector3 velocityA = new Vector3(1.0f, 0.0f, 0.0f);
    public Vector3 velocityB = new Vector3(-1.0f, 0.0f, 0.0f);
    public float switchingTime = 5.0f;
    public float vt = 0;

    bool turn = false;
    float changeTime = 0.0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if( changeTime <= 0.0f )
        {
            changeTime = Time.fixedTime;
            rb.velocity = velocityA;
        }

        if( Time.fixedTime + vt > changeTime + switchingTime )
        {
            rb.velocity = turn ? velocityA : velocityB;
            turn = turn ? false : true;
            changeTime = Time.fixedTime;
        }
    }
}
