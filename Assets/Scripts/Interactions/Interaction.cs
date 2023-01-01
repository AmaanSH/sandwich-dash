using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    protected Animation animation;
    protected bool canUse;

    public abstract void Interact();
    public abstract void CanInteract();
    public abstract void Exit();
}
