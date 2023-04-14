using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : StateBase
{
    // Store all waypoints and the currentIndex which is the target waypoint being used
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected int patrolIndex = 0;

    public override StateType GetStateType { get { return StateType.Patrol; } }

    // Set the agent destination to the current waypoint, make sure agent can move and set animator speed to 1 to make agent walk
    public override void OnEnter()
    {
        _myAgent.SetCurrentTarget(patrolPoints[patrolIndex].position, null, Vector3.Distance(transform.position, patrolPoints[patrolIndex].position), Time.time, Target.TargetType.None);
        _myAgent.GetNavAgent.SetDestination(_myAgent.GetCurrentTarget.GetPosition);

        _myAgent.GetNavAgent.isStopped = false;
        _myAgent.GetAnimator.SetFloat("speed", 1f);
    }

    /// <summary>
    /// If a target can be seen, chase it.
    /// If the destination has been reached, change the next waypoint index and go to idle which will wait for x amount of time before going to the next waypoint
    /// </summary>
    public override StateType OnUpdate()
    {
        if (_myAgent != null)
        {
            if (_myAgent.GetVisualTarget.GetTargetType == Target.TargetType.Visual)
            {
                _myAgent.SetCurrentTarget(_myAgent.GetVisualTarget, 1f);
                return StateType.Chase;
            }

            if (_myAgent.GetAudioTarget.GetTargetType == Target.TargetType.Sound)
            {
                _myAgent.SetCurrentTarget(_myAgent.GetAudioTarget, 1f);
                return StateType.Chase;
            }
        }

        if (_myAgent.GetHasReachedDestination == true)
        {
            GoToNextIndex();
            return StateType.Idle;
        }

        return GetStateType;
    }

    // Increases index, if the index is to big, set index equal to 0
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
