using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateBase
{
    // Range of time that the agent will wait before exiting idle
    [SerializeField] private Vector2 _waitTimeRange;

    // Variable to hold time agent will wait
    private float _curWaitTime;

    // Returns StateType.Idle
    public override StateType GetStateType { get { return StateType.Idle; } }


    /// <summary>
    /// If a target is seen, set the destination to that target and chase it
    /// 
    /// otherwise, decrease the current wait timer
    /// once it is 0 or less, move to patrol
    /// </summary>
    /// <returns></returns>
    public override StateType OnUpdate()
    {
        if(_myAgent != null)
        {
            if(_myAgent.GetVisualTarget.GetTargetType == Target.TargetType.Visual)
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

        _curWaitTime -= Time.deltaTime;

        if (_curWaitTime <= 0)
        {
            return StateType.Patrol;
        }

        return GetStateType;
    }

    public override void OnAnimate()
    {
        _myAgent.GetNavAgent.updateRotation = false;
    }

    public override void OnEnter()
    {
        _myAgent.GetNavAgent.isStopped = true;
        _myAgent.GetAnimator.SetFloat("speed", 0f);
        _curWaitTime = Random.Range(_waitTimeRange.x, _waitTimeRange.y);
    }
}
