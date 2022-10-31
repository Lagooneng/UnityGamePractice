using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    EnemyMain enemyMain;

    private void Awake()
    {
        enemyMain = GetComponentInParent<EnemyMain>();
        // Debug.Log(string.Format(">>> " + enemyMain.name));
    }

    private void OnWillRenderObject()
    {
        // Debug.Log(string.Format(">>> " + Camera.current.tag));
        if ( Camera.current.tag == "MainCamera" )
        {
            enemyMain.cameraEnabled = true;
        }
    }
}
