using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Controls behavior for the in-game pause menu
/// </summary>

public class PauseMenu : MonoBehaviour
{
    [Tooltip("The pause menu object in the scene")]
    public GameObject pauseMenu;
    [Tooltip("The settings menu object in the scene")]
    public GameObject SettingsMenu;
    [Tooltip("Whether or not the game is paused")]
    public static bool isPaused = false;

    /// <summary>
    /// Unpauses game and loads main menu
    /// </summary>
    public void ShowMainMenu()
    {
        DOTween.KillAll();
        Time.timeScale = 1f;
        isPaused = false;
        // TODO AudioManager.Instance.PlayAudio(AudioIdentifiers.QuitToMenu);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Displays the settings menu
    /// </summary>
    public void ToggleSettingsMenu(bool toggle)
    {
        if (toggle) 
        {
            transform.GetChild(0).gameObject.SetActive(false);
            SettingsMenu.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            SettingsMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Pauses game and shows pause menu
    /// </summary>
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        ToggleSettingsMenu(false);
        Time.timeScale = 0f;
        isPaused = true;
        EventManager.Instance.RaiseGameEvent(EventConstants.PAUSE);
    }

    public void ToggleMuteAll()
    {
        bool toggle = true;
        if(AudioManager.Instance.IsMusicMuted || AudioManager.Instance.IsSoundMuted)
        {
            toggle = false;
        }
        else if(!AudioManager.Instance.IsMusicMuted && !AudioManager.Instance.IsSoundMuted)
        {
            toggle = true;
        }
        AudioManager.Instance.MuteMusic(toggle);
        AudioManager.Instance.MuteSounds(toggle);
    }

    public void SfxSlider(Slider slider)
    {
        AudioManager.Instance.SetSoundVolume(slider);
    }
    public void MusicSlider(Slider slider)
    {
        AudioManager.Instance.SetMusicVolume(slider);
    }

    /// <summary>
    /// Unpauses game and hides pause menu
    /// </summary>
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        EventManager.Instance.RaiseGameEvent(EventConstants.UNPAUSE);
    }

    /// <summary>
    /// Toggles whether the game is paused
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Plays the button press sound effect
    /// </summary>
    public void PlayButtonSound()
    {
        AudioManager.Instance.PlayAudio(AudioIdentifiers.MenuButton);
    }
}
