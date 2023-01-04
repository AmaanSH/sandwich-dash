using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusString
{
    public string text;
    public bool good;
}

public class StatusPanel : MonoBehaviour
{
    private const float WAIT_BETWEEN_MESSAGES = 3f;

    public TMP_Text statusText;

    private List<StatusString> statusMessages = new List<StatusString>();

    private void Start()
    {
        StartCoroutine(UpdateStatusText());
    }

    public void AddStatusMessage(string message, bool good)
    {
        StatusString status = new StatusString();
        status.text = message;
        status.good = good;

        statusMessages.Add(status);
    }

    private IEnumerator UpdateStatusText()
    {
        while (true)
        {
            if (statusMessages.Count > 0)
            {
                statusText.text = statusMessages[0].text;
                statusText.color = (statusMessages[0].good) ? Color.green : Color.red;

                statusMessages.RemoveAt(0);

                yield return new WaitForSeconds(WAIT_BETWEEN_MESSAGES);

                statusText.text = "";
            }
            else
            {
                statusText.text = "";
            }

            yield return 0;
        }
    }
}