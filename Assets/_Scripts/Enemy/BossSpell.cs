using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpell : MonoBehaviour
{
    private Animator anim;
    
    [Header("Boss Spell")]
    [SerializeField] private float damage;
    [SerializeField] private AudioClip CastAudio;

    [Header("Range Parametter")]
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerAttack playerAttack;



    private bool canTakeDmg = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider.enabled = false; // Tắt collider mặc định
    }
    private void Start()
    {
        //CanDamage();
        ActivateSkill(); // Kích hoạt skill ngay khi được tạo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range / 2 * transform.localScale.x * colliderDistance
            , new Vector3(boxCollider.bounds.size.x * range / 2, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);
        if (hit.collider != null && collision.tag == "Player")
        {
            playerHealth = hit.transform.GetComponent<Health>();
            playerAttack = hit.transform.GetComponent<PlayerAttack>();
            canTakeDmg = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset canTakeDmg when the player leaves the collision area
        if (collision.CompareTag("Player"))
        {
            canTakeDmg = false;
        }
    }

    public void ActivateSkill()
    {
        if (anim != null)
        {
            anim.SetTrigger("deadHand"); // Kích hoạt animation
            SoundManager.instance.playSound(CastAudio);
            boxCollider.enabled = true; // Bật collider khi kỹ năng bắt đầu
            StartCoroutine(DisableColliderAfterDelay()); // Tắt collider sau khi kỹ năng kết thúc
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private IEnumerator DisableColliderAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // Điều chỉnh thời gian cho phù hợp
        boxCollider.enabled = false; // Tắt collider sau khi thời gian trễ kết thúc
    }

    private void Deactive()
    {
        gameObject.SetActive(false);

    }

    public void takeDamage()
    {
        if (playerHealth != null && playerHealth.enabled && canTakeDmg)
        {
            if (playerHealth.currentHealth > 0)
            {
                playerHealth.takeDamage(damage);
                playerAttack.Debuff(playerAttack.getPlayerDamage() / 2, 10);
            }
                
        }
 
    }
}
