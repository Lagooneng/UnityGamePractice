using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    // 캐시
    PlayerController playerCtrl;
    zFoxVirtualPad vpad;

    bool actionEtcRun = true;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
        vpad = GameObject.FindObjectOfType<zFoxVirtualPad>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerCtrl.activeSts)
        {
            return;
        }

        // 가상 패드
        float vpad_vertical = 0.0f;
        float vpad_horizontal = 0.0f;
        zFOXVPAD_BUTTON vpad_btnA = zFOXVPAD_BUTTON.NON;
        zFOXVPAD_BUTTON vpad_btnB = zFOXVPAD_BUTTON.NON;

        if( vpad != null )
        {
            vpad_vertical = vpad.vertical;
            vpad_horizontal = vpad.horizontal;
            vpad_btnA = vpad.buttonA;
            vpad_btnB = vpad.buttonB;
        }

        float joyMv = Input.GetAxis("Horizontal");
        float vpadMv = vpad_horizontal;

        playerCtrl.ActionMove(joyMv + vpadMv);

        if( Input.GetButtonDown("Jump") || vpad_btnA == zFOXVPAD_BUTTON.DOWN )
        {
            playerCtrl.ActionJump();
            return;
        }

        // 공격
        if( Input.GetButtonDown("Fire1") ||
            Input.GetButtonDown("Fire2") ||
            Input.GetButtonDown("Fire3") ||
            vpad_btnB == zFOXVPAD_BUTTON.DOWN )
        {
            if( Input.GetAxisRaw("Vertical") + vpad_vertical < 0.5f )
            {
                playerCtrl.AcctionAttack();
            }
            else
            {
                playerCtrl.AcctionAttackJump();
            }    
        }

        // 문을 열거나 통로에 들어간다

        if( Input.GetAxisRaw("Vertical") + vpad_vertical > 0.7f )
        {
            if( actionEtcRun )
            {
                playerCtrl.ActionEtc();
                actionEtcRun = false;
            }
        }
        else
        {
            actionEtcRun = true;
        }
    }
}
