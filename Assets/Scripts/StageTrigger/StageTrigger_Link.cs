using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageTrigger_Link : MonoBehaviour
{
    // 외부 파라미터, 인스펙터 표시
    public string jumpSceneName;
    public string jumpLabelName;

    public int jumpDir = 0;
    public bool jumpInput = true;
    public float jumpDelayTime = 0.0f;
    public bool save = true;
    public bool sePlay = true;

    // 내부 파라미터
    Transform playerTrfm;
    PlayerController playerCtrl;

    // Start is called before the first frame update
    void Awake()
    {
        playerTrfm = PlayerController.GetTransform();
        playerCtrl = playerTrfm.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D_PlayerEvent(GameObject go)
    {
        if( !jumpInput )
        {
            Jump();
        }
    }

    public void Jump()
    {
        // 점프할 곳을 설정
        if( jumpSceneName == "" )
        {
            jumpSceneName = SceneManager.GetActiveScene().name;
        }

        // 체크 포인트
        PlayerController.checkPointEnabled = true;
        PlayerController.checkPointLabelName = jumpLabelName;
        PlayerController.checkPointSceneName = jumpSceneName;
        PlayerController.checkPointHp = PlayerController.nowHp;
        if( save )
        {
            SaveData.SaveGamePlay();
        }

        playerCtrl.ActionMove(0.0f);
        playerCtrl.activeSts = false;

        Invoke("JumpWork", jumpDelayTime);
    }

    void JumpWork()
    {
        playerCtrl.activeSts = true;

        if( SceneManager.GetActiveScene().name == jumpSceneName )
        {
            // 씬 안에서 점프
            GameObject[] stageLinkList = GameObject.FindGameObjectsWithTag("EventTrigger");

            foreach( GameObject stageLink in stageLinkList )
            {
                if( stageLink.GetComponent<StageTrigger_CheckPoint>().labelName == jumpLabelName )
                {
                    playerTrfm.position = stageLink.transform.position;
                    playerCtrl.groundY = playerTrfm.position.y;
                    Camera.main.transform.position = new Vector3(playerTrfm.position.x,
                        playerTrfm.position.y, -10.0f);
                    break;
                }
            }
        }
        else
        {
            // 씬 밖으로 점프
            PlayerController.startFadeTime = 0.5f;
            SceneManager.LoadScene(jumpSceneName);
        }
    }
}
