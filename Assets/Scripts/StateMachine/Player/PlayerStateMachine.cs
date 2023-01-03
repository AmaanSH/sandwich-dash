using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public InteractionManager InteractionManager { get; private set; }
    [field: SerializeField] public ItemHolderManager ItemHolder { get; private set; }
    [field: SerializeField] public GameManager GameManager { get; private set; }
    [field: SerializeField] public float WalkSpeed { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float SecondsBetweenDash { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

    public Transform MainCameraTransform { get; private set; }

    private DateTime nextDashReadyTime;

    public bool HasCompletedOrder { get; private set; }
    public IngredientType CompletedOrderJam { get; private set; }


    private void Start()
    {
        MainCameraTransform = Camera.main.transform;

        SwitchState(new PlayerFreeMovementState(this));
    }

    public void SetDash()
    {
        nextDashReadyTime = DateTime.Now.AddSeconds(SecondsBetweenDash);
    }

    public bool CanDash()
    {
        return DateTime.Now > nextDashReadyTime;
    }

    public void SetOrderReady(bool value)
    {
        HasCompletedOrder = value;
    }

    public void SetOrderJam(IngredientType jam)
    {
        CompletedOrderJam = jam;
    }
}
