using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{

    public float delay = 0.1f;
    private string fullText;
    private string currentText = "";

    private void Start()
    {
       TriggerDialogueHandler.Instance.OnAdvanceConversation += CallShowText;
    }
    private void OnEnable()
    {
        fullText = GetComponent<TextMeshProUGUI>().text;
        StartCoroutine(ShowText());
    }

    public void CallShowText()
    {
        if (enabled)
        {
            StartCoroutine(ShowText());
        }
    }
    IEnumerator ShowText()
    {
        //if(fullText.Length > 120) 
        //{
        //    GetComponent<TextMeshProUGUI>().fontSize *= 0.9f;
        //}else if (fullText.Length > 140)
        //{
        //    GetComponent<TextMeshProUGUI>().fontSize *= 0.8f;

        //}else if(fullText.Length > 160)
        //{
        //    GetComponent<TextMeshProUGUI>().fontSize *= 0.7f;
        //}

        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i+1);
            GetComponent<TextMeshProUGUI>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
        currentText = "";
        fullText = "";
        StopAllCoroutines();
    }
}
