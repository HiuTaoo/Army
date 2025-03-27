using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    [SerializeField] private Health playerHealth;
    [SerializeField] private MP playerMP;
    [SerializeField] private PlayerAttack playerAttack;
    [Header("Respawn Audio")]
    [SerializeField] private AudioClip audio;

    private float healthSavePoint;
    private float mpSavePoint;
    private float playerAttackSavePoint;
    private UIManager uiManager;
    [SerializeField]  private float MPCost ;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        // Đặt vị trí hồi sinh cao hơn checkpoint
        Vector3 respawnPosition = currentCheckpoint.position + new Vector3(0, 1f, 0); // Tăng vị trí Y lên 2 đơn vị
        transform.position = respawnPosition;
        playerHealth.HealthRespawn(healthSavePoint);
        playerMP.MPRespawn(mpSavePoint);
        playerAttack.setPlayerAttack(playerAttackSavePoint);
        SoundManager.instance.playSound(audio);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "CheckPoint")
        {
            currentCheckpoint = collision.transform;
            SoundManager.instance.playSound(checkpointSound);
            collision.GetComponent<BoxCollider2D>().enabled = false;
            healthSavePoint = playerHealth.getCurrentHealth();
            mpSavePoint = playerMP.GetCurrentMP();
            playerAttackSavePoint = playerAttack.getPlayerDamage();
        }
    }

    public void CheckRespawn()
    {
        if(currentCheckpoint != null && (playerMP.GetCurrentMP() - MPCost) >= 0)
        {
            mpSavePoint = playerMP.GetCurrentMP() - MPCost;
            Respawn();
        }
        else
        {
            uiManager.GameOver();
        }
    }
}
