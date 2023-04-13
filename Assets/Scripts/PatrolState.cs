using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : StateBase
{
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected int patrolIndex = 0;

    public override StateType GetStateType { get { return StateType.Patrol; } }

    public override void OnEnter()
    {
        _myAgent.SetCurrentTarget(patrolPoints[patrolIndex].position, null, Vector3.Distance(transform.position, patrolPoints[patrolIndex].position), Time.time, Target.TargetType.None);
        _myAgent.GetNavAgent.SetDestination(_myAgent.GetCurrentTarget.GetPosition);

        _myAgent.GetNavAgent.isStopped = false;
        _myAgent.GetAnimator.SetFloat("speed", 1f);
    }

    public override StateType OnUpdate()
    {
        if (_myAgent.GetHasReachedDestination == true)
        {
            GoToNextIndex();
            return StateType.Idle;
        }

        return GetStateType;
    }

    private void GoToNextIndex()
    {
        patrolIndex++;

        if (patrolIndex > patrolPoints.Length - 1)
        {
            patrolIndex = 0;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (i < patrolPoints.Length - 1)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
            }
        }
    }
}
