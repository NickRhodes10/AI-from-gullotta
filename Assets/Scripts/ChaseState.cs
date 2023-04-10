using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateBase
{
    public override StateType GetStateType { get { return StateType.Chase; } }

    public override StateType OnUpdate()
    {
        if (_myAgent != null)
        {
            _myAgent.GetNavAgent.SetDestination(_myAgent.GetCurrentTarget.GetPosition);
        }

        return StateType.Chase;
    }
}
