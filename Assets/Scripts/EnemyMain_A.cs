using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_A : EnemyMain
{
    // 외부 파라미터, 인스펙터 표시
    public int aiIfRUNTOPLAYER = 20;
    public int aiIfJUMPTOPLAYER = 30;
    public int aiIfESCAPE = 10;
    public int aiIfRETURNTODOGPILE = 10;

    public int damageATTACK_A = 1;

    // AI 사고 루틴 처리
    public override void FixedUpdateAI()
    {
        // Debug.Log(string.Format(">>> aists {0}", aiState));
        // AI 스테이트
        
        switch(aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: // 사고 루틴 기점
                // 액션 선택
                int n = SelectRandomAIState();

                if( n < aiIfRUNTOPLAYER )
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                }
                else if( n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER )
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 1.0f);
                }
                else if( n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE )
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(2.0f, 5.0f));
                }
                else if( n < aiIfJUMPTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE + aiIfRETURNTODOGPILE )
                {
                    if ( dogPile != null )
                    {
                        SetAIState(ENEMYAISTS.RETRUNTONDOGPILE, 3.0f);
                    }
                }
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 1.0f));
                }

                break;

            case ENEMYAISTS.WAIT:
                enemyCtrl.ActionLookUp(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);

                break;

            case ENEMYAISTS.RUNTOPLAYER:
                if( GetDistancePlayerY() > 3.0f )
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 1.0f);
                }
                if( !enemyCtrl.ActionMoveToNear(player, 2.0f) )
                {
                    Attack_A();
                }

                break;

            case ENEMYAISTS.JUMPTOPLAYER:
                if( GetDistancePlayer() < 2.0f && IsChangeDistancePlayer(0.5f) )
                {
                    Attack_A();
                    break;
                }

                enemyCtrl.ActionJump();
                enemyCtrl.ActionMoveToNear(player, 0.1f);
                SetAIState(ENEMYAISTS.FREEZ, 0.5f);

                break;

            case ENEMYAISTS.ESCAPE:
                if( !enemyCtrl.ActionMoveToFar(player, 7.0f) )
                {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }

                break;

            case ENEMYAISTS.RETRUNTONDOGPILE:
                if( enemyCtrl.ActionMoveToNear(dogPile, 2.0f) )
                {
                    if( GetDistancePlayer() < 2.0f )
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

    void Attack_A()
    {
        enemyCtrl.ActionLookUp(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageATTACK_A);
        SetAIState(ENEMYAISTS.WAIT, 2.0f);
    }

    private void jump()
    {
        enemyCtrl.rb.AddForce(new Vector2(0.0f, 1000.0f));
    }

    // 코드, combatAI
    public override void SetCombatAIState(ENEMYAISTS sts)
    {
        base.SetCombatAIState(sts);

        switch(aiState)
        {
            case ENEMYAISTS.ACTIONSELECT: break;
            case ENEMYAISTS.WAIT:
                aiActionTimeLength = 1.0f + Random.Range(0.0f, 1.0f); break;
            case ENEMYAISTS.RUNTOPLAYER:  aiActionTimeLength = 3.0f; break;
            case ENEMYAISTS.JUMPTOPLAYER: aiActionTimeLength = 1.0f; break;
            case ENEMYAISTS.ESCAPE:
                aiActionTimeLength = Random.Range(2.0f, 5.0f); break;
            case ENEMYAISTS.RETRUNTONDOGPILE: aiActionTimeLength = 3.0f; break;
        }
    }
}
