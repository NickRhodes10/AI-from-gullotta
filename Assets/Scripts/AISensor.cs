using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AISensor : MonoBehaviour
{
    private FSMAgent _myAgent;

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

    private void OnTriggerEnter(Collider other)
    {
        if(_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Enter, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Stay, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_myAgent == null)
        {
            return;
        }

        _myAgent.OnSensorEvent(TriggerEventType.Exit, other);
    }
}