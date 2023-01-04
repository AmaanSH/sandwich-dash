using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
    public IngredientType jam;
    public string jamName;
    public int maxTime;
}

public class GameManager : MonoBehaviour
{
    private const int BAD_ORDER_MAX = 5;
    
    public OrderPanel orderPanel;
    public Transform orderHolder;

    public CustomerQueue customerQueue;
    public CustomerQueue readyQueue;

    public StatusPanel statusPanel;
    
    public GameOverPanel gameOverPanel;
    public int ActiveOrders { get; private set; }

    private List<OrderPanel> activeOrderPanels = new List<OrderPanel>();

    private int totalOrders = 0;
    private int goodOrders = 0;
    private int badOrders = 0;
    
    public bool IsGameOver { get; private set; }
    private bool firstOrder = true;

    public event Action onGameOverEvent;

    private void Start()
    {
        StartCoroutine(GenerateOrders());

        onGameOverEvent += GameOver;
    }

    public IEnumerator GenerateOrders()
    {
        while (!IsGameOver)
        {
            // pick up the pace with spawning new customers the longer the game has been running for
            float spawnTime = (firstOrder) ? 5f : Mathf.Clamp(20f - (Time.timeSinceLevelLoad / 10f), 10f, 20f);

            if (firstOrder)
            {
                firstOrder = false;
            }

            yield return new WaitForSeconds(spawnTime);

            if (customerQueue.QueueFull() || customerQueue.QueueMovingUp)
            {
                yield return null;
            }
            else
            {
                QueueSpot spot = customerQueue.SpawnCustomer();
                if (spot)
                {
                    spot.OnCustomerWaitedTooLong += OnCustomerWaitedTooLong;
                }
            }
        }
    }

    private void GameOver()
    {
        // TODO: display a screen with total orders we took, total good, total bad
        // TODO: show a star rating (?)
        Debug.Log("Game Over!");
        Debug.Log($"Total good orders {goodOrders} Total bad orders {badOrders}");

        customerQueue.Cleanup();

        for (int i = 0; i < activeOrderPanels.Count; i++)
        {
            activeOrderPanels[i].OnOrderTimeup -= OnOrderTimeUp;
            
            Destroy(activeOrderPanels[i].gameObject);
        }

        gameOverPanel.SetGameOverScreen(goodOrders, badOrders);
    }

    public void CreateOrder(QueueSpot spot)
    {
        if (spot.Order == null) { return; }

        totalOrders++;

        OrderPanel panel = Instantiate(orderPanel, orderHolder);
        panel.Setup(totalOrders, spot.Order);

        activeOrderPanels.Add(panel);
        ActiveOrders++;

        panel.OnOrderTimeup += OnOrderTimeUp;

        // move customer to the ready queue
        QueueSpot newSpot = readyQueue.SpawnCustomer(spot.Customer);
        panel.SetCustomer(newSpot.Customer);

        customerQueue.MarkCustomerInteractedWith();
    }

    public void OnCustomerWaitedTooLong(QueueSpot spot)
    {
        SetBadOrder(true);

        readyQueue.MarkCustomerInteractedWith();
        
        spot.OnCustomerWaitedTooLong -= OnCustomerWaitedTooLong;
    }

    public void OnOrderTimeUp(OrderPanel panel)
    {
        Debug.Log($"Order {panel.OrderId} timed out");

        CompleteOrder(panel.OrderId);
        SetBadOrder(true);

        statusPanel.AddStatusMessage($"Order missed! ({badOrders}/{BAD_ORDER_MAX})", false);
    }

    public void MarkItemComplete(int orderId, IngredientType ingredient)
    {
        // find the order panel
        OrderPanel orderPanel = activeOrderPanels.Find(x => x.OrderId == orderId);

        // mark the item complete
        orderPanel.MarkItemComplete(ingredient);
    }

    public OrderPanel FindOrderWithJam(IngredientType jam)
    {
        return activeOrderPanels.Find(x => x.JamType == jam);
    }

    public void CompleteOrder(int orderId)
    {
        // find the order panel
        OrderPanel orderPanel = activeOrderPanels.Find(x => x.OrderId == orderId);

        // remove the order panel
        activeOrderPanels.Remove(orderPanel);

        readyQueue.MarkCustomerInteractedWith(orderPanel.Customer);

        ActiveOrders--;

        // destroy the game object
        Destroy(orderPanel.gameObject);
    }

    public void SetBadOrder(bool missed = false)
    {
        if (!IsGameOver)
        {
            badOrders++;

            string statusText = (missed) ? "Order missed!" : "Bad order!";
            statusPanel.AddStatusMessage($"{statusText} ({badOrders}/{BAD_ORDER_MAX})", false);

            if (badOrders >= BAD_ORDER_MAX)
            {
                IsGameOver = true;
                onGameOverEvent?.Invoke();
            }
        }
    }

    public void SetGoodOrder()
    {
        if (!IsGameOver)
        {
            goodOrders++;

            statusPanel.AddStatusMessage($"Good order! ({goodOrders})", true);
        }
    }
}
