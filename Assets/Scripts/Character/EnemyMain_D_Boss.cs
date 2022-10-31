using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_D_Boss : EnemyMain
{
    // 외부 파라미터, 인스펙터 표시
    public int aiIfRUNTOPLAYER = 30;
    public int aiIfJUMPTOPLAYER = 10;
    public int aiIfESCAPE = 20;
    public int aiIFRETURNTODOGPILE = 10;

    // 캐시
    GameObject bossHud;
    LineRenderer hudHpBar;
    Transform playerTrfm;

    // 내부 파라미터
    float dogPileCheckTime = 0.0f;
    float jumpCheckTime = 0.0f;

    // 코드
    public override void Start()
    {
        base.Start();
        bossHud = GameObject.Find("BossHud");
        hudHpBar = GameObject.Find("HUD_HPBar_Boss").GetComponent<LineRenderer>();
        playerTrfm = PlayerController.GetTransform();
    }

    public override void Update()
    {
        base.Update();
        // 상태 표시
        if( enemyCtrl.hp > 0 )
        {
            hudHpBar.SetPosition(1, new Vector3(18.0f * ((float)enemyCtrl.hp / (float)enemyCtrl.hpMax), 0.0f, 0.0f));
        }
        else
        {
            if( bossHud != null )
            {
                bossHud.SetActive(false);
                bossHud = null;
            }
        }
    }

    public override void FixedUpdateAI()
    {
        bool play;
        // 플레이어가 스테이지 양쪽 끝으로 도망간 상태라면 도그 파일까지 쫓아온다
        
        if( Time.fixedTime - dogPileCheckTime > 3.0f &&
                (playerTrfm.position.x < 22.0f || playerTrfm.position.x > 45.0f))
        {
            if( transform.position.x < 24.0f || transform.position.x > 44.0f )
            {
                if( dogPile != null )
                {
                    SetAIState(ENEMYAISTS.RETRUNTONDOGPILE, Random.Range(2.0f, 3.0f));
                }
            }
            else
            {
                Attack_A();
                SetAIState(ENEMYAISTS.WAIT, 3.0f);
            }

            dogPileCheckTime = Time.fixedTime;
            jumpCheckTime = Time.fixedTime + 3.0f;
        }
        // 플레이어가 올라타고 있을 때나 깔려 있을 떄를 위한 긴급 처리
        else if( Time.fixedTime - jumpCheckTime > 1.0f &&
                    enemyCtrl.hp > enemyCtrl.hpMax / 2.0f &&
                    GetDistancePlayer() < 4.0f )
        {
            Attack_Jump();
            // SetAIState(ENEMYAISTS.WAIT, 3.0f);
            jumpCheckTime = Time.fixedTime;
        }
        
        // AI 상태
        // Debug.Log(string.Format(">>> " + aiState));
        switch ( aiState )
        {
            case ENEMYAISTS.ACTIONSELECT:
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
                else if( n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE + aiIFRETURNTODOGPILE )
                {
                    if( dogPile != null )
                    {
                        SetAIState(ENEMYAISTS.RETRUNTONDOGPILE, Random.Range(2.0f, 3.0f));
                    }
                }
                else
                {
                    SetAIState(ENEMYAISTS.WAIT, Random.Range(0.0f, 1.0f));
                }
                enemyCtrl.ActionMove(0.0f);

                // 호밍 공격은 체력이 떨어졌을 때부터
                if (enemyCtrl.hp > enemyCtrl.hpMax / 4.0f)
                {
                    if (aiState == ENEMYAISTS.ESCAPE)
                    {
                        Attack_B();
                        SetAIState(ENEMYAISTS.WAIT, 1.0f);
                    }
                }
                break;

            case ENEMYAISTS.WAIT:
                enemyCtrl.ActionLookUp(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.RUNTOPLAYER:
                play = enemyCtrl.ActionMoveToNear(player, 7.0f);
                if ( !play )
                {
                    Attack_A();
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("Run");
                }
                break;

            case ENEMYAISTS.JUMPTOPLAYER:
                if( GetDistancePlayer() > 5.0f )
                {
                    Attack_Jump();
                }
                else
                {
                    enemyCtrl.ActionLookUp(player, 0.1f);
                    SetAIState(ENEMYAISTS.WAIT, 1.0f);
                }
                break;

            case ENEMYAISTS.ESCAPE:
                play = enemyCtrl.ActionMoveToNear(player, 7.0f);
                if (!play)
                {
                    Attack_A();
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("Run");
                }
                break;

            case ENEMYAISTS.RETRUNTONDOGPILE:
                if( enemyCtrl.ActionMoveToNear(dogPile, 3.0f) ) { }
                else
                {
                    enemyCtrl.ActionMove(0.0f);
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 0.0f);
                }
                break;
        }
        
    }

    void Attack_A()
    {
        enemyCtrl.ActionLookUp(player, 0.1f);
        enemyCtrl.ActionAttack("Attack_A", 5);
        enemyCtrl.attackNockBackVector = new Vector2(1000.0f, 100.0f);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }

    void Attack_B()
    {
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_B", 0);
        SetAIState(ENEMYAISTS.WAIT, 3.0f);
    }

    void Attack_Jump()
    {
        enemyCtrl.ActionLookUp(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.attackEnabled = true;
        enemyCtrl.attackDamage = 1;
        enemyCtrl.attackNockBackVector = new Vector2(1000.0f, 100.0f);
        enemyCtrl.ActionJump();
    }
}
