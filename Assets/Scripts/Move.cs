using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float moving;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        moving = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(moving * 5.0f, rb.velocity.y);
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Jump");
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 30.0f);
        }
    }

    private void FixedUpdate()
    {
        
    }
}
