using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTrigger_EventObject : MonoBehaviour
{
    // 외부 파라미터, 인스펙터 표시

    [System.Serializable] public class Rigidbody2DParam
    {
        public bool enabled = true;
        public float mass = 1.0f;
        public float linearDrag = 0.0f;
        public float angularDrag = 0.05f;
        public float gravityScale = 1.0f;
        public bool fixedAngle = false;
        public bool isKinetic = false;
        public RigidbodyInterpolation2D interpolation = RigidbodyInterpolation2D.None;
        public RigidbodySleepMode2D sleepingMode = RigidbodySleepMode2D.StartAwake;
        public CollisionDetectionMode2D collisionDetection = CollisionDetectionMode2D.Discrete;

        [Header("-----------------")]
        public Vector2 centerOfMass = new Vector2(0.0f, 0.0f);
        public Vector2 velovity = new Vector2(0.0f, 0.0f);
        public float angularVelocity = 0.0f;

        [Header("-----------------")]
        public bool addForceEnabled = false;
        public Vector2 addForcePower = new Vector2(0.0f, 0.0f);

        public bool addForceAtPositionEnabled = false;
        public GameObject addForceAtPositionObject;
        public Vector2 addForceAtPositionPower = new Vector2(0.0f, 0.0f);

        public bool addRelativeForceEnabled = false;
        public Vector2 addRelativeForcePower = new Vector2(0.0f, 0.0f);

        public bool addTorqueEnabled = false;
        public float addTorquePower = 0.0f;

        public bool movePositionEnabled = false;
        public Vector2 movePosition = new Vector2(0.0f, 0.0f);

        public bool moveRotationEnabled = false;
        public float moveRotation = 0.0f;
    }

    public float runTime = 0.0f;
    public float destroyTime = 0.0f;

    [Space(10)]
    public bool sendMessageObjectEnabled = false;
    public string sendMessageString = "OnTriggerEnter2D_PlayerEvect";
    public GameObject[] sendMessageObjectList;
    public bool instantiateGameObjectEnabled = false;
    public GameObject[] instantiateGameObjectList;

    [Space(10)]
    public Rigidbody2DParam rigidbody2DParam;

    // 외부 파라미터
    [System.NonSerialized] public bool triggerOn = false;
    [System.NonSerialized] private Rigidbody2D rb;

    // 코드
    private void OnTriggerEnter2D_PlayerEvent(GameObject go)
    {
        Invoke("runTriggerWork", runTime);
    }

    void runTriggerWork()
    {
        if( rigidbody2DParam.enabled )
        {
            if( gameObject.GetComponent<Rigidbody2D>() == null )
            {
                gameObject.AddComponent<Rigidbody2D>();
            }
            rb = gameObject.GetComponent<Rigidbody2D>();

            rb.mass = rigidbody2DParam.mass;
            rb.drag = rigidbody2DParam.linearDrag;
            rb.angularDrag = rigidbody2DParam.angularDrag;
            rb.gravityScale = rigidbody2DParam.gravityScale;
            rb.freezeRotation = rigidbody2DParam.fixedAngle;
            rb.isKinematic = rigidbody2DParam.isKinetic;
            rb.interpolation = rigidbody2DParam.interpolation;
            rb.sleepMode = rigidbody2DParam.sleepingMode;
            rb.collisionDetectionMode = rigidbody2DParam.collisionDetection;

            rb.centerOfMass = rigidbody2DParam.centerOfMass;
            rb.velocity = rigidbody2DParam.velovity;
            rb.angularVelocity = rigidbody2DParam.angularVelocity;

            if( rigidbody2DParam.addForceEnabled )
            {
                rb.AddForce(rigidbody2DParam.addForcePower);
            }

            if( rigidbody2DParam.addForceAtPositionEnabled )
            {
                rb.AddForceAtPosition(rigidbody2DParam.addForceAtPositionPower,
                    rigidbody2DParam.addForceAtPositionObject.transform.position);
            }

            if( rigidbody2DParam.addRelativeForceEnabled )
            {
                rb.AddRelativeForce(rigidbody2DParam.addRelativeForcePower);
            }

            if( rigidbody2DParam.addTorqueEnabled )
            {
                rb.AddTorque(rigidbody2DParam.addTorquePower);
            }

            if( rigidbody2DParam.movePositionEnabled )
            {
                rb.MovePosition(rigidbody2DParam.movePosition);
            }

            if( rigidbody2DParam.moveRotationEnabled )
            {
                rb.MoveRotation(rigidbody2DParam.moveRotation);
            }
        }

        if( sendMessageObjectEnabled && sendMessageObjectList != null )
        {
            foreach( GameObject go in sendMessageObjectList )
            {
                go.SendMessage(sendMessageString, gameObject);
            }
        }

        if( instantiateGameObjectEnabled && instantiateGameObjectList != null )
        {
            foreach( GameObject go in instantiateGameObjectList )
            {
                Instantiate(go);
            }
        }

        if( destroyTime > 0.0f )
        {
            Destroy(gameObject, destroyTime);
        }

        triggerOn = true;
    }
}
