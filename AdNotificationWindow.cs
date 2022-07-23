using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdNotificationWindow : MonoBehaviour
{
    public GameObject Window;
    public TextMeshProUGUI NormalChipTier;
    public TextMeshProUGUI AdChipTier;
    public void DisableWindow() 
    { 
        Window.SetActive(false); 
    }
    public void EnableWindow() 
    {
        if (NormalChipTier != null && AdChipTier != null)
        {
            NormalChipTier.text = PlayerStats.Instance.ChipEconomy.ToString();
            AdChipTier.text = (PlayerStats.Instance.ChipEconomy + 2).ToString();
        }
        Window.SetActive(true); 
    }
}
