using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    [SerializeField] private PlayerAttack playerAttack;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider.enabled = false; // Tắt collider mặc định
    }

    private void Start()
    {
        ActivateSkill(); // Kích hoạt skill ngay khi được tạo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.isKinematic = true;

        if (collision.tag == "Enemy")
            collision.GetComponent<Health>().takeDamage(playerAttack.getPlayerDamage());
    }

    public void ActivateSkill()
    {
        if (anim != null)
        {
            anim.SetTrigger("shower"); // Kích hoạt animation
            boxCollider.enabled = true; // Bật collider khi kỹ năng bắt đầu
            StartCoroutine(DisableColliderAfterDelay()); // Tắt collider sau khi kỹ năng kết thúc
        }
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
}
