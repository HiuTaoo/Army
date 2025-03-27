using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    [Header("Attack Parametter")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;

    [Header("Range Parametter")]
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;

    [Header("Collider Parametter")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol")]
    [SerializeField] private Patrol enemyPatrol;

    private float cooldownTimer = Mathf.Infinity;

    private Animator anim;
    private Health playerHealth;
    private Health enemyHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb= GetComponent<Rigidbody2D>();
        enemyHealth= GetComponent<Health>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown && enemyHealth.currentHealth > 0)
            {
                cooldownTimer = 0;
                anim.SetBool("moving", false);
                anim.SetTrigger("basicAttack");
                rb.velocity = Vector3.zero;
            }
        }
        if(enemyPatrol != null)
        {
           enemyPatrol.enabled = !PlayerInSight();
        }
        
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance
            , new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y,boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);
        if(hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            if(playerHealth != null && playerHealth.enabled)
            {
                if (playerHealth.currentHealth > 0)
                    playerHealth.takeDamage(damage);
            }
        }
    }

    public void EnablePatrol()
    {
        enemyPatrol.enabled = true;
    }

    public void DisablePatrol()
    {
        enemyPatrol.enabled = false;
    }

    public void DestroyEnemy()
    {
        Destroy(enemyPatrol);
    }

    public void Attacking()
    {
        enemyPatrol.SetAttacking(true);
    }

    public void resetAttacking()
    {
        enemyPatrol.SetAttacking(false);
    }
}
