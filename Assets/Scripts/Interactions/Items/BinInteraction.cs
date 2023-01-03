using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinInteraction : Interaction
{
    public override bool CanInteract()
    {
        return stateMachine.HasCompletedOrder;
    }

    public override void Interact()
    {
        stateMachine.SetOrderReady(false);
        stateMachine.ItemHolder.RemoveItem();

        base.Exit();
    }
}
