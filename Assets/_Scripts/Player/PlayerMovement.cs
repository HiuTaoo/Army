using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    float horizontalInput;
    private Rigidbody2D rb;
    private Animator anim;
    private Boolean grounded;
    private Boolean CanEndGame = false;
    private Health playerHealth;

    private float deathYPosition = -7f;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip jumpSound;

    String currentState;
    const String IDLE = "idle";
    const String JUMP_UP = "jump_up";
    const String JUMP_DOWN = "jump_down";
    const String RUN = "run";

    [Header("Dash")]
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] public TrailRenderer trailRenderer;
    private Boolean canDash = true;
    private Boolean isDashing= false;
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<Health>();  
    }

    private void Update()
    {
        CheckIfPlayerIsDead();
        // Kiểm tra nếu nhấn phím Space để nhảy khi đang trên mặt đất
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            jump();
            SoundManager.instance.playSound(jumpSound);
        }

    }

    private void FixedUpdate()
    {
        
        if (isDashing) 
            return;
        if (playerHealth.IsDead())
        {
            this.enabled = false;
            return;
        }
            
        else
        {
            move();

            if (rb.velocity.y > 0.1f && !grounded) 
            {
                ChangeAnimationState(JUMP_UP);
            }
            else if (rb.velocity.y < -1f && !grounded) 
            {
                ChangeAnimationState(JUMP_DOWN);
            }
            else if (rb.velocity.y == 0 && grounded && horizontalInput == 0) 
            {
                grounded = true;
                ChangeAnimationState(IDLE);
            }
            if (horizontalInput != 0 && grounded)
            {
                ChangeAnimationState(RUN);
            }
            if (Input.GetMouseButton(1) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
        


    }

    public void ChangeAnimationState(String newState)
    {
        if (currentState == newState) return;
        anim.Play(newState);
        currentState = newState;
    }

    private void move()
    {
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        horizontalInput = Input.GetAxis("Horizontal");
        
        //flip player 
        if (horizontalInput != 0)
        {           
            if (horizontalInput > 0)
                transform.localScale = Vector3.one;

            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);            
        }
        
    }


    private void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, speed);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Object")
        {
            grounded = true;
        }
        if (collision.gameObject.tag == "EndGame")
            CanEndGame = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Khi rời khỏi bề mặt tiếp đất, set grounded = false
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Object"))
        {
            grounded = false;
        }
        
    }

    public Boolean canAttack()
    {
        return grounded;
    }

    private void CheckIfPlayerIsDead()
    {
        if (transform.position.y <= deathYPosition)
        {
            Die();
        }
    }

    public void Die()
    {
        playerHealth.SuddenDeath();
        rb.velocity = Vector3.zero;
        
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;  
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public bool getCanEndGame()
    {
        return CanEndGame;
    }



}
