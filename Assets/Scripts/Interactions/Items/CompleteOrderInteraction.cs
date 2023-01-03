using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteOrderInteraction : Interaction
{
    public Transform holder;
    public override bool CanInteract()
    {
        return stateMachine.HasCompletedOrder;
    }

    public override void Interact()
    {
        GameObject holding = stateMachine.ItemHolder.CurrentHolding;

        // okay.. lets move this to the holder transform
        holding.transform.SetParent(holder);
        holding.transform.localPosition = Vector3.zero;

        stateMachine.ItemHolder.RestCurrentHolding();

        stateMachine.SetOrderReady(false);

        Invoke(nameof(CheckOrder), 0.2f);

        base.Exit();
    }

    private void CheckOrder()
    {
        // TODO: is this an order that was generated?
        OrderPanel panel = stateMachine.GameManager.FindOrderWithJam(stateMachine.CompletedOrderJam);
        if (panel == null)
        {
            // okay this was a bad order... play some FX?
            Debug.Log("Bad order - doesn't exist!");
            stateMachine.GameManager.SetBadOrder();
        }
        else
        {
            Debug.Log("Good order!");
            
            stateMachine.GameManager.SetGoodOrder();

            stateMachine.GameManager.CompleteOrder(panel.OrderId);
        }

        foreach (Transform child in holder)
        {
            Destroy(child.gameObject);
        }
    }
}
