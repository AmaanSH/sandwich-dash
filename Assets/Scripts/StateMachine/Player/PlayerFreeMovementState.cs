using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : PlayerBaseState
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.InputReader.InteractEvent += OnInteraction;
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = GetMovement();
        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            return;
        }

        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.InteractEvent -= OnInteraction;
    }

    private Vector3 GetMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y + right * stateMachine.InputReader.MovementValue.x;
    }

    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }

    private void OnInteraction()
    {
        // TODO
        // find the nearest interaction to the playert
        // if it is interactable then lets switch state
    }
}
