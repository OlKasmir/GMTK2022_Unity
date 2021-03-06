using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  // Start() variables
  private Rigidbody2D rb;
  private Animator anim;



  //FSM State Maschine

  // private enum State { idle, running, extra, falling, hurt }
  //private State state = State.idle;
  private Collider2D coll;
  [SerializeField]
  private Collider2D groundCheckCollider;

  // Inspector Variables
  [SerializeField] private LayerMask ground;
  [SerializeField] private float speed = 5f;
  [SerializeField] private float jumpForce = 10f;

  [SerializeField]
  private float _maxHangTime = 0.1f;
  [SerializeField]
  private float _jumpBufferTime = 0.1f;

  // [SerializeField] private float acceleration = 10;


  private Countdown movementBlockTime;
  private Stopwatch hangCountdown;
  private Stopwatch jumpBufferCountdown;

  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    // anim = GetComponent<Animator>();
    coll = GetComponent<Collider2D>();
    movementBlockTime = new Countdown(0.0f);
    InitAndResetCountdowns();
  }

#if UNITY_EDITOR
  private void OnValidate() {

  }
#endif

  private void InitAndResetCountdowns() {
    hangCountdown = new Stopwatch();
    jumpBufferCountdown = new Stopwatch();
    jumpBufferCountdown.Elapsed = _jumpBufferTime;
  }

  private void Update() {
    MovementManager();

    // if (state != State.hurt)
    {
      // MovementManager();
    }

    // AnimationState();
    // anim.SetInteger("state", (int)state); //sets Animation based on Enumerator state
  }





  private void MovementManager() {


    bool grounded = groundCheckCollider.IsTouchingLayers(ground);

    if (grounded) {
      hangCountdown.Reset();
      // Debug.Log("Resetting");
    }

    float hDirection = Input.GetAxis("Horizontal");

    if (movementBlockTime.Check()) {
      rb.velocity = new Vector2(hDirection * speed, rb.velocity.y); //  * Time.deltaTime

      // Moving Left
      if (hDirection < 0) {
        //rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(-speed, rb.velocity.y), Time.deltaTime * acceleration);
        //rb.AddForce(Vector2.left * speed * Time.deltaTime);
        //rb.velocity = new Vector2(-speed * Time.deltaTime, rb.velocity.y);
        transform.localScale = new Vector2(-1, 1);
      }

      //Moving Right
      else if (hDirection > 0) {
        //rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(speed, rb.velocity.y), Time.deltaTime * acceleration);
        //rb.AddForce(Vector2.right * speed * Time.deltaTime);
        //rb.velocity = new Vector2(+speed * Time.deltaTime, rb.velocity.y);
        transform.localScale = new Vector2(1, 1);
      }
    }

    // Buffered Jump
    if(grounded && jumpBufferCountdown.Elapsed < _jumpBufferTime) {
      // Debug.Log($"Elapsed: {jumpBufferCountdown.Elapsed}");

      Jump();
    }

    // Jumping
    if (Input.GetKeyDown(KeyCode.W)) {
      jumpBufferCountdown.Reset();

      if (grounded || hangCountdown.Elapsed < _maxHangTime) {
        Jump();
      }
    }

    // Stopping Jumping
    if(Input.GetKeyUp(KeyCode.W) && rb.velocity.y > 0) {
      StopJump();
    }

    // transform.localScale = new Vector2(-1, 1);
  }


  private void Jump() {
    // Debug.Log("Jumping");

    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    // state = State.extra;
    hangCountdown.Elapsed = _maxHangTime;
    jumpBufferCountdown.Elapsed = _jumpBufferTime;
  }

  private void StopJump() {
    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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

*/
  public void ApplyMovementBlockTime(float seconds) {
    movementBlockTime.Reset(seconds);
  }




}



