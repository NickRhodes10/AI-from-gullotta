using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class FSMAgent : MonoBehaviour
{
    //remove later
    public Color lineColour;

    [SerializeField, Range(0, 360)] protected float _FOV = 60f;
    [SerializeField] protected AISensor _sensor;
    [SerializeField] protected AITrigger _aiTrigger;

    protected Target _curTarget = new Target();
    protected Target _visualTarget = new Target();
    protected Target _audioTarget = new Target();
    protected Dictionary<StateBase.StateType, StateBase> _allStates = new Dictionary<StateBase.StateType, StateBase>();
    protected StateBase _curState;
    protected Animator _anim;
    protected NavMeshAgent _navAgent;
    [SerializeField] protected bool _hasReachedDestination = false;

    public float GetFOV { get { return _FOV; } }
    public Target GetCurrentTarget { get { return _curTarget; } }
    public Target GetVisualTarget { get { return _visualTarget; } }
    public Target GetAudioTarget { get { return _audioTarget; } }
    public bool GetHasReachedDestination { get { return _hasReachedDestination; } }
    public NavMeshAgent GetNavAgent { get { return _navAgent; } }
    public Animator GetAnimator { get { return _anim; } }

    /// <summary>
    /// Finds the position of the sensor based on its position relative to all parent objects of the sensor (lossyScale)
    /// </summary>
    public Vector3 GetSensorPosition
    {
        get
        {
            if (_sensor == null)
            {
                return Vector3.zero;
            }

            Vector3 pos = _sensor.transform.position;
            pos.x += _sensor.GetComponent<SphereCollider>().center.x * _sensor.transform.lossyScale.x;
            pos.y += _sensor.GetComponent<SphereCollider>().center.y * _sensor.transform.lossyScale.y;
            pos.z += _sensor.GetComponent<SphereCollider>().center.z * _sensor.transform.lossyScale.z;

            return pos;
        }
    }

    /// <summary>
    /// Returns the radius of the sensor trigger scaled by all parents (lossyScale) of the sensor
    /// The radius is equal to the greatest length between x, y, and z.
    /// </summary>
    public float GetSensorRadius
    {
        get
        {
            if (_sensor == null)
            {
                return 0f;
            }

            float sensorRadius = _sensor.GetComponent<SphereCollider>().radius;
            float radius = Mathf.Max(sensorRadius * _sensor.transform.lossyScale.x, sensorRadius * _sensor.transform.lossyScale.y);
            radius = Mathf.Max(radius, sensorRadius * _sensor.transform.lossyScale.z);

            return radius;
        }
    }

    /// <summary>
    /// Store local variables of animator and navmeshagent
    /// Search for a sensor as a child object
    /// If one is not found, display error message
    /// </summary>
    protected virtual void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        if (GetComponentInChildren<AISensor>() != null)
        {
            _sensor = GetComponentInChildren<AISensor>();
        }
        else
        {
            Debug.LogError("ERROR: there is no AISensor child of this FSMAgent");
        }
    }

    /// <summary>
    /// Gather all states
    /// Add unique states to _allStates dictionary
    /// Set our state to Idle
    /// </summary>
    protected virtual void Start()
    {
        StateBase[] foundStates = GetComponents<StateBase>();

        for (int i = 0; i < foundStates.Length; i++)
        {
            if(_allStates.ContainsKey(foundStates[i].GetStateType) == false)
            {
                _allStates.Add(foundStates[i].GetStateType, foundStates[i]);
                foundStates[i].InitState(this);
            }
        }

        ChangeState(StateBase.StateType.Idle);
    }

    /// <summary>
    /// Based on the current state, the agent will preform actions 
    /// The actions of the state will retun a State to transition to
    /// Transition to that new state
    /// </summary>
    protected virtual void Update()
    {
        if (_curState == null)
        {
            return;
        }

       ChangeState(_curState.OnUpdate());
    }

    /// <summary>
    /// Once all physics have been calculated, clear the visual and audio targets in preperation for the next frame.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        _visualTarget.Clear();
        _audioTarget.Clear();
    }

    /// <summary>
    /// If the current state is null, set it to idle.
    /// 
    /// If the current state is not null, check to see if the new state is not the same as our current state,
    /// If it is not the same AND allStates contains the new state, exit the old, change to the new state, then enter the new state.
    /// </summary>
    protected virtual void ChangeState(StateBase.StateType newType)
    {
        if(_curState == null)
        {
            if (_allStates.ContainsKey(StateBase.StateType.Idle) == true)
            {
                _curState = _allStates[StateBase.StateType.Idle];
                _curState.OnEnter();
            }
            else
            {
                Debug.LogError("Agent " + gameObject.name + " does not contain an idle state. Please add one!");
                return;
            }
        }

        if(_curState.GetStateType != newType && _allStates.ContainsKey(newType) == true)
        {
            _curState.OnExit();
            _curState = _allStates[newType];
            _curState.OnEnter();
        }
    }

    /// <summary>
    /// If an object has entered or stayed in the sensor trigger,
    /// Send that data to the curState to evaluate
    /// For example, if it detects a threat, go to the chase state.
    /// </summary>
    public void OnSensorEvent(TriggerEventType tet, Collider other)
    {
        if(_curState == null)
        {
            return;
        }

        if(tet != TriggerEventType.Exit)
        {
            _curState.OnSensorEvent(other);
        }
    }
    /// <summary>
    /// Sets current target based on passed in data
    /// Move AI Trigger to target position
    /// Set radius of AI Trigger if passed in
    /// The radius size is helpful to determine range of certain actions such as attacking
    /// For example, patroling might want a radius of 1, whereas a ranged attack might need a radius of 8.
    /// </summary>
    public void SetCurrentTarget(Vector3 p, Collider c, float d, float t, Target.TargetType tt)
    {
        _curTarget.Set(p, c, d, t, tt);

        _aiTrigger.transform.position = p;
    }

    public void SetCurrentTarget(Vector3 p, Collider c, float d, float t, Target.TargetType tt, float r)
    {
        _curTarget.Set(p, c, d, t, tt);

        _aiTrigger.transform.position = p;
        _aiTrigger.SetRadius(r);
    }

    public void SetCurrentTarget(Target t, float r)
    {
        _curTarget.Set(t.GetPosition, t.GetCollider, t.Distance, t.GetTime, t.GetTargetType);
        _aiTrigger.transform.position = t.GetPosition;
        _aiTrigger.SetRadius(r);
    }

    /// <summary>
    /// Sets if the agent has reached its destination
    /// this happens on entering or staying in the AI Trigger
    /// </summary>
    public void DestinationReached(bool value)
    {
        _hasReachedDestination = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColour;

        if (_aiTrigger != null)
        {
            Gizmos.DrawLine(transform.position, _aiTrigger.transform.position);
        }

        Color colour = new Color(1f, 0f, 0f, 0.15f);
        UnityEditor.Handles.color = colour;
        Vector3 rotatedForward = Quaternion.Euler(0f, -_FOV * 0.5f, 0f) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(GetSensorPosition, Vector3.up, rotatedForward, _FOV, GetSensorRadius);
    }

    /// <summary>
    /// Some animations have root motion OnAnimatorMove() calculates the delta position and rotation of the root motion
    /// Then gives the user control of that data
    /// Using that data, tell the current state to update based on the passed in delta data
    /// </summary>
    protected virtual void OnAnimatorMove()
    {
        if(_curState == null)
        {
            return;
        }

        _curState.OnAnimate();
    }
}
