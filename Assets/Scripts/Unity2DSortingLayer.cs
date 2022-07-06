using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity2DSortingLayer : MonoBehaviour
{
    public string sortingLayerName = "Front";
    public int sortingOrder = 0;
    Renderer rd;

    private void Awake()
    {
        rd = GetComponent<Renderer>();
        rd.sortingLayerName = sortingLayerName;
        rd.sortingOrder = sortingOrder;
    }

}
