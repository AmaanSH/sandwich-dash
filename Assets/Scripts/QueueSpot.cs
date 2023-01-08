using System.Collections;
using UnityEngine;
using System;

public enum QueueType
{
    Waiting,
    Ready
}

public class QueueSpot : MonoBehaviour
{
    private readonly int MOVEMENT_SPEED = Animator.StringToHash("MovementSpeed");
    private const float ANIMATOR_DAMP_TIME = 0.1f;
    private const float ANGRY_TIME = 10f;

    public event Action<QueueSpot> OnCustomerWaitedTooLong;

    [field: SerializeField] public SandwichRandomiser SandwichRandomiser { get; private set; }
    [field: SerializeField]  public GameObject Spot { get; private set; }
    [field: SerializeField] public Timer Timer { get; private set; }
    [field: SerializeField] public float Index { get; private set; }
    [field: SerializeField] public QueueType QueueType { get; private set; }
    public bool CustomerReady { get; private set; }
    public Animator Animator { get; private set; }
    public GameObject Customer { get; private set; }
    public bool IsMoving { get; private set; }
    public Order Order { get; private set; }
    public int OrderId { get; private set; } = -1;

    private float timeElpased = 0f;
    private Timer currentTimer;
    private bool audioPlayed;

    public void SetCustomer(GameObject customer)
    {
        Customer = customer;
        Animator = customer.GetComponent<Animator>();

        // remove timer component if it exists in children
        foreach (Transform child in customer.transform)
        {
            if (child.TryGetComponent(out Timer timer))
            {
                Destroy(timer.gameObject);
                break;
            }
        }
    }

    public void SetOrderId(int orderId)
    {
        OrderId = orderId;
    }

    public IEnumerator MoveCustomerToQueueSpot()
    {
        IsMoving = true;

        while (Customer.transform.position != Spot.transform.position)
        {
            Customer.transform.position = Vector3.MoveTowards(Customer.transform.position, Spot.transform.position, 1f * Time.deltaTime);

            if (Animator)
                Animator.SetFloat(MOVEMENT_SPEED, 0.5f, ANIMATOR_DAMP_TIME, Time.deltaTime);

            yield return 0;
        }

        Animator.SetFloat(MOVEMENT_SPEED, 0f);
        IsMoving = false;

        if (Index == 0 && QueueType == QueueType.Waiting)
        {
            CreateOrder();
        }
    }

    public void ClearQueue(bool destroy = false)
    {
        CustomerReady = false;

        if (destroy)
        {
            StopAllCoroutines();
            Destroy(Customer);
        }

        Customer = null;
        Order = null;
        OrderId = -1;

        if (currentTimer)
        {
            Destroy(currentTimer.gameObject);
            currentTimer = null;
        }

        timeElpased = 0;
    }

    private void CreateOrder()
    {
        Order order = SandwichRandomiser.GenerateOrder();
        Debug.Log($"Generated order for {order.jamName} with a max time of {order.maxTime}");

        currentTimer = Instantiate(Timer, Customer.transform);
        currentTimer.Setup(ANGRY_TIME);

        Order = order;
        CustomerReady = true;
    }

    private void Update()
    {
        if (QueueType == QueueType.Waiting && CustomerReady && Customer != null)
        {
            timeElpased += Time.deltaTime;
            currentTimer.SetFill(360 / (ANGRY_TIME / timeElpased));

            if (ANGRY_TIME - timeElpased <= 5f && !audioPlayed)
            {
                MusicManager.Instance.Play("TimeRunningOut");
                audioPlayed = true;
            }

            if (timeElpased > ANGRY_TIME)
            {
                OnCustomerWaitedTooLong?.Invoke(this);
            }
        }
    }
}
