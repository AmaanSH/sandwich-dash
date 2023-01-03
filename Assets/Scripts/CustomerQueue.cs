using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QueueSpot
{
    public GameObject spot;
    public bool customerReady;
    public Animator animator;
    public GameObject Customer { get; set; }

    public bool isMoving;

    public Order order;
}

public class CustomerQueue : MonoBehaviour
{
    private readonly int MOVEMENT_SPEED = Animator.StringToHash("MovementSpeed");
    private const float ANIMATOR_DAMP_TIME = 0.1f;

    public QueueSpot[] queueSpots;
    public Transform spawnPosition;
    public Transform queue;

    public GameObject[] customerPrefabs;
    public SandwichRandomiser sandwichRandomiser;

    public bool QueueMovingUp { get; private set; }

    public void SpawnCustomer()
    {
        GameObject customer = SelectRandomCustomerPrefab();
        QueueSpot queueSpot = GetAvailableQueueSpot();
        
        if (customer && queueSpot != null)
        {
            GameObject newCustomer = Instantiate(customer, queue);
            newCustomer.transform.position = spawnPosition.position;

            queueSpot.Customer = newCustomer;
            queueSpot.isMoving = true;
            StartCoroutine(MoveCustomerToQueueSpot(queueSpot, newCustomer, queueSpot.spot));
        }
    }

    public IEnumerator MoveCustomerToQueueSpot(QueueSpot queue, GameObject customer, GameObject destination)
    {
        Animator animator = customer.GetComponent<Animator>();
        queue.animator = animator;

        while (customer.transform.position != destination.transform.position)
        {            
            customer.transform.position = Vector3.MoveTowards(customer.transform.position, destination.transform.position, 1f * Time.deltaTime);
            
            if (queue.animator)
                queue.animator.SetFloat(MOVEMENT_SPEED, 0.5f, ANIMATOR_DAMP_TIME, Time.deltaTime);
            
            yield return 0;
        }

        queue.animator.SetFloat(MOVEMENT_SPEED, 0f);

        queue.isMoving = false;

        // are they at the front of the queue?
        if (queueSpots[0].Customer == customer)
        {
            queue.customerReady = true;

            Order order = sandwichRandomiser.GenerateOrder();
            Debug.Log($"Generated order for {order.jamName} with a max time of {order.maxTime}");

            queue.order = order;
        }
    }

    public void MarkCustomerInteractedWith()
    {
        Destroy(queueSpots[0].Customer);

        queueSpots[0].order = null;
        queueSpots[0].Customer = null;
        queueSpots[0].customerReady = false;

        StartCoroutine(MoveCustomers());
    }

    public QueueSpot GetQueueSpot(int index)
    {
        return queueSpots[index];
    }

    public QueueSpot GetQueueSpot(GameObject customer)
    {
        foreach (QueueSpot queue in queueSpots)
        {
            if (queue.Customer == customer)
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
            if (spot.customerReady && spot.Customer != null)
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
            
            if (queueSpots[i].Customer == null && queueSpots[i + 1].Customer != null)
            {
                while (queueSpots[i].isMoving || queueSpots[i + 1].isMoving)
                {
                    Debug.Log("Waiting for old move to finish...");
                    yield return 0;
                }

                queueSpots[i].Customer = queueSpots[i + 1].Customer;
                queueSpots[i + 1].Customer = null;
                queueSpots[i + 1].animator = null;

                yield return StartCoroutine(MoveCustomerToQueueSpot(queueSpots[i], queueSpots[i].Customer, queueSpots[i].spot));
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
