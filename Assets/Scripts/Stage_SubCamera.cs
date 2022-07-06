using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_SubCamera : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("HUD_Dead").GetComponent<MeshRenderer>().enabled = false;
        GameObject.Find("HUD_DeadShadow").GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
