using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Tutorial : MonoBehaviour
{
    private Animator anim;
    float horizontalInput;

    String currentState;
    const String IDLE = "Idle";
    const String JUMP_UP = "Jump_up";
    const String JUMP_DOWN = "jump_down";
    const String RUN = "Run";
    const String BASIC_ATTACK = "BasicAttack";
    const String COMBAT = "Combat";
    const String BURST = "Burst";
    const String ARROW_SHOWER = "ArrowShower";
    const String DEFEND = "Defend";

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ChangeAnimationState(IDLE);
    }

    private void Update()
    {
        move();
        switch (Input.inputString)
        {
            case "q":
                ChangeAnimationState(BURST);
                break;

            case "v":
                ChangeAnimationState(COMBAT);
                break;

            case "e":
                ChangeAnimationState(ARROW_SHOWER);
                break;

            case "c":
                ChangeAnimationState(DEFEND);
                break;
            default:
                if (Input.GetMouseButton(0))
                    ChangeAnimationState(BASIC_ATTACK);
                if (horizontalInput != 0)
                    ChangeAnimationState(RUN);
                if(Input.GetKeyDown(KeyCode.Space))
                    ChangeAnimationState(JUMP_UP);
                break;
        }
    }
    private void move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //flip player 
        if (horizontalInput != 0)
        {
            if (horizontalInput > 0)
                transform.localScale = new Vector3(10, 10, 10);

            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-10, 10, 10);
        }
    }

    public void ChangeAnimationState(String newState)
    {
        if (currentState == newState) return;
        anim.Play(newState);
        currentState = newState;
    }

    public void ResetState()
    {
        ChangeAnimationState(IDLE);
    }

}
