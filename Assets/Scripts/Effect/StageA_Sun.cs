using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageA_Sun : MonoBehaviour
{
    Transform playerTrfm;

    // Start is called before the first frame update
    void Awake()
    {
        // transform.position = new Vector3(0.0f, 10.0f, 2.0f);
        playerTrfm = PlayerController.GetTransform();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = playerTrfm.position + new Vector3(8.0f, 3.0f, 2.0f);
        transform.position = new Vector3(targetPosition.x, Mathf.Lerp(transform.position.y, targetPosition.y, 0.0001f),
            targetPosition.z);
    }
}
