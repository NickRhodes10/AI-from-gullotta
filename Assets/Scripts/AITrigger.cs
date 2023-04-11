using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AITrigger : MonoBehaviour
{
    [SerializeField] private FSMAgent _myAgent;
    private SphereCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;

        if (transform.parent.GetComponentInChildren<FSMAgent>() != null)
        {
            _myAgent = transform.parent.GetComponentInChildren<FSMAgent>();
        }
        else
        {
            Debug.LogError("Error: Trigger does not have a parent with an FSMAgent on it");
        }
    }

    public void SetRadius(float r)
    {
        _collider.radius = r;
    }    

    private void OnTriggerEnter(Collider other)
    {
        if(_myAgent == null || other.transform != _myAgent.transform)
        {
            return;
        }

        _myAgent.DestinationReached(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_myAgent == null || other.transform != _myAgent.transform)
        {
            return;
        }

        _myAgent.DestinationReached(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_myAgent == null || other.transform != _myAgent.transform)
        {
            return;
        }

        _myAgent.DestinationReached(false);
    }
}