using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    // 캐시
    PlayerController playerCtrl;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerCtrl.activeSts)
        {
            return;
        }

        float joyMv = Input.GetAxis("Horizontal");
        playerCtrl.ActionMove(joyMv);

        if (Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
            return;
        }

        // 공격
        if( Input.GetButtonDown("Fire1") ||
            Input.GetButtonDown("Fire2") ||
            Input.GetButtonDown("Fire3") )
        {
            if( Input.GetAxisRaw("Vertical") < 0.5f )
            {
                playerCtrl.AcctionAttack();
            }
            else
            {
                playerCtrl.AcctionAttackJump();
            }
            
        }
    }
}
