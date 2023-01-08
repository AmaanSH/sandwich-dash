using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionState : PlayerBaseState
{
    private Interaction active;
    public PlayerInteractionState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        active = stateMachine.InteractionManager.ActiveInteraction;

        active.InteractionFinished += InteractionFinished;

        //Debug.Log($"Interaction {active.name}");

        active.Interact();
    }

    public override void Tick(float deltaTime)
    {

    }

    public override void Exit()
    {
        active.InteractionFinished -= InteractionFinished;
    }

    private void InteractionFinished()
    {
        if (!active.keepInteractionOnCompletion)
            stateMachine.InteractionManager.RemoveInteraction(active);
        
        stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
    }
}
