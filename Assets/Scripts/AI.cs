using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        transform.position = _startPoint.position;

        _navMeshAgent.destination = _endPoint.position;
    }

    public void SetStartPoint(Transform startPoint)
    {
        _startPoint = startPoint;
    }

    public void SetEndPoint(Transform endPoint)
    {
        _endPoint = endPoint;
    }
}
