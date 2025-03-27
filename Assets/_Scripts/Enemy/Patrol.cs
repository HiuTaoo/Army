using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header ("Patrol Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;

    [Header("Idle Behavior")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("EnemyHealth")]

    private Health enemyHealth;
    private Vector3 initScale;
    private bool movingLeft;
    private Goblin enemyController;
    private Boolean attacking = false;
    

    private void Awake()
    {
        initScale = enemy.localScale;
        enemyController = enemy.GetComponent<Goblin>();
        enemyHealth = enemy.GetComponent<Health>();
    }

    private void OnDisable()
    {
        animator.SetBool("moving", true);
    }

    private void Update()
    {
        // Kiểm tra nếu player ở trong vùng từ leftEdge đến rightEdge và trên cùng trục Y (hoặc gần trên cùng)
        float yDifference = Mathf.Abs(player.position.y - enemy.position.y);

        if (player.position.x >= leftEdge.position.x && player.position.x <= rightEdge.position.x && yDifference <= 0.5f) // Thay đổi 0.5f theo độ cao bạn mong muốn
        {
                // Nếu player ở bên trái enemy, di chuyển trái; nếu bên phải, di chuyển phải
                int directionToPlayer = player.position.x < enemy.position.x ? -1 : 1;
                MoveInDirection(directionToPlayer);
                movingLeft = directionToPlayer == -1;
        }
        else
        {
                
                // Nếu không di chuyển theo người chơi, thực hiện tuần tra bình thường
                if (movingLeft)
                {
                    if (enemy.position.x >= leftEdge.position.x)
                        MoveInDirection(-1);
                    else
                    {
                        ChangeDirection();
                        enemyHealth.Heal();
                    }
                        
                }
                else
                {
                    if (enemy.position.x <= rightEdge.position.x)
                        MoveInDirection(1);
                    else
                    {
                        ChangeDirection();
                        enemyHealth.Heal();
                    }
            }
        }
    }



    private void MoveInDirection(int _direction)
    {
            idleTimer = 0;
            animator.SetBool("moving", true);
            //make enemy face direction
            enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
                initScale.y, initScale.z);
            //move in that direction 
            enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed, enemy.position.y, enemy.position.z);
    }

    private void ChangeDirection()
    {
        animator.SetBool("moving", false);

        idleTimer += Time.deltaTime;

        if(idleTimer > idleDuration )
            movingLeft = !movingLeft;
    }

    public void SetAttacking(bool att)
    {
        attacking = att;
    }



}
