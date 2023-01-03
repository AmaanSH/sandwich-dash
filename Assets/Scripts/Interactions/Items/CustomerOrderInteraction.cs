using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderInteraction : Interaction
{
    public override bool CanInteract()
    {
        return stateMachine.GameManager.customerQueue.QueueCustomersReady();
    }

    public override void Interact()
    {
        QueueSpot queueSpot = stateMachine.GameManager.customerQueue.GetQueueSpot(0);

        if (queueSpot.Customer != null && queueSpot.Order != null)
        {
            stateMachine.GameManager.CreateOrder(queueSpot);
        }

        base.Exit();
    }
}
