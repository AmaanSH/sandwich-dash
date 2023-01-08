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
            if (stateMachine.GameManager.ActiveOrders >= 3)
            {
                stateMachine.GameManager.statusPanel.AddStatusMessage("You can only have 3 orders at a time!", false);
            }
            else
            {
                stateMachine.GameManager.TakeOrder(queueSpot);
            }
        }

        base.Exit();
    }
}
