using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ParticleChrDir : MonoBehaviour
{
    Rigidbody2D rootObject;

    // Start is called before the first frame update
    void Start()
    {
        rootObject = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float ra = (rootObject.transform.localScale.x < 0) ? +10 : -50;
        transform.localRotation = Quaternion.Euler( -150, 90, 0);
    }
}
