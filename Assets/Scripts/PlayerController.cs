using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : BaseCharacterController
{
    // 외부 파라미터, 인스펙터 표시용
    public float initHpMax = 20.0f;
    [Range(0.1f, 100.0f)] public float initSpeed = 12.0f;

    // 외부 파라미터, 저장 데이터 파라미터
    public static float nowHpMax = 0;
    public static float nowHp = 0;
    public static int score;

    // 기본 파라미터
    [System.NonSerialized] public Vector3 enemyActiveZonePointA;
    [System.NonSerialized] public Vector3 enemyActiveZonePointB;
    [System.NonSerialized] public float groundY = 0.0f;

    [System.NonSerialized] public int comboCount = 0;

    // 외부 파라미터, 애니메이션 해시 이름
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Player_Idle");
    public readonly static int ANISTS_Walk = Animator.StringToHash("Base Layer.Player_Walk");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Player_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Player_Jump");
    public readonly static int ANISTS_ATTACK_A = Animator.StringToHash("Base Layer.Player_ATK_A");
    public readonly static int ANISTS_ATTACK_B = Animator.StringToHash("Base Layer.Player_ATK_B");
    public readonly static int ANISTS_ATTACK_C = Animator.StringToHash("Base Layer.Player_ATK_C");
    public readonly static int ANISTS_ATTACKJUMP_A = Animator.StringToHash("Base Layer.Player_ATKJUMP_A");
    public readonly static int ANISTS_ATTACKJUMP_B = Animator.StringToHash("Base Layer.Player_ATKJUMP_B");
    public readonly static int ANISTS_DEAD = Animator.StringToHash("Base Layer.Player_Dead");

    // 캐시
    LineRenderer hudHpBar;
    TextMesh hudScore;
    TextMesh hudCombo;

    // 내부 파라미터
    int jumpCount = 0;

    volatile bool atkInputEnabled = false;
    volatile bool atkInputNow = false;

    bool breakEnabled = true;
    float groundFriction = 0.0f;

    float comboTimer = 0.0f;


    protected override void Awake()
    {
        base.Awake();

        // 캐시
        hudHpBar = GameObject.Find("HUD_HPBar").GetComponent<LineRenderer>();
        hudScore = GameObject.Find("HUD_Score").GetComponent<TextMesh>();
        hudCombo = GameObject.Find("HUD_Combo").GetComponent<TextMesh>();

        // 파리미터 초기화
        speed = initSpeed;
        SetHp(initHpMax, initHpMax);

        // 컬라이터에서 활성 영역을 가져옴
        BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
        // Debug.Log(string.Format(">>> " + boxCol2D.name));
        enemyActiveZonePointA = new Vector3(boxCol2D.offset.x - boxCol2D.size.x / 2.0f,
            boxCol2D.offset.y - boxCol2D.size.y / 2.0f);
        enemyActiveZonePointB = new Vector3(boxCol2D.offset.x + boxCol2D.size.x / 2.0f,
            boxCol2D.offset.y + boxCol2D.size.y / 2.0f);
        boxCol2D.transform.gameObject.SetActive(false);
    }

    protected void Update()
    {
        // 상태 표시
        hudHpBar.SetPosition(1, new Vector3(5.0f * (hp / hpMax), 0.0f, 0.0f));
        hudScore.text = string.Format("Score {0}", score);

        if( comboTimer <= 0.0f )
        {
            hudCombo.gameObject.SetActive(false);
            comboCount = 0;
            comboTimer = 0.0f;
        }
        else
        {
            comboTimer -= Time.deltaTime;

            if( comboTimer > 5.0f )
            {
                comboTimer = 5.0f;
            }

            float s = 0.3f + 0.1f * comboTimer;
            hudCombo.gameObject.SetActive(true);
            hudCombo.transform.localScale = new Vector3(s, s, 1.0f);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Velocity 값 검사
        float vx = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.y, velocityMin.y, velocityMax.y);

        GetComponent<Rigidbody2D>().velocity = new Vector2(vx, vy);
    }

    protected override void FixedUpdateCharacter()
    {
        // 현재 스테이트 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 착지 검사
        if (jumped)
        {
            if ( (grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f) )
            {
                animator.SetTrigger("Idle");
                jumped = false;
                jumpCount = 0;
                rb.gravityScale = gravityScale;
            }

            if( Time.fixedTime > jumpStartTime + 1.0f )
            {
                if(stateInfo.fullPathHash == ANISTS_Idle ||
                    stateInfo.fullPathHash == ANISTS_Walk ||
                    stateInfo.fullPathHash == ANISTS_Run ||
                    stateInfo.fullPathHash == ANISTS_Jump )
                {
                    rb.gravityScale = gravityScale;
                }
            }
        }

        if (!jumped)
        {
            jumpCount = 0;
            rb.gravityScale = gravityScale;
        }

        // 공격 중인지 확인
        if( stateInfo.fullPathHash == ANISTS_ATTACK_A ||
            stateInfo.fullPathHash == ANISTS_ATTACK_B ||
            stateInfo.fullPathHash == ANISTS_ATTACK_C ||
            stateInfo.fullPathHash == ANISTS_ATTACKJUMP_A ||
            stateInfo.fullPathHash == ANISTS_ATTACKJUMP_B)
        {
            speedVx = 0;
        }


        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        if (jumped && !grounded)
        {
            if (breakEnabled)
            {
                breakEnabled = false;
                speedVx *= 0.9f;
            }
        }

        if (breakEnabled)
        {
            speedVx *= 0.9f;
        }

        if (breakEnabled)
        {
            speedVx *= groundFriction;
        }

        // Camera.main.transform.position = transform.position - Vector3.forward;
    }

    public override void ActionMove(float n)
    {
        if (!activeSts)
        {
            return;
        }

        float dirOld = dir;
        breakEnabled = false;

        float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);
        animator.SetFloat("MoveSpeed", moveSpeed);

        if (n != 0.0f)
        {
            // 이동
            dir = Mathf.Sign(n);
            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
            speedVx = initSpeed * moveSpeed * dir;
        }
        else
        {
            breakEnabled = true;
        }

        if (dirOld != dir)
        {
            breakEnabled = true;
        }

    }

    public void ActionJump()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if( stateInfo.fullPathHash == ANISTS_Idle ||
            stateInfo.fullPathHash == ANISTS_Walk ||
            stateInfo.fullPathHash == ANISTS_Run ||
            stateInfo.fullPathHash == ANISTS_Jump)
        {
            switch (jumpCount)
            {
                case 0:
                    if (grounded)
                    {
                        animator.SetTrigger("Jump");
                        GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;
                        jumpStartTime = Time.fixedTime;
                        jumped = true;
                        jumpCount++;
                    }
                    break;
                case 1:
                    if (!grounded)
                    {
                        animator.Play("Player_Jump", 0, 0.0f);
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 20.0f);
                        jumped = true;
                        jumpCount++;
                    }
                    break;
            }
        }
    }

    public void AcctionAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if( stateInfo.fullPathHash == ANISTS_Idle ||
            stateInfo.fullPathHash == ANISTS_Walk ||
            stateInfo.fullPathHash == ANISTS_Run ||
            stateInfo.fullPathHash == ANISTS_Jump ||
            stateInfo.fullPathHash == ANISTS_ATTACK_C )
        {
            animator.SetTrigger("Attack_A");

            if( stateInfo.fullPathHash == ANISTS_Jump ||
                stateInfo.fullPathHash == ANISTS_ATTACK_C )
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0.1f;
            }
        }
        else
        {
            if( atkInputEnabled )
            {
                atkInputEnabled = false;
                atkInputNow = true;
            }
        }
    }

    public void AcctionAttackJump()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == ANISTS_Idle ||
            stateInfo.fullPathHash == ANISTS_Walk ||
            stateInfo.fullPathHash == ANISTS_Run ||
            stateInfo.fullPathHash == ANISTS_Jump ||
            stateInfo.fullPathHash == ANISTS_ATTACK_C)
        {
            animator.SetTrigger("Attack_C");
            jumpCount = 2;
        }
        else
        {
            if( atkInputEnabled )
            {
                atkInputEnabled = false;
                atkInputNow = true;
            }
        }
    }

    public void ActionDamage(float damage)
    {
        if( !activeSts )
        {
            return;
        }

        animator.SetTrigger("DMG_A");
        speedVx = 0;
        rb.gravityScale = gravityScale;

        if( jumped )
        {
            damage *= 1.5f;
        }

        if( SetHp(hp - damage, hpMax) )
        {
            Dead();
        }
    }

    public override void Dead()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if( !activeSts || stateInfo.fullPathHash == ANISTS_DEAD )
        {
            return;
        }

        base.Dead();

        SetHp(0, hpMax);
        Invoke("GameOver", 3.0f);

        GameObject.Find("HUD_Dead").GetComponent<MeshRenderer>().enabled = true;
        GameObject.Find("HUD_DeadShadow").GetComponent<MeshRenderer>().enabled = true;
    }

    public void GameOver()
    {
        PlayerController.score = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Application.LoadLevel(Application.loadedLevelName); ->>> obsolete
    }

    public override bool SetHp(float _hp, float _hpMax)
    {
        if( _hp > _hpMax )
        {
            _hp = _hpMax;
        }

        nowHp = _hp;
        nowHpMax = _hpMax;

        return base.SetHp(_hp, _hpMax);
    }

    public void AddCombo()
    {
        comboCount++;
        comboTimer += 1.0f;
        hudCombo.text = string.Format("Combo {0}", comboCount);
    }

    // 지원 함수
    public static GameObject GetGameObject()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static Transform GetTransform()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }

    public static PlayerController GetController()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public static Animator GetAnimator()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // 애니메이션 이벤트용 코드
    public void EnableAttackInput()
    {
        atkInputEnabled = true;
    }

    public void SetNextAttack(string name)
    {
        if( atkInputNow == true )
        {
            atkInputNow = false;
            animator.Play(name);
        }
    }
}
