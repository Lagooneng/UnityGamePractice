using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    // 외부 파라미터
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(+100.0f, -50.0f);
    public bool superArmor = false;
    public bool superArmor_jumpAttackDmg = true;

    public GameObject[] fireObjectList;

    [System.NonSerialized] public float hpMax = 10.0f;
    [System.NonSerialized] public float hp = 10.0f;
    [System.NonSerialized] public float dir = 1.0f;
    [System.NonSerialized] public float speed = 6.0f;
    [System.NonSerialized] public float basScaleX = 1.0f;
    [System.NonSerialized] public bool activeSts = false;
    [System.NonSerialized] public bool jumped = false;
    [System.NonSerialized] public bool grounded = false;
    [System.NonSerialized] public bool groundedPrev = false;

    // 캐시
    [System.NonSerialized] public Animator animator;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    // 내부 파라미터
    protected float speedVx = 0.0f;
    protected float speedVxAddPower = 0.0f;
    protected float gravityScale = 10.0f;
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;
    public Rigidbody2D rb;

    protected bool addForceVxEnabled = false;
    protected float addForceVxStartTime = 0.0f;

    protected bool addVelocityEnabled = false;
    protected float addVelocityVx = 0.0f;
    protected float addVelocityVy = 0.0f;

    protected bool setVelocityVxEnabled = false;
    protected bool setVelocityVyEnabled = false;
    protected float setVelocityVx = 0.0f;
    protected float setVelocityVy = 0.0f;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;
        basScaleX = transform.localScale.x * dir;
        transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

        rb = GetComponent<Rigidbody2D>();

        activeSts = true;
        gravityScale = rb.gravityScale;

    }


    protected virtual void FixedUpdate()
    {

        if (transform.position.y < -30.0f)
        {
            Dead();
        }

        groundedPrev = grounded;
        grounded = false;

        groundCheck_OnRoadObject = null;
        groundCheck_OnMoveObject = null;
        groundCheck_OnEnemyObject = null;

        Collider2D[][] groundCheckCollider = new Collider2D[3][];
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

        foreach (Collider2D[] groundCheckList in groundCheckCollider)
        {
            foreach (Collider2D groundCheck in groundCheckList)
            {
                if (groundCheck != null)
                {
                    if (!groundCheck.isTrigger)
                    {
                        grounded = true;

                        if (groundCheck.tag == "Road")
                        {
                            groundCheck_OnRoadObject = groundCheck.gameObject;
                        }
                        else if (groundCheck.tag == "MoveObject")
                        {
                            groundCheck_OnMoveObject = groundCheck.gameObject;
                        }
                        else if (groundCheck.tag == "Enemy")
                        {
                            groundCheck_OnEnemyObject = groundCheck.gameObject;
                        }
                    }
                }
            }
        }

        FixedUpdateCharacter();

        // 이동 계산
        if( addForceVxEnabled )
        {
            if( Time.fixedTime - addForceVxStartTime > 0.5f )
            {
                addForceVxEnabled = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(speedVx + speedVxAddPower, rb.velocity.y);
        }

        // 최종 Velocity 계산
        if(addVelocityEnabled)
        {
            addVelocityEnabled = false;

            rb.velocity = new Vector2(rb.velocity.x + addVelocityVx, rb.velocity.y + addVelocityVy);
        }

        // 강제로 Velocity 계산
        if(setVelocityVxEnabled)
        {
            setVelocityVxEnabled = false;
            rb.velocity = new Vector2(setVelocityVx, rb.velocity.y);
        }
        if (setVelocityVyEnabled)
        {
            setVelocityVyEnabled = false;
            rb.velocity = new Vector2(rb.velocity.x, setVelocityVy);
        }

        
    }

    protected virtual void FixedUpdateCharacter() { }

    public virtual void ActionMove(float n)
    {
        if (n != 0.0f)
        {
            dir = Mathf.Sign(n);
            speedVx = speed * n;
        }
        else
        {
            speedVx = 0;
            animator.SetTrigger("Idle");
        }
    }

    public void ActionFire()
    {
        Transform goFire = transform.Find("Muzzle");

        foreach( GameObject fireObject in fireObjectList )
        {
            GameObject go = Instantiate(fireObject, goFire.position, Quaternion.identity) as GameObject;
            go.GetComponent<FireBullet>().owner = transform;
        }
    }

    public virtual void Dead()
    {
        if (!activeSts)
        {
            return;
        }

        activeSts = false;
        animator.SetTrigger("Dead");
    }

    public virtual bool SetHp(float _hp, float _hpMax)
    {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }

    // 애니메이션용 코드
    public virtual void AddForceAnimatorVx(float vx)
    {
        if( vx != 0 )
        {
            rb.AddForce(new Vector2(vx * dir, 0.0f));
            addForceVxEnabled = true;
            addForceVxStartTime = Time.fixedTime;
        }
    }

    public virtual void AddForceAnimatorVy(float vy)
    {
        if (vy != 0)
        {
            rb.AddForce(new Vector2(0.0f, vy));
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
    }

    public virtual void AddVelocityVx(float vx)
    {
        addVelocityEnabled = true;
        addVelocityVx = vx * dir;
    }

    public virtual void AddVelocityVy(float vy)
    {
        addVelocityEnabled = true;
        addVelocityVy = vy;
    }

    public virtual void SetVelocityVx(float vx)
    {
        setVelocityVxEnabled = true;
        setVelocityVx = vx * dir;
    }

    public virtual void SetVelocityVy(float vy)
    {
        setVelocityVyEnabled = true;
        setVelocityVy = vy;
    }

    public virtual void SetLightGravity()
    {
        rb.velocity = new Vector2(0.0f, 0.0f);
        rb.gravityScale = 0.1f;
    }

    public void EnableSuperArmor()
    {
        superArmor = true;
    }

    public void DisableSuperArmor()
    {
        superArmor = false;
    }

    public bool ActionLookUp(GameObject go, float near)
    {
        if( Vector3.Distance( transform.position, go.transform.position ) > near )
        {
            dir = (transform.position.x < go.transform.position.x) ? +1.0f : -1.0f;
            return true;
        }
        return false;
    }

    public bool ActionMoveToNear(GameObject go, float near)
    {
        if (Vector3.Distance(transform.position, go.transform.position) > near )
        {
            ActionMove( (transform.position.x < go.transform.position.x) ? +1.0f : -1.0f );
            return true;
        }
        return false;
    }

    public bool ActionMoveToFar(GameObject go, float far)
    {
        if (Vector3.Distance(transform.position, go.transform.position) > far)
        {
            ActionMove((transform.position.x > go.transform.position.x) ? +1.0f : -1.0f);
            return true;
        }
        return false;
    }
}
