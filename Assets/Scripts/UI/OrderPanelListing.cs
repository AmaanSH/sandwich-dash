using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderPanelListing : MonoBehaviour
{
    public TMP_Text orderListingText;

    public IngredientType IngredeintType { get; private set; }

    public void SetText(string text, IngredientType ingredient)
    {
        orderListingText.text = text;

        IngredeintType = ingredient;

        gameObject.SetActive(true);
    }

    public void MarkComplete()
    {
        orderListingText.fontStyle ^= FontStyles.Strikethrough;
    }
}
