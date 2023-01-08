using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusPanel : MonoBehaviour
{
    private const float WAIT_BETWEEN_MESSAGES = 3f;

    public TMP_Text statusText;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = statusText.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void AddStatusMessage(string message, bool good)
    {
        StopAllCoroutines();
        
        StartCoroutine(UpdateStatusText(message, good));
    }
    
    private IEnumerator UpdateStatusText(string text, bool good)
    {
        statusText.text = text;
        statusText.color = (good) ? Color.green : Color.red;

        yield return ToggleFade(false);
        yield return new WaitForSeconds(WAIT_BETWEEN_MESSAGES);
        yield return ToggleFade(true);

        statusText.text = "";
    }
    
    private IEnumerator ToggleFade(bool fadeOut)
    {
        float time = 0f;
        float duration = 0.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp((fadeOut) ? 1f : 0f, (fadeOut) ? 0f : 1f, time / duration);
            yield return null;
        }
    }
}