using Cainos.PixelArtPlatformer_VillageProps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject pickupText; // UI hiển thị nút "F"
    private RectTransform pickupTextRect; // RectTransform của pickupText để di chuyển
    private Chest currentChest;

    private void Start()
    {
        pickupText.SetActive(false); // Ẩn UI khi bắt đầu
        pickupTextRect = pickupText.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Kiểm tra nếu nhấn phím F và nhân vật đang trong phạm vi rương
        if (currentChest != null && Input.GetKeyDown(KeyCode.F))
        {
            currentChest.Open(); // Mở rương
            pickupText.SetActive(false); // Ẩn thông báo sau khi mở
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm với rương
        if (collision.CompareTag("Chest"))
        {
            currentChest = collision.GetComponent<Chest>();
            if (currentChest != null && !currentChest.IsOpened)
            {
                pickupText.SetActive(true); // Hiển thị nút "F"

                // Cập nhật vị trí của pickupText lên góc trên của rương
                Vector3 chestPosition = currentChest.transform.position;
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(chestPosition);

                // Đặt vị trí của pickupText theo tọa độ màn hình
                pickupTextRect.position = screenPosition;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ẩn nút "F" khi nhân vật rời khỏi phạm vi rương
        if (collision.CompareTag("Chest"))
        {
            currentChest = null;
            pickupText.SetActive(false);
        }
    }
}
