using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseCharacterController
{
    // 외부 파라미터, 인스펙터 표시
    public float initHpMax = 5.0f;
    public float initSpeed = 6.0f;
    public bool jumpActionEnabled = true;
    public Vector2 jumpPower = new Vector2(0.0f, 1500.0f);
    public int addScore = 500;

    // 외부 파라미터
    [System.NonSerialized] public bool cameraRendered = false;
    [System.NonSerialized] public bool attackEnabled = false;
    [System.NonSerialized] public int attackDamage = 1;
    [System.NonSerialized] public Vector2 attackNockBackVector = Vector3.zero;

    // 애니메이션 해시 이름
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Enemy_Idle");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Enemy_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Enemy_Jump");
    public readonly static int ANITAG_ATTACK = Animator.StringToHash("Attack");
    public readonly static int ANISTS_DMG_A = Animator.StringToHash("Base Layer.Enemy_DMG_A");
    public readonly static int ANISTS_DMG_B = Animator.StringToHash("Base Layer.Enemy_DMG_B");
    public readonly static int ANISTS_Dead = Animator.StringToHash("Base Layer.Enemy_Dead");

    // 캐시
    PlayerController playerCtrl;
    Animator playerAnim;

    // 코드
    protected override void Awake()
    {
        base.Awake();

        playerCtrl = PlayerController.GetController();
        playerAnim = playerCtrl.GetComponent<Animator>();
        hpMax = initHpMax;
        hp = hpMax;
        speed = initSpeed;
    }

    protected override void FixedUpdateCharacter()
    {
        // Debug.Log(string.Format("localscal x : {0}", transform.localScale.x));
        if ( !cameraRendered )
        {
            return;
        }
        

        // 점프했는지 검사
        if (jumped)
        {
            // 착지 검사
            if( (grounded && ! groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f) )
            {
                jumped = false;
            }

            if( Time.fixedTime > jumpStartTime + 1.0f)
            {
                rb.gravityScale = gravityScale;
            }
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        // 캐릭터 방향
        transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // 공중에서 피격 시 x 축 고정
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if( stateInfo.fullPathHash == EnemyController.ANISTS_DMG_A ||
            stateInfo.fullPathHash == EnemyController.ANISTS_DMG_B ||
            stateInfo.fullPathHash == EnemyController.ANISTS_Dead )
        {
            speed = 0.0f;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    public bool ActionJump()
    {
        if(jumpActionEnabled && grounded && !jumped)
        {
            animator.SetTrigger("Jump");
            rb.AddForce(jumpPower);
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }

        return jumped;
    }

    public void ActionAttack(string atkname, int damage)
    {
        attackEnabled = true;
        attackDamage = damage;
        animator.SetTrigger(atkname);
    }

    public void ActionDamage()
    {
        int damage = 0;

        if( hp <= 0 )
        {
            return;
        }
        if( superArmor )
        {
            animator.SetTrigger("SuperArmor");
        }

        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

        if( stateInfo.fullPathHash == PlayerController.ANISTS_ATTACK_C )
        {
            damage = 3;

            if( !superArmor || superArmor_jumpAttackDmg )
            {
                animator.SetTrigger("DMG_B");
                jumped = true;
                jumpStartTime = Time.fixedTime;
                AddForceAnimatorVy(1500.0f);
            }
        }
        else
        {
            if( !grounded )
            {
                damage = 2;

                if( !superArmor || superArmor_jumpAttackDmg )
                {
                    jumped = true;
                    jumpStartTime = Time.fixedTime;
                    playerCtrl.rb.AddForce(new Vector2(0.0f, 20.0f));
                }
            }
            else
            {
                damage = 1;

                if( !superArmor )
                {
                    animator.SetTrigger("DMG_A");
                }
            }

            if( SetHp(hp - damage, hpMax) )
            {
                Dead(true);

                int addScoreV = ((int)((float)addScore * (playerCtrl.hp / playerCtrl.hpMax)));
                addScoreV = (int)((float)addScore * (grounded ? 1.0f : 1.5f));
                PlayerController.score += addScoreV;
            }

            playerCtrl.AddCombo();
        }
    }

    public override void Dead(bool gameOver)
    {
        base.Dead(true);
        Destroy(gameObject, 1.0f);
    }

}
