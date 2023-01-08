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
    private const int BAD_ORDER_MAX = 10;

    public OrderPanel orderPanel;
    public Transform orderHolder;

    public MusicManager musicManager;

    public CustomerQueue customerQueue;
    public CustomerQueue readyQueue;

    public StatusPanel statusPanel;

    public GameOverPanel gameOverPanel;
    [field: SerializeField] public bool TestSpawn { get; private set; }
    [field: SerializeField] public bool IgnoreEndCondition { get; private set; }

    private List<OrderPanel> activeOrderPanels = new List<OrderPanel>();

    public int ActiveOrders 
    {
        get
        {
            return activeOrderPanels.Count;
        }
    }

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
            float spawnTime = (firstOrder) ? 5f : Mathf.Clamp(20f - (Time.timeSinceLevelLoad / 20f), 10f, 20f);

            if (TestSpawn)
                spawnTime = 1f;

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
                    spot.OnCustomerWaitedTooLong -= OnCustomerWaitedTooLong;
                    spot.OnCustomerWaitedTooLong += OnCustomerWaitedTooLong;
                }
            }
        }
    }

    private void GameOver()
    {
        if (IgnoreEndCondition) { return; }

        customerQueue.Cleanup();
        readyQueue.Cleanup();

        for (int i = 0; i < activeOrderPanels.Count; i++)
        {
            activeOrderPanels[i].OnOrderTimeup -= OnOrderTimeUp;
            
            Destroy(activeOrderPanels[i].gameObject);
        }

        musicManager.StopAllLayers();

        gameOverPanel.SetGameOverScreen(goodOrders, badOrders);

        musicManager.Play("GameOver");
    }

    public void TakeOrder(QueueSpot spot)
    {
        if (spot.Order == null) { return; }

        musicManager.Play("OrderTaken");

        totalOrders++;

        OrderPanel panel = Instantiate(orderPanel, orderHolder);
        panel.Setup(totalOrders, spot.Order);
        panel.OnOrderTimeup += OnOrderTimeUp;

        activeOrderPanels.Add(panel);

        // move customer to the ready queue
        QueueSpot newSpot = readyQueue.SpawnCustomer(spot.Customer);
        newSpot.SetOrderId(panel.OrderId);
        
        Debug.Log($"Registered #{newSpot.OrderId} customer is {newSpot.Customer.name}");

        customerQueue.MarkCustomerInteractedWith();
    }

    public void OnCustomerWaitedTooLong(QueueSpot spot)
    {
        SetBadOrder(true);
    }

    public void OnOrderTimeUp(OrderPanel panel)
    {
        Debug.Log($"Order {panel.OrderId} timed out");

        CompleteOrder(panel.OrderId);
        SetBadOrder(true);

        panel.OnOrderTimeup -= OnOrderTimeUp;
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

        // destroy the game object
        Destroy(orderPanel.gameObject);

        // remove the order panel
        activeOrderPanels.Remove(orderPanel);

        // moving the ready queue up
        StartCoroutine(readyQueue.MarkCustomerInteractedWith(orderId));

    }

    public void SetBadOrder(bool missed = false)
    {
        if (!IsGameOver)
        {
            badOrders++;

            musicManager.Play("OrderRejected");
            musicManager.StopAudioLayer();

            string statusText = (missed) ? "Order missed!" : "Bad order!";
            statusPanel.AddStatusMessage($"{statusText} ({badOrders}/{BAD_ORDER_MAX})", false);

            if (badOrders == BAD_ORDER_MAX && !IgnoreEndCondition)
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

            musicManager.Play("OrderAccepted");
            musicManager.PlayAudioLayer();

            statusPanel.AddStatusMessage($"Good order! ({goodOrders})", true);
        }
    }
}
