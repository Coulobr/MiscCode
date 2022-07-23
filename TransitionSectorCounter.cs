using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSectorCounter : Counter
{
    public const string PREFIX_TEXT = "SECTOR: ";

    public int Count { get { return AreaProgression.Instance.CurrentArea; } }
    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(AnimateText());
        }
    }
    private void OnEnable() => UpdateUI();    
    
    public override void UpdateUI() => DisplayedText.text = PREFIX_TEXT + (this.Count + 1);
  
    public IEnumerator AnimateText()
    {
        //Enable all text in load screen after the delay      
        yield return new WaitForSeconds(1.5f);
 
        DisplayedText.DOFade(1, 1f);
        DisplayedText.transform.DOScale(1, 1f).From(1.25f);

        UpdateUI();
        yield return null;
    }

    public override void Increment()
    {

    }

    public override void Decrement()
    {
        
    }
}
