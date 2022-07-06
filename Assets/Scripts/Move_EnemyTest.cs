using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_EnemyTest : MonoBehaviour
{
    private void FixedUpdate()
    {
        Animator a = GetComponent<Animator>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        a.Play("Enemy_Jump");
        rb.AddForce(new Vector2(0.0f, 100.0f));
    }
}
