using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_C : EnemyMain
{
    // 외부 파라미터, 인스펙터 표시
    public int aiIfATTACKONSIGHT = 50;
    public int aiIfRUNTOPLAYER = 10;
    public int aiIfESCAPE = 10;
    public int aiIfRETURNTODOGPILE = 10;
    public float aiPlayerEscapeDistance = 0.0f;

    public int damageAttack_A = 1;

    public int fireAttack_A = 3;
    public float waitAttack_A = 10.0f;

    // 내부 파라미터
    int fireCountAttack_A = 0;
    

    // 코드
    public override void FixedUpdateAI()
    {
        enemyCtrl.ActionMoveToFar(player, aiPlayerEscapeDistance);
        enemyCtrl.ActionLookUp(player, 0.1f);
        //Debug.Log(string.Format(">>> " + aiState));
        // AI 스테이트

        switch ( aiState )
        {
            case ENEMYAISTS.ACTIONSELECT:
                int n = SelectRandomAIState();

                if( n < aiIfATTACKONSIGHT )
                {
                    SetAIState(ENEMYAISTS.ATTACKONSIGHT, 100.0f);
                }
                else if( n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER )
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                }
                else if( n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER + aiIfESCAPE )
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(2.0f, 5.0f));
                }
                else if (n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER + aiIfESCAPE + aiIfRETURNTODOGPILE)
                {
                    if (dogPile != null)
                    {
                        SetAIState(ENEMYAISTS.RETRUNTONDOGPILE, 3.0f);
                    }
                }
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 1.0f));
                }
                enemyCtrl.ActionMove(0.0f);

                break;

            case ENEMYAISTS.WAIT:
                enemyCtrl.ActionLookUp(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);

                break;

            case ENEMYAISTS.ATTACKONSIGHT:
                Attack_A();

                break;

            case ENEMYAISTS.RUNTOPLAYER:
                if( !enemyCtrl.ActionMoveToNear(player, 10.0f) )
                {
                    Attack_A();
                }

                break;
            case ENEMYAISTS.ESCAPE:
                if ( !enemyCtrl.ActionMoveToFar(player, 4.0f) )
                {
                    Attack_A();
                }

                break;

            case ENEMYAISTS.RETRUNTONDOGPILE:
                if (enemyCtrl.ActionMoveToNear(dogPile, 2.0f))
                {
                    if (GetDistancePlayer() < 2.0f)
                    {
                        Attack_A();
                    }
                    else
                    {
                        SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                    }
                }

                break;
        }
    }

    // 코드, 액션 처리
    void Attack_A()
    {
        enemyCtrl.ActionLookUp(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageAttack_A);

        fireCountAttack_A++;

        if( fireCountAttack_A >= fireAttack_A )
        {
            fireCountAttack_A = 0;
            SetAIState(ENEMYAISTS.FREEZ, waitAttack_A);
        }
    }

    // 코드, combatAI
    public override void SetCombatAIState(ENEMYAISTS sts)
    {
        base.SetCombatAIState(sts);

        switch (aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: break;
            case ENEMYAISTS.WAIT:
                aiActionTimeLength = 1.0f + Random.Range(0.0f, 1.0f); break;
            case ENEMYAISTS.RUNTOPLAYER: aiActionTimeLength = 3.0f; break;
            case ENEMYAISTS.JUMPTOPLAYER: aiActionTimeLength = 1.0f; break;
            case ENEMYAISTS.ESCAPE:
                aiActionTimeLength = Random.Range(2.0f, 5.0f); break;
            case ENEMYAISTS.RETRUNTONDOGPILE: aiActionTimeLength = 3.0f; break;
        }
    }
}
