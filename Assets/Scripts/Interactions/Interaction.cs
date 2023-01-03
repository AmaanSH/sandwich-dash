using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interaction : MonoBehaviour
{    
    public Animation animationClip;
    public bool keepInteractionOnCompletion;

    protected PlayerStateMachine stateMachine;
    
    public event Action InteractionFinished;

    public void Init(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    
    public abstract void Interact();
    public abstract bool CanInteract();
    
    public virtual void Exit()
    {
        InteractionFinished?.Invoke();
    }
}
