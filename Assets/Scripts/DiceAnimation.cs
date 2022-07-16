using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private DiceRollMechanicSimple diceRollMechanicSimple;
    private int currentSide => diceRollMechanicSimple.GetCurrentSide();
    private enum State { idle, running, extra }
    private enum DiceState {boost, sticky, attack, stamp, dash, shield }
    
    private State state = State.idle;
    private DiceState diceFace = DiceState.boost;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        diceRollMechanicSimple = GetComponent<DiceRollMechanicSimple>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationState();
        anim.SetInteger("state", (int)state); //sets Animation based on Enumerator state

        DiceSideState();
        anim.SetInteger("diceFace", (int)diceFace);
    }

    private void AnimationState()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            state = State.extra;
        }
        
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            state = State.idle;
        }
        
        else if (Mathf.Abs(rb.velocity.x) > 2f && state != State.extra)
        {
            state = State.running;
        }
        
        else
        {
            state = State.idle;
        }
    }

    private void DiceSideState()
    {
        if (currentSide == 1)
        {
            diceFace = DiceState.boost; // 0
        }
        
        else if (currentSide == 2)
        {
            diceFace = DiceState.sticky; // 1
        }
        
        else if (currentSide == 3)
        {
            diceFace = DiceState.attack; // 2
        }
        
        else if (currentSide == 4)
        {
            diceFace = DiceState.stamp; // 3
        }
        
        else if (currentSide == 5)
        {
            diceFace = DiceState.dash; // 4
        }
        
        else if (currentSide == 6)
        {
            diceFace = DiceState.shield; // 5
        }
    }   

}
