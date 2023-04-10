using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateBase
{
    public override StateType GetStateType { get { return StateType.Idle; } }

    public override StateType OnUpdate()
    {
        if(_myAgent != null)
        {
            if(_myAgent.GetVisualTarget.GetTargetType == Target.TargetType.Visual)
            {
                _myAgent.SetCurrentTarget(_myAgent.GetVisualTarget, 1f);
                return StateType.Chase;
            }
        }

        return GetStateType;
    }
}
