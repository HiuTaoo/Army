using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{ 
    public static SoundManager instance {  get; private set; }
    private AudioSource source;

    [Header("Theme")]
    [SerializeField] private AudioSource themeSource;
    [SerializeField] private AudioSource sfxSource;

    private Boolean isThemePaused = false;
    private Boolean isSFXMuted = false;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        themeSource.volume = 0.3f;
    }

    public void playSound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }

    public Boolean PauseTheme()
    {
        if (themeSource.isPlaying)
        {
            themeSource.Pause();
            isThemePaused = true;
        }
        else if (isThemePaused)
        {
            themeSource.UnPause();
            isThemePaused = false;
        }
        return isThemePaused;
    }

    public Boolean MuteSFX()
    {
        isSFXMuted = !isSFXMuted;
        sfxSource.mute = isSFXMuted;
        return isSFXMuted;
    }


    // Phương thức để cập nhật âm lượng của themeSource
    public void SetThemeVolume(float volume)
    {
        themeSource.volume = volume;
    }





}
