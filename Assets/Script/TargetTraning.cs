using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetTraining : MonoBehaviour
{
    public static List<TargetTraining> AllTargets =
        new List<TargetTraining>();

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float idleTime = 5f;

    [SerializeField]
    private float moveTime = 3f;

    private Rigidbody rb;

    private float stateTimer;

    private enum MoveState
    {
        IdleBeforeForward,
        MoveForward,
        IdleBeforeBackward,
        MoveBackward
    }

    private MoveState currentState;

    private void OnEnable()
    {
        AllTargets.Add(this);
    }

    private void OnDisable()
    {
        AllTargets.Remove(this);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        currentState =
            MoveState.IdleBeforeForward;

        stateTimer =
            idleTime;
    }

    private void FixedUpdate()
    {
        UpdateStateMachine();
    }

    private void UpdateStateMachine()
    {
        stateTimer -= Time.fixedDeltaTime;

        Vector3 moveDirection =
            Vector3.zero;

        switch (currentState)
        {
            case MoveState.IdleBeforeForward:

                if (stateTimer <= 0)
                {
                    currentState =
                        MoveState.MoveForward;

                    stateTimer =
                        moveTime;
                }

                break;

            case MoveState.MoveForward:

                moveDirection =
                    transform.forward;

                if (stateTimer <= 0)
                {
                    currentState =
                        MoveState.IdleBeforeBackward;

                    stateTimer =
                        idleTime;
                }

                break;

            case MoveState.IdleBeforeBackward:

                if (stateTimer <= 0)
                {
                    currentState =
                        MoveState.MoveBackward;

                    stateTimer =
                        moveTime;
                }

                break;

            case MoveState.MoveBackward:

                moveDirection =
                    -transform.forward;

                if (stateTimer <= 0)
                {
                    currentState =
                        MoveState.IdleBeforeForward;

                    stateTimer =
                        idleTime;
                }

                break;
        }

        rb.MovePosition(
            rb.position +
            moveDirection *
            moveSpeed *
            Time.fixedDeltaTime);
    }
}