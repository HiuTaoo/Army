using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject[] enemies;

    [Header("End Game Menu")]
    [SerializeField] private GameObject endGameMenu;

    [Header("Player")]
    [SerializeField] private PlayerMovement player;

    [Header("Audio")]
    [SerializeField] private AudioClip audio;

    private bool isPlay = false;

    private void Update()
    {
        if (AllEnemiesDefeated() && player.getCanEndGame())///
        {
            ShowEndGameMenu();          
            if (!isPlay)
            {
                SoundManager.instance.playSound(audio);
                isPlay = true;
            }
        }
    }

    private bool AllEnemiesDefeated()
    {
       
        foreach (var enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                return false; 
            }
        }
        return true; 
    }

    private void ShowEndGameMenu()
    {
        if (endGameMenu != null && !endGameMenu.activeInHierarchy)
        {
            endGameMenu.SetActive(true); // Hiển thị menu end game
            Debug.Log("All enemies defeated! Showing end game menu.");
       
        }
    }

    

}
