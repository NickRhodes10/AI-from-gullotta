using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    [SerializeField] private float _turnSlerpSpeed = 2.5f;

    protected FSMAgent _myAgent;

    public abstract StateType GetStateType { get; }

    public void InitState(FSMAgent myAgent)
    {
        _myAgent = myAgent;
    }

    public virtual void OnEnter() { }

    public virtual void OnAnimate()
    {
        _myAgent.GetNavAgent.updatePosition = true;
        _myAgent.GetNavAgent.updateRotation = false;

        _myAgent.GetNavAgent.velocity = _myAgent.GetAnimator.deltaPosition / Time.deltaTime;

        Quaternion newRot = Quaternion.LookRotation(_myAgent.GetNavAgent.desiredVelocity);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, _turnSlerpSpeed * Time.deltaTime);
    }

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
        Vector3 direction = other.transform.position - _myAgent.GetSensorPosition;
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
        Patrol,
        Attack
    }
}
