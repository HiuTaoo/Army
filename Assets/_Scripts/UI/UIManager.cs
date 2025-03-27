using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause Game")]
    [SerializeField] private GameObject pauseScreen;

    [Header("GameScreen")]
    [SerializeField] private GameObject gameScreen;

    [Header("Select Level")]
    [SerializeField] private GameObject levelSelect;

    [Header("Sound")]
    [SerializeField] private AudioClip themeSound;
    [SerializeField] private AudioClip sfxSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private Image ThemeButton;
    [SerializeField] private Image SFXButton;

    [SerializeField] private Scrollbar Scrollbar;

    private Boolean Paused = false, Muted = false;

    private void Awake()
    {
        if(gameOverScreen != null)
            gameOverScreen.SetActive(false);
        if(pauseScreen != null)
            pauseScreen.SetActive(false);
        if (levelSelect != null)
            levelSelect.SetActive(false);

        if (Scrollbar != null)
            Scrollbar.onValueChanged.AddListener(UpdateThemeVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
            {
                PauseGame(false);
            }
            else PauseGame(true);
        }
    }

    #region MENU
    public void StartGame()
    {
        if(levelSelect != null)
            levelSelect.SetActive(true);
    }
    public void OpenTutorial()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }
    public void Level1()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1.0f;
    }
    public void Level2()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 1.0f;
    }
    public void Level3()
    {
        SceneManager.LoadScene(4);
        Time.timeScale = 1.0f;
    }
    public void Level4()
    {
        SceneManager.LoadScene(5);
        Time.timeScale = 1.0f;
    }

    #endregion
    #region Game Over
    public void GameOver()
    {
        if(gameScreen != null)
            gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        SoundManager.instance.playSound(gameOverSound);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

    public void GameQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

   
    #region Pause
    private void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        if (gameScreen != null)
            gameScreen.SetActive(!status);
        if(status)
            Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void UnPause()
    {
        PauseGame(false);
    }

    public void PauseTheme()
    {
        Paused = SoundManager.instance.PauseTheme();
        if(Paused)
            ThemeButton.rectTransform.localScale = new Vector3(-1, 1, 1);
        else if(!Paused)
            ThemeButton.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void MuteSFX()
    {
        Muted = SoundManager.instance.MuteSFX();
        if (Muted)
            SFXButton.rectTransform.localScale = new Vector3(-1, 1, 1);
        else if (!Muted)
            SFXButton.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    // Hàm điều chỉnh âm lượng nhạc nền khi thay đổi giá trị Scrollbar
    public void UpdateThemeVolume(float value)
    {
        SoundManager.instance.SetThemeVolume(value); // Cập nhật âm lượng nhạc nền trong SoundManager
    }


    #endregion
}
