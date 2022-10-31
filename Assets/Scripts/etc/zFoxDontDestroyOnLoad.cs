using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zFoxDontDestroyOnLoad : MonoBehaviour
{
    public bool DonDestroyEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        if( DonDestroyEnabled )
        {
            DontDestroyOnLoad(this);
        }
    }
}
