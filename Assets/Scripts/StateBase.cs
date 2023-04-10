using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    protected FSMAgent _myAgent;

    public abstract StateType GetStateType { get; }

    public void InitState(FSMAgent myAgent)
    {
        _myAgent = myAgent;
    }

    public virtual void OnEnter() { }

    public abstract StateType OnUpdate();

    public virtual void OnExit() { }

    public virtual void OnSensorEvent(Collider other)
    {
        if(other == null)
        {
            return;
        }

        if(IsColliderVisible(other) == true)
        {
            _myAgent.GetVisualTarget.Set(other.transform.position, other, Vector3.Distance(transform.position, other.transform.position), Time.time, Target.TargetType.Visual);
        }
    }

    protected bool IsColliderVisible(Collider other)
    {
        Vector3 direction = other.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);

        if(angle > _myAgent.GetFOV * 0.5f)
        {
            return false;
        }

        RaycastHit hit;

        if(Physics.Raycast(_myAgent.GetSensorPosition, direction.normalized, out hit, _myAgent.GetSensorRadius))
        {
            if(hit.collider == other)
            {
                return true;
            }
        }

        return false;
    }

    public enum StateType
    {
        Idle,
        Chase,
        Attack
    }
}
