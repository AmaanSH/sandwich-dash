using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerQueue : MonoBehaviour
{
    public QueueSpot[] queueSpots;
    public Transform spawnPosition;
    public Transform queue;

    public GameObject[] customerPrefabs;

    public bool QueueMovingUp { get; private set; }

    private void Start()
    {
        for (int i = 0; i < queueSpots.Length; i++)
        {
            queueSpots[i].OnCustomerWaitedTooLong += CustomerWaitedTooLong;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < queueSpots.Length; i++)
        {
            queueSpots[i].OnCustomerWaitedTooLong -= CustomerWaitedTooLong;
        }
    }
    
    public void Cleanup()
    {
        for (int i = 0; i < queueSpots.Length; i++)
        {
            queueSpots[i].OnCustomerWaitedTooLong -= CustomerWaitedTooLong;
        }

        for (int i = 0; i < queueSpots.Length; i++)
        {
            queueSpots[i].ClearQueue(true);
        }
    }

    public QueueSpot SpawnCustomer(GameObject customerObj = null)
    {
        GameObject customer = (customerObj) ? customerObj : SelectRandomCustomerPrefab();
        QueueSpot queueSpot = GetAvailableQueueSpot();
        
        if (customer && queueSpot != null)
        {
            GameObject newCustomer = Instantiate(customer, queue);
            newCustomer.transform.position = spawnPosition.position;

            queueSpot.SetCustomer(newCustomer);
            StartCoroutine(queueSpot.MoveCustomerToQueueSpot());

            return queueSpot;
        }

        return null;
    }
  
    public void MarkCustomerInteractedWith()
    {
        queueSpots[0].ClearQueue(true);

        StartCoroutine(MoveCustomers());
    }

    public IEnumerator MarkCustomerInteractedWith(int orderId)
    {
        while (QueueMovingUp)
        {
            yield return 0;
        }

        QueueSpot spot = GetQueueSpotWithOrder(orderId);

        while (spot.IsMoving)
        {
            yield return 0;
        }

        spot.ClearQueue(true);

        StartCoroutine(MoveCustomers());
    }

    public void CustomerWaitedTooLong(QueueSpot spot)
    {
        spot.ClearQueue(true);
        
        StartCoroutine(MoveCustomers());
    }

    public QueueSpot GetQueueSpot(int index)
    {
        return queueSpots[index];
    }

    public QueueSpot GetQueueSpotWithOrder(int orderId)
    {
        foreach (QueueSpot queue in queueSpots)
        {
            if (queue.OrderId == orderId)
            {
                return queue;
            }
        }

        return null;
    }

    public bool QueueFull()
    {
        int queueSpotsFull = 0;

        foreach (QueueSpot spot in queueSpots)
        {
            if (spot.Customer != null)
            {
                queueSpotsFull++;
            }
        }

        return queueSpotsFull >= queueSpots.Length;
    }

    public bool QueueCustomersReady()
    {
        int queueSpotsReady = 0;

        foreach (QueueSpot spot in queueSpots)
        {
            if (spot.CustomerReady && spot.Customer != null)
            {
                queueSpotsReady++;
            }
        }

        return queueSpotsReady > 0;
    }

    private IEnumerator MoveCustomers()
    {
        QueueMovingUp = true;

        for (int i = 0; i < queueSpots.Length; i++)
        {
            // end of the queue so no need to move forward
            if (i + 1 == queueSpots.Length)
            {
                break;
            }

            QueueSpot current = queueSpots[i + 1];
            QueueSpot next = queueSpots[i];
            
            if (next.Customer == null && current.Customer != null)
            {
                while (next.IsMoving || current.IsMoving)
                {
                    yield return 0;
                }

                next.SetCustomer(current.Customer);

                if (current.OrderId != -1)
                {
                    next.SetOrderId(current.OrderId);
                }

                current.ClearQueue();

                yield return StartCoroutine(next.MoveCustomerToQueueSpot());
            }
        }

        QueueMovingUp = false;
    }

    private QueueSpot GetAvailableQueueSpot()
    {
        for (int i = 0; i < queueSpots.Length; i++)
        {
            if (i + 1 == queueSpots.Length)
            {
                if (queueSpots[i].Customer == null)
                {
                    return queueSpots[i];
                }
            }
            else
            {
                if (queueSpots[i].Customer == null && queueSpots[i + 1].Customer == null)
                {
                    return queueSpots[i];
                }
            }
        }

        return null;
    }

    private GameObject SelectRandomCustomerPrefab()
    {
        return customerPrefabs[Random.Range(0, customerPrefabs.Length)];
    }
}
