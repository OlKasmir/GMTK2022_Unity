using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start() variables
    private Rigidbody2D rb;
    private Animator anim;



    //FSM State Maschine
  
    // private enum State { idle, running, extra, falling, hurt }
   //private State state = State.idle;
    private Collider2D coll;

    // Inspector Variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    // [SerializeField] private float acceleration = 10;


  private Countdown movementBlockTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       // anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    movementBlockTime = new Countdown(0.0f);
    }

    private void Update()
    {
        MovementManager();
        
        // if (state != State.hurt)
        {
           // MovementManager();
        }

       // AnimationState();
       // anim.SetInteger("state", (int)state); //sets Animation based on Enumerator state
    }

   

   

    private void MovementManager()
    {
    

        
        
            float hDirection = Input.GetAxis("Horizontal");

    if (movementBlockTime.Check()) {
      // Moving Left
      if (hDirection < 0) {
        // rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(-speed, rb.velocity.y), Time.deltaTime * acceleration);
        rb.velocity = new Vector2(-speed, rb.velocity.y);
        transform.localScale = new Vector2(-1, 1);
      }

      //Moving Right
      else if (hDirection > 0) {
        // rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(speed, rb.velocity.y), Time.deltaTime * acceleration);
        rb.velocity = new Vector2(+speed, rb.velocity.y);
        transform.localScale = new Vector2(1, 1);
      }
    }

            // Jumping
            if (Input.GetKeyDown(KeyCode.W) && coll.IsTouchingLayers(ground))
            {
                Jump();
            }
            // transform.localScale = new Vector2(-1, 1);
        


    }


    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
       // state = State.extra;
    }
   
    /* private void AnimationState()
    {
        if (state == State.extra)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }

        }

        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            //Moving
            state = State.running;
        }
        else
        {
            state = State.idle;
        }


    }

 */ public void ApplyMovementBlockTime(float seconds) {
    movementBlockTime.Reset(seconds);
  }




}  

    

