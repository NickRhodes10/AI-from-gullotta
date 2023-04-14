using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateBase
{
    public override StateType GetStateType { get { return StateType.Chase; } }

    /// <summary>
    /// If a target can still be seen, set the destination to the targets position
    /// 
    /// If we reach our destination and cannot see our target, return to idle. 
    /// If we reach our destination and can see our target, return to attack.
    /// </summary>
    /// <returns></returns>
    public override StateType OnUpdate()
    {
        if (_myAgent.GetVisualTarget.GetTargetType == Target.TargetType.Visual)
        {
            _myAgent.SetCurrentTarget(_myAgent.GetVisualTarget, 1f);
            _myAgent.GetNavAgent.SetDestination(_myAgent.GetCurrentTarget.GetPosition);
        }



        if (_myAgent.GetHasReachedDestination == true)
        {
            if (_myAgent != null && _myAgent.GetVisualTarget.GetTargetType == Target.TargetType.None)
            {
                return StateType.Idle;
            }
            else if (_myAgent != null && _myAgent.GetVisualTarget.GetTargetType == Target.TargetType.Visual)
            {
                return StateType.Attack;
            }


        }

        return StateType.Chase;
    }

    /// <summary>
    /// Make sure the navAgent can move
    /// then set animation speed variable to make agent run
    /// </summary>
    public override void OnEnter()
    {
        _myAgent.GetNavAgent.isStopped = false;
        _myAgent.GetAnimator.SetFloat("speed", 2f);
        _myAgent.GetNavAgent.SetDestination(_myAgent.GetCurrentTarget.GetPosition);
    }
}
