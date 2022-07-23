using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEnemies : MonoBehaviour
{
    public GameObject MainMenuEnemies;

    public void SwitchEnemiesOff()
    {
        MainMenuEnemies.SetActive(true);
    }
}
