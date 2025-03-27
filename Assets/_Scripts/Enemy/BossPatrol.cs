using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatrol : MonoBehaviour
{
    [Header("Patrol Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private Transform startPoint;

    [Header("Movement parameters")]
    [SerializeField] private float speed;

    [Header("Idle Behavior")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Boss HP Bar")]
    [SerializeField] private GameObject hpbar;

    private bool isReturning = false;
    private BoxCollider2D bossCollider;
    private Boolean attacking = false;



    private void Awake()
    {
        bossCollider = enemy.GetComponent<BoxCollider2D>();
        hpbar.SetActive(false);
    }

    private void Update()
    {
        // Kiểm tra khoảng cách giữa enemy và người chơi
        float playerDistance = Vector2.Distance(enemy.position, player.position);

        // Nếu người chơi ở trong tầm phát hiện
        if (player.position.x > leftEdge.position.x && player.position.x < rightEdge.position.x)
        {
            hpbar.SetActive(true);
            if (!attacking)
            {
                isReturning = false;
                MoveTowardsPlayer();
            }
        }
        else 
        {
            // Nếu người chơi ra khỏi tầm, boss trở về vị trí ban đầu
            ReturnToStartPosition();
            hpbar.SetActive(false);
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - enemy.position).normalized;

        // Xác định hướng và quay boss
        if ((direction.x < 0 && enemy.localScale.x < 0) || (direction.x > 0 && enemy.localScale.x > 0))
        {
            Flip();
            
        }

        if(!attacking)
        {
            // Di chuyển enemy về phía người chơi
            enemy.position = Vector2.MoveTowards(enemy.position, player.position, speed * Time.deltaTime);
            animator.SetBool("moving", true);
        }
    }

    private void ReturnToStartPosition()
    {
        if (!isReturning)
        {
            idleTimer = idleDuration;
            isReturning = true;
        }

        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            animator.SetBool("moving", false);
        }
        else
        {
            // Di chuyển enemy về vị trí ban đầu
            Vector3 direction = (startPoint.position - bossCollider.bounds.center).normalized;

            // Kiểm tra xem boss có cần quay lại không
            if ((direction.x < 0 && enemy.localScale.x < 0) || (direction.x > 0 && enemy.localScale.x > 0))
            {
                Flip();
            }

            // Di chuyển về targetPosition nhưng chỉ khi chưa đạt vị trí
            if (bossCollider.bounds.center.x < startPoint.position.x || bossCollider.bounds.center.x < startPoint.position.x)
            {
                enemy.position = Vector2.MoveTowards(enemy.position, startPoint.position, speed * Time.deltaTime);
                animator.SetBool("moving", true);
            }
            else
            {
                // Khi đạt gần startPoint, dừng di chuyển
                enemy.position = startPoint.position;
                isReturning = false;
                animator.SetBool("moving", false);
                enemy.GetComponent<Health>().Heal();
            }
        }
    }

    private void Flip()
    {
        // Đảo ngược hướng của enemy
        Vector3 scale = enemy.localScale;
        scale.x *= -1;
        enemy.localScale = scale;
    }

    public void SetAttacking(bool att)
    {
        attacking = att;
    }




}
