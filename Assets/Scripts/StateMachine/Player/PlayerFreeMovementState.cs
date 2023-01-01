using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : PlayerBaseState
{
    private readonly int MOVEMENT_SPEED = Animator.StringToHash("MovementSpeed");
    private readonly int MOVEMENT_BLENDTREE = Animator.StringToHash("FreeMovementBlendTree");

    private const float ANIMATOR_DAMP_TIME = 0.1f;
    private const float DASH_TIME = 0.3f;

    private bool isDashing = false;
    private float dashingTimeElapsed;

    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.InputReader.InteractEvent += OnInteraction;
        stateMachine.InputReader.DashEvent += OnDash;

        stateMachine.Animator.Play(MOVEMENT_BLENDTREE);
    }

    public override void Tick(float deltaTime)
    {
        float speed = (isDashing) ? stateMachine.DashSpeed : stateMachine.WalkSpeed;
        float value = (isDashing) ? 1f : 0.5f;

        Vector3 movement = (isDashing) ? GetDashMovement() : GetMovement();
        Move(movement * speed, deltaTime);

        if (isDashing)
        {
            dashingTimeElapsed += deltaTime;
            if (dashingTimeElapsed > DASH_TIME)
            {
                isDashing = false;
                dashingTimeElapsed = 0f;
            }
        }
        else
        {
            if (stateMachine.InputReader.MovementValue == Vector2.zero)
            {
                stateMachine.Animator.SetFloat(MOVEMENT_SPEED, 0f, ANIMATOR_DAMP_TIME, deltaTime);
                return;
            }
        }

        stateMachine.Animator.SetFloat(MOVEMENT_SPEED, value, ANIMATOR_DAMP_TIME, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.InteractEvent -= OnInteraction;
        stateMachine.InputReader.DashEvent -= OnDash;
    }

    private Vector3 GetMovement()
    {
        return new Vector3(-stateMachine.InputReader.MovementValue.x, 0, -stateMachine.InputReader.MovementValue.y);
    }

    private Vector3 GetDashMovement()
    {
        Vector3 forward = stateMachine.transform.forward;
        forward.y = 0;

        forward.Normalize();

        return forward * 1f;
    }

    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }

    private void OnDash()
    {
        if (stateMachine.CanDash())
        {
            stateMachine.SetDash();
            isDashing = true;
        }
    }

    private void OnInteraction()
    {
        // TODO
        // find the nearest interaction to the playert
        // if it is interactable then lets switch state
    }
}
