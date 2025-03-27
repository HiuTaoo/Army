using Cainos.PixelArtPlatformer_VillageProps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pickupText; // UI hiển thị nút "F"
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform pickupTextRect; // RectTransform của pickupText để di chuyển

    [Header("Sound")]
    [SerializeField] private AudioClip potionSound;

    [Header("Player")]
    private Health playerHealth;
    private MP playerMP;
    private PlayerAttack playerAttack;

    [Header("Potion")]
    [SerializeField] float HP;
    [SerializeField] float MP;
    [SerializeField] float HPPerSecond;
    [SerializeField] float HealSecond;
    [SerializeField] float MPPerSecond;
    [SerializeField] float RecoverSecond;
    [SerializeField] float Atk;
    [SerializeField] float Atk_Boost;
    [SerializeField] float Boost_time;

    public enum Options
    {
        HP_Instance,
        HP_Overtime,
        MP_Instance,
        MP_OverTime,
        Increase_HP,
        Increase_MP, 
        Increase_Atk,
        Boost
    }

    [SerializeField] private Options selectedOption;

    private void Awake()
    {
        pickupTextRect = pickupText.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Kiểm tra nếu nhấn phím F và nhân vật đang trong phạm vi rương
        if (Input.GetKeyDown(KeyCode.F) && pickupText.activeSelf)
        {
            switch (selectedOption)
            {
                case Options.HP_Instance:
                    playerHealth.Healing(HP);
                    break;
                case Options.HP_Overtime:
                    playerHealth.HealOverTime(HPPerSecond, HealSecond);
                    break;
                case Options.MP_Instance:
                    playerMP.RecoverMP(MP);
                    break;
                case Options.MP_OverTime:
                    playerMP.RecoverManaOverTime(MPPerSecond, RecoverSecond);
                    break;
                case Options.Increase_HP:
                    playerHealth.IncreaseHP(HP);
                    break;
                case Options.Increase_MP:
                    playerMP.IncreaseMP(MP);
                    break;
                case Options.Increase_Atk:
                    playerAttack.IncreasePlayerDamage(Atk);
                    break;
                case Options.Boost:
                    playerAttack.Boost(Atk_Boost, 10);
                    break;
                default:
                    break; 
            }
            pickupText.SetActive(false); 
            gameObject.SetActive(false);
            SoundManager.instance.playSound(potionSound);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm với người chơi
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<Health>();
            playerMP = collision.GetComponent<MP>();
            playerAttack = collision.GetComponent<PlayerAttack>();
            if (playerHealth != null || playerMP != null)
            {
                pickupText.SetActive(true); // Hiển thị nút "F"
                text.SetText("F. Use");

                // Cập nhật vị trí của pickupText lên góc trên của potion
                Vector3 potionPosition = transform.position;
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(potionPosition);

                // Đặt vị trí của pickupText theo tọa độ màn hình
                pickupTextRect.position = screenPosition;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ẩn pickupText khi người chơi rời khỏi phạm vi potion
        if (collision.CompareTag("Player"))
        {
            pickupText.SetActive(false);
        }
    }

    public void UsePotion()
    {
        playerHealth.Healing(HP);
        playerMP.RecoverMP(MP);
    }
}
