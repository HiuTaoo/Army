using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private PlayerAttack playerAttack;
    private float direction;
    private bool hit;

    private PolygonCollider2D arrowcollider;
    private Animator anim;

    private Rigidbody2D rb;

    private void Awake()
    {
        arrowcollider = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0 , 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        arrowcollider.enabled = false;
        rb.isKinematic = true;

        anim.SetTrigger("explode");

        if (collision.tag == "Enemy")
                collision.GetComponent<Health>().takeDamage(playerAttack.getPlayerDamage());
    }

    public void SetArrowShower()
    {
        anim.SetTrigger("shower");
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        arrowcollider.enabled = true;

        rb.isKinematic = false;

        float localScaleX = transform.localScale.x;
        if(Math.Sign(localScaleX) != direction)
            localScaleX = -localScaleX;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactive()
    {
        gameObject.SetActive(false);
        
    }
}
