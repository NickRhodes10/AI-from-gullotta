using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateBase
{
    public override StateType GetStateType { get { return StateType.Chase; } }

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
}
