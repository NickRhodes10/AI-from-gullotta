using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AISensor : MonoBehaviour
{
    // The agent who owns this sensor
    private FSMAgent _myAgent;

    // Set collider to be a trigger, assign the agent, otherwise complain
    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;

        if (GetComponentInParent<FSMAgent>() != null)
        {
            _myAgent = GetComponentInParent<FSMAgent>();
        }
        else
        {
            Debug.LogError("Error: sensor does not have a parent with an FSMAgent on it");
        }
    }

    /// <summary>
    /// Make sure we have an owner, then let them know there was a sensor event of type Enter and pass in the collider
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Enter, other);
    }

    /// <summary>
    /// Make sure we have an owner, then let them know there was a sensor event of type Stay and pass in the collider
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Stay, other);
    }

    /// <summary>
    /// Make sure we have an owner, then let them know there was a sensor event of type Enter and pass in the collider
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Exit, other);
    }
}