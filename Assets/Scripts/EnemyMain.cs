using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYAISTS  // 적 AI 상태
{
    ACTIONSELECT,       // 액션 선택
    WAIT,               // 대기
    RUNTOPLAYER,        // 달려서 플레이어에
    JUMPTOPLAYER,       // 점프해서 플레이어에
    ESCAPE,             // 도망
    RETRUNTONDOGPILE,   // 도그 파일로 돌아온다
    ATTACKONSIGHT,      // 제자리에서 공격: 원거리 공격
    FREEZ               // 행동 정지, 단 이동 처리는 계속함
}

public class EnemyMain : MonoBehaviour
{
    // 외부 파라미터, 인스펙터 표시
    public bool cameraSwitch = true;
    public bool inActiveZoneSwitch = false;
    public bool combatAIOrder = true;
    public int debug_SelectRandomAIState = -1;
    public float dogPileReturnLength = 10.0f;

    // 외부 파라미터
    [System.NonSerialized] public bool cameraEnabled = false;
    [System.NonSerialized] public bool inActiveZone = false;
    [System.NonSerialized] public ENEMYAISTS aiState = ENEMYAISTS.ACTIONSELECT;
    [System.NonSerialized] public GameObject dogPile;

    // 캐시
    protected EnemyController enemyCtrl;
    protected GameObject player;
    protected PlayerController playerCtrl;

    // 내부 파라미터
    protected float aiActionTimeLength = 0.0f;
    protected float aiActionTimeStart = 0.0f;
    protected float distanceToPlayer = 0.0f;
    protected float distanceToPlayerPrev = 0.0f;

    // 코드
    public virtual void Awake()
    {
        enemyCtrl = GetComponent<EnemyController>();
        player = PlayerController.GetGameObject();
        playerCtrl = player.GetComponent<PlayerController>();
    }

    public virtual void Start()
    {
        // Dog Pile Set
        StageObject_DogPile[] dogPileList = GameObject.FindObjectsOfType<StageObject_DogPile>();
        foreach( StageObject_DogPile findDogFile in dogPileList )
        {
            foreach( GameObject go in findDogFile.enemyList )
            {
                if( gameObject == go )
                {
                    dogPile = findDogFile.gameObject;
                    break;
                }
            }
        }
    }

    public virtual void Update()
    {
        cameraEnabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if( enemyCtrl.grounded && CheckAction() )
        {
            if (other.name == "EnemyJumpTrigger_L")
            {
                if (enemyCtrl.ActionJump())
                {
                    enemyCtrl.ActionMove(-1.0f);
                }
            }
            else if (other.name == "EnemyJumpTrigger_R")
            {
                if (enemyCtrl.ActionJump())
                {
                    enemyCtrl.ActionMove(+1.0f);
                }
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if( BeginEnemyCommonWork() )
        {
            FixedUpdateAI();
            EndEnemyCommonWork();
        }
    }

    public virtual void FixedUpdateAI() {
    }

    public bool BeginEnemyCommonWork()
    {
        // 살아 있는지 확인
        if( enemyCtrl.hp <= 0 )
        {
            return false;
        }

        // 활성 영역에 들어와 있는지 확인
        if( inActiveZoneSwitch )
        {
            inActiveZone = false;
            Vector3 vecA = player.transform.position + playerCtrl.enemyActiveZonePointA;
            Vector3 vecB = player.transform.position + playerCtrl.enemyActiveZonePointB;

            if( transform.position.x > vecA.x && transform.position.x < vecB.x &&
                transform.position.y > vecA.y && transform.position.y < vecB.y )
            {
                inActiveZone = true;
            }
        }


        // 공중에서는 강제 실행
        if( enemyCtrl.grounded )
        {
            // 카메라에 비쳐지고 있는지 확인
            if( cameraSwitch && !cameraEnabled && !inActiveZone )
            {
                // 비쳐지고 있지 않다
                enemyCtrl.ActionMove(0.0f);
                enemyCtrl.cameraRendered = false;
                enemyCtrl.animator.enabled = false;
                enemyCtrl.rb.Sleep();

                return false;
            }
        }

        enemyCtrl.animator.enabled = true;
        enemyCtrl.cameraRendered = true;

        // 상태 검사
        if( !CheckAction() )
        {
            return false;
        }

        // 도그 파일
        if( dogPile != null )
        {
            if( GetDistanceDogPile() > dogPileReturnLength )
            {
                aiState = ENEMYAISTS.RETRUNTONDOGPILE;
            }
        }

        return true;
    }

    public void EndEnemyCommonWork()
    {
        // 액션 한계 시간 검사
        float time = Time.fixedTime - aiActionTimeStart;

        if( time > aiActionTimeLength )
        {
            aiState = ENEMYAISTS.ACTIONSELECT;
        }
    }

    public bool CheckAction()
    {
        // 상태 검사
        AnimatorStateInfo stateInfo = enemyCtrl.animator.GetCurrentAnimatorStateInfo(0);

        if( stateInfo.fullPathHash == EnemyController.ANITAG_ATTACK ||
            stateInfo.fullPathHash == EnemyController.ANISTS_DMG_A ||
            stateInfo.fullPathHash == EnemyController.ANISTS_DMG_B ||
            stateInfo.fullPathHash == EnemyController.ANISTS_Dead )
        {
            return false;
        }

        return true;
    }

    public int SelectRandomAIState()
    {
#if UNITY_EDITOR
        if( debug_SelectRandomAIState >= 0 )
        {
            return debug_SelectRandomAIState;
        }
#endif
        return Random.Range(0, 100 + 1);
    }

    public void SetAIState(ENEMYAISTS sts, float t)
    {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        aiActionTimeLength = t;
    }

    public virtual void SetCombatAIState(ENEMYAISTS sts)
    {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        enemyCtrl.ActionMove(0.0f);
    }

    public float GetDistancePlayer()
    {
        distanceToPlayerPrev = distanceToPlayer;
        distanceToPlayer = Vector3.Distance(transform.position, playerCtrl.transform.position);

        return distanceToPlayer;
    }

    public bool IsChangeDistancePlayer(float l)
    {
        return (Mathf.Abs(distanceToPlayer - distanceToPlayerPrev) > l);
    }

    public float GetDistancePlayerX()
    {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;

        posA.y = 0;
        posA.z = 0;
        posB.y = 0;
        posB.z = 0;

        return Vector3.Distance(posA, posB);
    }

    public float GetDistancePlayerY()
    {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;

        posA.x = 0;
        posA.z = 0;
        posB.x = 0;
        posB.z = 0;

        return Vector3.Distance(posA, posB);
    }

    public float GetDistanceDogPile()
    {
        return Vector3.Distance(transform.position, dogPile.transform.position);
    }
}
