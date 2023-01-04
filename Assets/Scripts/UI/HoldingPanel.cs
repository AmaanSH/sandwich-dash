using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoldingPanel : MonoBehaviour
{
    public Image holdingIcon;
    public TMP_Text holdingName;

    public void SetHolding(Sprite sprite, string text)
    {
        holdingIcon.sprite = sprite;
        holdingName.text = text;
        gameObject.SetActive(true);
    }

    public void ClearHolding()
    {
        holdingIcon.sprite = null;
        holdingName.text = "";
        gameObject.SetActive(false);
    }
}
