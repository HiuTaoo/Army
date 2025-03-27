using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Boss : MonoBehaviour
{
    [Header("Attack Parametter")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private float spellCooldown;

    [Header("Range Parametter")]
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;

    [Header("Collider Parametter")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol")]
    [SerializeField] private BossPatrol enemyPatrol;

    [Header("Patrol Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spell")]
    [SerializeField] private GameObject spell;

    private float cooldownTimer = Mathf.Infinity;

    private Animator anim;
    Health playerHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetBool("moving", false);
                anim.SetTrigger("basicAttack");
                rb.velocity = Vector3.zero;
            }
        }
        if (PlayerInRangeToSpell())
        {
            if (cooldownTimer >= spellCooldown)
            {
                cooldownTimer = 0;
                anim.SetBool("moving", false);
                rb.velocity = Vector3.zero;
                anim.SetTrigger("spell");
            }
        }
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();    
        }

    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance
            , new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);
        if (hit.collider != null)
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
            if (playerHealth != null && playerHealth.enabled)
            {
                if (playerHealth.currentHealth > 0)
                    playerHealth.takeDamage(damage);
            }
        }
    }

    public void CreateSpell()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(player.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        Vector3 spawnPosition = player.position;

        if (groundHit.collider != null)
        {
            spawnPosition.y = groundHit.point.y; 
        }

        GameObject skillInstance = Instantiate(spell, spawnPosition, Quaternion.identity);

        skillInstance.SetActive(true);
    }

    public Boolean PlayerInRangeToSpell()
    {
        // Điều kiện kiểm tra trục X
        bool inRangeX = player.position.x > leftEdge.position.x && player.position.x < rightEdge.position.x;

        // Điều kiện kiểm tra trục Y
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);
        bool inRangeY = distanceY <= 1f; // Thay `someDistance` bằng khoảng cách cho phép trên trục Y.

        // Chỉ trả về true khi cả hai điều kiện đều thỏa mãn
        return inRangeX && inRangeY;
    }


    public void EnablePatrol()
    {
        enemyPatrol.enabled = true;
    }

    public void DisablePatrol()
    {
        enemyPatrol.enabled = false;
    }

    public void Attacking()
    {
        enemyPatrol.SetAttacking(true);
    }

    public void resetAttacking()
    {
        enemyPatrol.SetAttacking(false);
    }

    public void DestroyEnemy()
    {
        Destroy(enemyPatrol);
    }

    public void DisableCollider()
    {
        boxCollider.enabled = false;
    } 
}
