using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Range Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Attack Cooldown")]
    [SerializeField] private float attackCooldown;

    [Header("Arrow")]
    [SerializeField] private Transform arrowPoint;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject arrowShower;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip arrowSound;
    [SerializeField] private AudioClip beamSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deadSound;

    [Header("Range Parametter")]
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;

    [Header("Collider Parametter")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Enemies")]
    [SerializeField] private Transform[] enemies;

    [Header("Skill MP")]
    [SerializeField] private float burstMP;
    [SerializeField] private float arrowShowerMP;
    [SerializeField] private float defendMP;

    [Header("PlayerMovement")]
    [SerializeField] private PlayerMovement playerMovement;

    Health enemyHealth;
    MP mp;
    Health playerHealth;

    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private Rigidbody2D rb;
    private float playerDamage = 10;
    private float originalPlayerDamage;

    private bool isDebuffed = false;    
    private float debuffEndTime;
    private bool isBoosted = false;       
    private float boostEndTime;

    const String BASIC_ATTACK = "basicAttack";
    const String COMBAT = "combat";
    const String BURST = "burst";
    const String ARROW_SHOWER = "ArrowShower";
    const String DEFEND = "defend";

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        mp = GetComponent<MP>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<Health>();
        playerHealth.enabled = true;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Lấy phím nhấn và kiểm tra bằng switch
        switch (Input.inputString)
        {
            case "q":
                if (cooldownTimer > attackCooldown * 2 && mp.isEnoughMP(burstMP))
                {
                    burst();
                    rb.velocity = Vector3.zero;
                    mp.MPConsumpt(burstMP);
                }
                break;

            case "v":
                if (cooldownTimer > attackCooldown/2 && playerMovement.canAttack())
                {
                    combat();
                    rb.velocity = Vector3.zero;
                }
                
                break;

            case "e":
                if (cooldownTimer > attackCooldown * 1.5 && mp.isEnoughMP(arrowShowerMP))
                {
                    ArrowShower();
                    rb.velocity = Vector3.zero;
                    mp.MPConsumpt(arrowShowerMP);
                }
                break;

            case "c":
                if (mp.isEnoughMP(arrowShowerMP))
                {
                    Defend();
                    rb.velocity = Vector3.zero;
                    mp.MPConsumpt(defendMP);
                }
                break;

            default:
                // Kiểm tra riêng nút chuột trái
                if (Input.GetMouseButton(0))
                {
                    if (cooldownTimer > attackCooldown && playerMovement.canAttack())
                    {
                        basicAttack();
                        rb.velocity = Vector3.zero;
                    }
                }
                break;
        }
    }

    private void AimAndShoot()
    {
        // Lấy vị trí con trỏ chuột trong không gian thế giới (world space)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Tính toán hướng từ player tới vị trí chuột
        Vector3 direction = mousePosition - transform.position;

        // Chỉ giữ lại hướng X và Y (vì game 2D)
        direction.z = 0; // Loại bỏ trục Z
        
        // Xoay nhân vật để hướng về phía con trỏ chuột (tùy thuộc vào cơ chế game)
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);  // Quay nhân vật sang phải
        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Quay nhân vật sang trái

    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance
            , new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);
        if (hit.collider != null)
            enemyHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private bool CanCloseCombat()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range/2 * transform.localScale.x * colliderDistance
            , new Vector3(boxCollider.bounds.size.x * range /2, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);
        if (hit.collider != null)
            enemyHealth = hit.transform.GetComponent<Health>();
        return hit.collider != null;
    }

    private bool CanDefend()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range / 3 * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range / 3, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);

        if (hit.collider != null)
        {
            // Kiểm tra xem đối tượng trúng có phải là đạn (mũi tên hoặc cầu lửa)
            if (hit.collider.CompareTag("Projectile"))
            {
                Destroy(hit.collider.gameObject); // Hủy đối tượng đạn
            }
        }
        return hit.collider != null;
    }

    private void BurstDamage()
    {
        if (PlayerInSight())
        {
            if (enemyHealth.currentHealth > 0)
                enemyHealth.takeDamage(playerDamage * 10);
        }
    }

    private void combat()
    {
        AimAndShoot();
        playerMovement.enabled = false;
        playerMovement.ChangeAnimationState(COMBAT);
        cooldownTimer = 0;
        if (CanCloseCombat())
        {
            if (enemyHealth.currentHealth > 0)
                enemyHealth.takeDamage(playerDamage);
        }
    }

    private void burst()
    {
        AimAndShoot();
        playerMovement.enabled = false;
        playerMovement.ChangeAnimationState(BURST);
        cooldownTimer = 0;
    }

    public void BurstSound()
    {
        SoundManager.instance.playSound(beamSound);
    }

    public void HurtSound()
    {
        SoundManager.instance.playSound(hurtSound);
    }

    private void Defend()
    {
        AimAndShoot();
        playerMovement.enabled = false;
        playerMovement.ChangeAnimationState(DEFEND);
        cooldownTimer = 0;
        
    }

    private void basicAttack()
    {
        AimAndShoot();
        playerMovement.enabled = false;
        playerMovement.ChangeAnimationState(BASIC_ATTACK);
        cooldownTimer = 0;
    }

    private void ArrowShower()
    {
        AimAndShoot();
        playerMovement.enabled = false;
        playerMovement.ChangeAnimationState(ARROW_SHOWER);
        cooldownTimer = 0;
    }

    public void createArrowShower()
    {
        bool enemyInRange = false;

        // Kiểm tra các kẻ địch trong phạm vi từ leftEdge đến rightEdge
        foreach (Transform enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                float enemyPositionX = enemy.position.x;
                if (enemyPositionX >= leftEdge.position.x && enemyPositionX <= rightEdge.position.x)
                {
                    // Xác định vị trí mặt đất của kẻ địch
                    RaycastHit2D groundHit = Physics2D.Raycast(enemy.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

                    Vector3 spawnPosition = enemy.position;

                    // Nếu tìm thấy mặt đất bên dưới kẻ địch, điều chỉnh vị trí spawn theo vị trí mặt đất
                    if (groundHit.collider != null)
                    {
                        spawnPosition.y = groundHit.point.y ; // Đặt vị trí Y bằng vị trí của mặt đất
                    }

                    // Tạo skill tại vị trí đã điều chỉnh theo mặt đất
                    GameObject skillInstance = Instantiate(arrowShower, spawnPosition, Quaternion.identity);

                    skillInstance.SetActive(true);
                    enemyInRange = true;
                    break; // Dừng vòng lặp sau khi tạo một skill
                }
            }
        }

        // Nếu không có kẻ địch trong phạm vi, tạo skill ở leftEdge hoặc rightEdge
        if (!enemyInRange)
        {
            Vector3 spawnPosition = transform.localScale.x > 0 ? rightEdge.position : leftEdge.position;

            // Xác định vị trí mặt đất cho leftEdge hoặc rightEdge
            RaycastHit2D groundHit = Physics2D.Raycast(spawnPosition, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

            // Nếu tìm thấy mặt đất, điều chỉnh vị trí theo mặt đất
            if (groundHit.collider != null)
            {
                spawnPosition.y = groundHit.point.y;
            }

            GameObject skillInstance = Instantiate(arrowShower, spawnPosition, Quaternion.identity);
            skillInstance.SetActive(true);
        }
    }

    private void createArrow()
    {
        GameObject arrowTmp = Instantiate(arrow, arrowPoint.position, Quaternion.identity);
        arrowTmp.transform.position = arrowPoint.position;
        arrowTmp.GetComponent<Projectiles>().SetDirection(Mathf.Sign(transform.localScale.x));
        SoundManager.instance.playSound(arrowSound);
    }
   
    public void EnablePlayerMovement()
    {
        playerMovement.enabled = true;
    }

    public void EnableHealth()
    {
        playerHealth.enabled = true;
    }

    public void DisableHealth()
    {
        playerHealth.enabled = false;
    }

    public float getPlayerDamage()
    {
        return playerDamage;
    }

    public void IncreasePlayerDamage(float _value)
    {
        playerDamage += _value;
    }

    public void setPlayerAttack(float _value)
    {
        playerDamage = _value;
    }

    public void Debuff(float value, float delay)
    {
        // Kiểm tra nếu đang được boost, hủy boost và áp dụng debuff
        if (isBoosted)
        {
            // Khôi phục lại sát thương gốc nếu đang được boost
            playerDamage = originalPlayerDamage;
            isBoosted = false;  // Hủy trạng thái boost
            Debug.Log("Boost canceled due to debuff.");
        }

        if (isDebuffed)
        {
            // Nếu đang bị debuff, chỉ cần reset lại thời gian
            debuffEndTime = Time.time + delay;
        }
        else
        {
            // Nếu chưa bị debuff, bắt đầu debuff và thiết lập biến kiểm tra
            originalPlayerDamage = playerDamage; // Lưu lại giá trị sát thương gốc
            playerDamage = value;                // Áp dụng debuff
            debuffEndTime = Time.time + delay;   // Thiết lập thời gian kết thúc debuff
            isDebuffed = true;
            StartCoroutine(DebuffRoutine());     // Bắt đầu coroutine để quản lý debuff
        }
    }

    private IEnumerator DebuffRoutine()
    {
        while (Time.time < debuffEndTime)
        {
            yield return null; // Chờ cho đến khi thời gian debuff kết thúc
        }
        if (isDebuffed)
        {
            // Khôi phục lại sát thương gốc sau khi debuff kết thúc
            playerDamage = originalPlayerDamage;
            isDebuffed = false; // Đặt lại biến kiểm tra
        }
        
    }

    public void Boost(float value, float delay)
    {
        // Kiểm tra nếu đang bị debuff, hủy debuff và áp dụng boost
        if (isDebuffed)
        {
            // Khôi phục lại sát thương gốc nếu đang bị debuff
            playerDamage = originalPlayerDamage;
            isDebuffed = false;  // Hủy trạng thái debuff
            Debug.Log("Debuff canceled due to boost.");
        }

        if (isBoosted)
        {
            // Nếu đang được boost, chỉ cần reset lại thời gian
            boostEndTime = Time.time + delay;
        }
        else
        {
            // Nếu chưa được boost, bắt đầu boost và thiết lập biến kiểm tra
            originalPlayerDamage = playerDamage; // Lưu lại giá trị sát thương gốc
            playerDamage += value;               // Áp dụng boost
            boostEndTime = Time.time + delay;    // Thiết lập thời gian kết thúc boost
            isBoosted = true;
            StartCoroutine(BoostRoutine());      // Bắt đầu coroutine để quản lý boost
        }
    }

    private IEnumerator BoostRoutine()
    {
        while (Time.time < boostEndTime)
        {
            yield return null; // Chờ cho đến khi thời gian boost kết thúc
        }
        if (isBoosted)
        {
            // Khôi phục lại sát thương gốc sau khi boost kết thúc
            playerDamage = originalPlayerDamage;
            isBoosted = false; // Đặt lại biến kiểm tra
        }
        
    }
    public void playSoundDead()
    {
        SoundManager.instance.playSound(deadSound);
    }


}
