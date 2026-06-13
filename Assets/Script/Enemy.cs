using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    public static List<Enemy> AllEnemies =
        new List<Enemy>();

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float gravity = -20f;

    [Header("Timing")]
    [SerializeField]
    private float idleTime = 5f;

    [SerializeField]
    private float moveTime = 3f;

    private CharacterController controller;

    private float verticalVelocity;

    private float stateTimer;

    private MoveState currentState;

    private enum MoveState
    {
        IdleBeforeForward,
        MoveForward,
        IdleBeforeBackward,
        MoveBackward
    }

    private void OnEnable()
    {
        AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        AllEnemies.Remove(this);
    }

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentState =
            MoveState.IdleBeforeForward;

        stateTimer = idleTime;
    }

    private void Update()
    {
        UpdateStateMachine();

        ApplyGravity();
    }

    private void UpdateStateMachine()
    {
        stateTimer -= Time.deltaTime;

        Vector3 moveDirection =
            Vector3.zero;

        switch (currentState)
        {
            case MoveState.IdleBeforeForward:

                if (stateTimer <= 0f)
                {
                    currentState =
                        MoveState.MoveForward;

                    stateTimer = moveTime;
                }

                break;

            case MoveState.MoveForward:

                moveDirection =
                    transform.forward;

                if (stateTimer <= 0f)
                {
                    currentState =
                        MoveState.IdleBeforeBackward;

                    stateTimer = idleTime;
                }

                break;

            case MoveState.IdleBeforeBackward:

                if (stateTimer <= 0f)
                {
                    currentState =
                        MoveState.MoveBackward;

                    stateTimer = moveTime;
                }

                break;

            case MoveState.MoveBackward:

                moveDirection =
                    -transform.forward;

                if (stateTimer <= 0f)
                {
                    currentState =
                        MoveState.IdleBeforeForward;

                    stateTimer = idleTime;
                }

                break;
        }

        controller.Move(
            moveDirection *
            moveSpeed *
            Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime);
    }
}