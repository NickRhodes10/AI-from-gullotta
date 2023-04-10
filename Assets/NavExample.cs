using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavExample : MonoBehaviour
{
    NavMeshAgent _agent;

    private void Start()
    {
        if(_agent.hasPath == false && _agent.pathPending == false)
        {

        }

        //if(_agent.isPathStale)
    }
}
