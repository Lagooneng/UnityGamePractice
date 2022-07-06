using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FIREBULLET
{
    ANGLE,
    HOMING,
    HOMING_Z
}

public class FireBullet : MonoBehaviour
{
    // 외부 파라미터, 인스펙터 표시
    public FIREBULLET fireType = FIREBULLET.HOMING;

    public float attackDamage = 1;
    public Vector2 attackNockBackVector;

    public bool penetration = false;

    public float lifeTime = 3.0f;
    public float speedV = 10.0f;
    public float speedA = 0.0f;
    public float angle = 0.0f;
    public float homingTime = 0.0f;
    public float homingAngleV = 180.0f;
    public float homingAngleA = 20.0f;

    public Vector3 bulletScaleV = Vector3.zero;
    public Vector3 bulletScaleA = Vector3.zero;

    public Sprite hiteSprite;
    public Vector3 hitEffectScale = Vector3.one;
    public float rotateVt = 360.0f;

    // 외부 파라미터
    [System.NonSerialized] public Transform owner;
    [System.NonSerialized] public GameObject targetObject;
    [System.NonSerialized] public bool attackEnabled;

    // 내부 파라미터
    float fireTime;
    Vector3 posTarget;
    float homingAngle;
    Quaternion homingRotate;
    float speed;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 주인 검사
        if( !owner )
        {
            return;
        }

        // 초기화
        targetObject = PlayerController.GetGameObject();
        posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

        switch( fireType )
        {
            case FIREBULLET.ANGLE:
                speed = (owner.localScale.x < 0.0f) ? -speedV : +speedV;

                break;
            case FIREBULLET.HOMING:
                speed = speedV;
                homingRotate = Quaternion.LookRotation(posTarget - transform.position);

                break;
            case FIREBULLET.HOMING_Z:
                speed = speedV;

                break;
        }

        fireTime = Time.fixedTime;
        homingAngle = angle;
        attackEnabled = true;
        Destroy(this.gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 주인 검사
        if( !owner )
        {
            return;
        }

        // 자기 자신에게 닿았는지 검사
        if ((other.isTrigger) ||
            (owner.tag == "Player" && other.tag == "PlayerBody") ||
            (owner.tag == "Player" && other.tag == "PlayerArm") ||
            (owner.tag == "Player" && other.tag == "PlayerArmBullet") ||
            (owner.tag == "Enemy" && other.tag == "EnemyBody") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArm") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArmBullet"))
        {
            return;
        }

        // 벽에 닿았는지 검사
        if( !penetration )
        {
            GetComponent<SpriteRenderer>().sprite = hiteSprite;
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            transform.localScale = hitEffectScale;
            Destroy(this.gameObject, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotateVt);
    }

    private void FixedUpdate()
    {
        bool homing = ((Time.fixedTime - fireTime) < homingTime);

        if(homing)
        {
            posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        }

        // 호밍 처리
        switch(fireType)
        {
            case FIREBULLET.ANGLE:
                rb.velocity = Quaternion.Euler(0.0f, 0.0f, angle) * new Vector3(speed, 0.0f, 0.0f);

                break;
            case FIREBULLET.HOMING:
                if(homing)
                {
                    homingRotate = Quaternion.LookRotation(posTarget - transform.position);
                }

                Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
                rb.velocity = Quaternion.Euler(0.0f, 0.0f, angle) * vecMove;

                break;
            case FIREBULLET.HOMING_Z:
                if(homing)
                {
                    float targetAngle = Mathf.Atan2(
                        posTarget.y - transform.position.y,
                        posTarget.x - transform.position.x) * Mathf.Rad2Deg;
                    float deltaAngle = Mathf.DeltaAngle(targetAngle, homingAngle);
                    float deltaHomingAngle = homingAngleV * Time.fixedDeltaTime;

                    if( Mathf.Abs(deltaAngle) >= deltaHomingAngle )
                    {
                        homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
                    }

                    homingAngleV += (homingAngleA * Time.fixedDeltaTime);
                    homingRotate = Quaternion.Euler(0.0f, 0.0f, homingAngle);
                }
                rb.velocity = (homingRotate * Vector3.right) * speed;

                break;
        }

        speed += speedA * Time.fixedDeltaTime;

        transform.localScale += bulletScaleV;
        bulletScaleV += bulletScaleA * Time.fixedDeltaTime;

        if( transform.localScale.x < 0.0f || transform.localScale.y < 0.0f ||
            transform.localScale.z < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
