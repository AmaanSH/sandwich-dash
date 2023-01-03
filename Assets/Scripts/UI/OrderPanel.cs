using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Jobs.LowLevel.Unsafe;
using System;

public class OrderPanel : MonoBehaviour
{
    public TMP_Text orderNumber;
    public OrderPanelListing panelListing;
    public Image timeBar;
    public Transform panelHolder;
    
    private OrderPanelListing[] listings;
    private int maxTime = 0;

    public event Action<OrderPanel> OnOrderTimeup;

    public int OrderId { get; private set; }

    public float TimeElapsed { get; private set; }
    public IngredientType JamType { get; private set; }

    private void Update()
    {
        if (maxTime > 0f)
        {
            TimeElapsed += Time.deltaTime;
            timeBar.fillAmount = TimeElapsed / maxTime;

            if (TimeElapsed >= maxTime)
            {
                End();
                maxTime = 0;
            }
        }
    }

    public void End()
    {
        // okay.. the order wasn't completed in time so..
        OnOrderTimeup?.Invoke(this);
    }

    public void Setup(int id, Order order)
    {
        OrderId = id;
        orderNumber.text = "Order #" + id;
        maxTime = order.maxTime;

        listings = new OrderPanelListing[3];

        listings[0] = Instantiate(panelListing, panelHolder);
        listings[1] = Instantiate(panelListing, panelHolder);
        listings[2] = Instantiate(panelListing, panelHolder);
        

        listings[0].SetText("Bread", IngredientType.Bread);
        listings[1].SetText(order.jamName, order.jam);
        listings[2].SetText("Bread", IngredientType.Bread);

        JamType = order.jam;

        gameObject.SetActive(true);
    }

    public void MarkItemComplete(IngredientType ingredient)
    {
        for (int i = 0; i < listings.Length; i++)
        {
            if (listings[i].IngredeintType == ingredient)
            {
                listings[i].MarkComplete();
                break;
            }
        }
    }
}
