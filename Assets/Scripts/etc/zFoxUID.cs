using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum zFOXUID_TYPE
{
    NUMBER,
    GUID
}

public class zFoxUID : MonoBehaviour
{
    public zFOXUID_TYPE type;
    public string uid;

    public zFoxUID()
    {
        type = zFOXUID_TYPE.NUMBER;
        uid = "(non)";
    }
}
