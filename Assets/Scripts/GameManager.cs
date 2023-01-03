using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class Order
{
    public IngredientType jam;
    public string jamName;
    public int maxTime;
}

public class GameManager : MonoBehaviour
{
    public OrderPanel orderPanel;
    public Transform orderHolder;
    public CustomerQueue customerQueue;

    private List<OrderPanel> activeOrderPanels = new List<OrderPanel>();

    private int totalOrders = 0;
    private int goodOrders = 0;
    private int badOrders = 0;

    private void Start()
    {
        StartCoroutine(GenerateOrders());
    }

    public IEnumerator GenerateOrders()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (customerQueue.QueueFull() || customerQueue.QueueMovingUp)
            {
                if (customerQueue.QueueMovingUp)
                {
                    Debug.Log("Skipping spawning as the queue is moving!");
                }
                yield return null;
            }
            else
            {
                customerQueue.SpawnCustomer();
            }
        }
    }

    public void CreateOrder(QueueSpot spot)
    {
        if (spot.order == null) { return; }

        totalOrders++;

        OrderPanel panel = Instantiate(orderPanel, orderHolder);
        panel.Setup(totalOrders, spot.order);

        activeOrderPanels.Add(panel);

        panel.OnOrderTimeup += OnOrderTimeUp;

        customerQueue.MarkCustomerInteractedWith();
    }

    public void OnOrderTimeUp(OrderPanel panel)
    {
        Debug.Log($"Order {panel.OrderId} timed out");

        CompleteOrder(panel.OrderId);
        SetBadOrder();
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

        // destroy the game object
        Destroy(orderPanel.gameObject);
    }

    public void SetBadOrder()
    {
        badOrders++;
    }

    public void SetGoodOrder()
    {
        goodOrders++;
    }
}
