using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanelListing : MonoBehaviour
{
    public TMP_Text orderListingText;
    public Image orderListingImage;

    public IngredientType IngredeintType { get; private set; }

    public void SetText(string text, IngredientType ingredient, Sprite ingredientImage)
    {
        orderListingText.text = text;
        orderListingImage.sprite = ingredientImage;

        IngredeintType = ingredient;

        gameObject.SetActive(true);
    }

    public void MarkComplete()
    {
        orderListingText.fontStyle ^= FontStyles.Strikethrough;
    }
}
