using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationStartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveData.LoadOption();
    }
}
