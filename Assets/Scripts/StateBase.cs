using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    // The speed at which the agent can turn using a slerp
    [SerializeField] private float _turnSlerpSpeed = 2.5f;

    protected FSMAgent _myAgent;

    // All states have a type, this returns the type that this state is
    public abstract StateType GetStateType { get; }

    // All states belong to an agent, this sets which agent owns this state
    public void InitState(FSMAgent myAgent)
    {
        _myAgent = myAgent;
    }

    // Gives states the option to run unique code upon their start up
    public virtual void OnEnter() { }



    // Makes agent inact behaviours such as patroling, then will return a new StateType based on world knowledge
    // Ex. If we can see the player, transition from patrol to chase
    public abstract StateType OnUpdate();

    // Gives states the option to run unique code upon exiting the state
    public virtual void OnExit() { }

    /// <summary>
    /// Handle input when an object enters or stays in the sensor radius
    /// If that object can be seen, set it to our current target
    /// 
    /// Other features such as being heard can be added in here as well.
    /// </summary>
    public virtual void OnSensorEvent(Collider other)
    {
        if(other == null)
        {
            return;
        }

        if(other.transform.tag == "Player")
        {
            if (IsColliderVisible(other) == true)
            {
                _myAgent.GetVisualTarget.Set(other.transform.position, other, Vector3.Distance(transform.position, other.transform.position), Time.time, Target.TargetType.Visual);
            }
        }
        else if (other.transform.tag == "AudioSource")
        {
            _myAgent.GetAudioTarget.Set(other.transform.position, other, Vector3.Distance(transform.position, other.transform.position), Time.time, Target.TargetType.Sound);
        }

    }

    /// <summary>
    /// Calculate the direction between agent and the target
    /// If the direction is within the field of view, we might be able to see it
    /// Fire a raycast to the object, if the ray hits that object, we can see it
    /// Otherwise vision is hidden by something such as a wall.
    /// </summary>
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

    // Is called on OnAnimatorMove() from the agent. This is the default logic but can have a state override.
    public virtual void OnAnimate()
    {
        _myAgent.GetNavAgent.updatePosition = true;
        _myAgent.GetNavAgent.updateRotation = false;

        _myAgent.GetNavAgent.velocity = _myAgent.GetAnimator.deltaPosition / Time.deltaTime;

        Quaternion newRot = Quaternion.LookRotation(_myAgent.GetNavAgent.desiredVelocity);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, _turnSlerpSpeed * Time.deltaTime);
    }

    public enum StateType
    {
        Idle,
        Chase,
        Patrol,
        Attack
    }
}
