using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_DogPile : MonoBehaviour
{
    public GameObject[] enemyList;
    public GameObject[] destroyObjectList;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckEnemy", 0.0f, 1.0f);   
    }

    void CheckEnemy()
    {
        // 적이 생존 상태인지 확인, 1초에 한 번
        bool flag = true;

        foreach( GameObject enemy in enemyList )
        {
            if( enemy != null )
            {
                flag = false;
            }
        }

        // 모든 적이 쓰러져 있는가?
        if( flag )
        {
            // 제거할 게임 오브젝트 리스트에 포함된 오브젝트를 삭제한다
            foreach( GameObject destroyObject in destroyObjectList )
            {
                destroyObject.AddComponent<Effect_FadeObject>();
                destroyObject.SendMessage("FadeStart");
                Destroy(destroyObject, 1.0f);
            }
            CancelInvoke("CheckEnemy");
        }
    }
}
