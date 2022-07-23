using CloudOnce;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles basic functions of the main menu, 
/// playing the menu theme and loading the level
/// <summary>

public class MainMenu : MonoBehaviour
{
    public GameObject SaveFoundNotification;
    public SaveManager SaveManager;
    /// <summary>
    /// Play the menu theme
    /// </summary>
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioIdentifiers.MenuTheme);
    }

    /// <summary>
    /// Load the level
    /// <summary>
    public void CheckForSave()
    {
        // If no save exists
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "save_data.dat")))
        {
            LoadMap01();
        }
        else
        {
            LoadSaveConfirmation();
        }
    }

    /// <summary>
    /// CAlled via button
    /// </summary>
    public void LoadCurrentSavedArea()
    {
        StartCoroutine(LoadSavedAreaRoutine());
    }
    /// <summary>
    /// Loads the scene that the player has saved to the area progression
    /// </summary>
    /// 
    public IEnumerator LoadSavedAreaRoutine()
    {
        EventManager.Instance.RaiseGameEvent(EventConstants.LOAD_FROM_MAIN_MENU);
        yield return new WaitForSeconds(2.5f);
        SaveManager.LoadSavedScene();
        yield return 0;
    }

    /// <summary>
    /// CAlled via button
    /// </summary>
    public void LoadMap01()
    {
        StartCoroutine(LoadMap01Routine());
    }

    private IEnumerator LoadMap01Routine()
    {        
        // TODO AudioManager.Instance.PlayAudio(AudioIdentifiers.StartGame);
        EventManager.Instance.RaiseGameEvent(EventConstants.LOAD_FROM_MAIN_MENU);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadSceneAsync("Map01");
        yield return 0;
    }

    // Settings 
    public void LoadSaveConfirmation()
    {
        SaveFoundNotification.SetActive(true);
    }

    public void ToggleMuteAll()
    {
        bool toggle = true;
        if (AudioManager.Instance.IsMusicMuted || AudioManager.Instance.IsSoundMuted)
        {
            toggle = false;
        }
        else if (!AudioManager.Instance.IsMusicMuted && !AudioManager.Instance.IsSoundMuted)
        {
            toggle = true;
        }
        AudioManager.Instance.MuteMusic(toggle);
        AudioManager.Instance.MuteSounds(toggle);
    }
    public void MuteMusic()
    {
        bool m_shouldMute = !AudioManager.Instance.IsMusicMuted;
        AudioManager.Instance.MuteMusic(m_shouldMute);
    }
    public void MuteSounds() 
    {
        bool m_shouldMute = !AudioManager.Instance.IsSoundMuted;
        AudioManager.Instance.MuteSounds(m_shouldMute);
    }

    /// <summary>
    /// Play the button press sound effect
    /// </summary>
    public void PlayButtonSound()
    {
        AudioManager.Instance.PlayAudio(AudioIdentifiers.MenuButton);
    }
}
