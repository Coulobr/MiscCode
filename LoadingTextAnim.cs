using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTextAnim : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(TextAnim());
    }

    private IEnumerator TextAnim()
    {
        yield return new WaitUntil(()=> text.enabled == true);
        while (text.color.a > 0.05)
        {
            text.text = "Loading";
            yield return new WaitForSeconds(.25f);
            text.text = "Loading.";
            yield return new WaitForSeconds(.25f);
            text.text = "Loading..";
            yield return new WaitForSeconds(.25f);
            text.text = "Loading...";
            yield return new WaitForSeconds(.25f);
        }
        yield return 0;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
