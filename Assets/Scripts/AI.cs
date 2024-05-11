using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    enum fsmStates
    {
        Running,
        Hiding,
        Dead
    }

    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;

    [SerializeField, Range(0, 1)] float _hideProbability;
    private fsmStates _state = fsmStates.Running;
    private float _hideEndTime;
    List<HidingPlace> _hidingPoints;
    List<HidingPlace>.Enumerator _hidingPointIterator;
    [SerializeField] private float _stoppingThreshold = 0.5f;
    [SerializeField] private float _hidingTime;

    HidingPlace hidingPlace;

    public NavMeshAgent NavMeshAgent { 
        get { 
            if (!_navMeshAgent) { _navMeshAgent = GetComponent<NavMeshAgent>(); }

            if (_navMeshAgent == null)
            {
                Debug.LogError("Could not find NavMesh!", this.gameObject);
            }

            return _navMeshAgent;
        }
    }

    private void Reset()
    {
        // Because we're using object-pooling, we need to reset all pertinent items.
        // Start and End Points don't change but we'll pass them in.
        Debug.Log($"Resetting to {_startPoint} - {_endPoint} - {_hidingPoints}");
        Debug.Break();
        Initialize(_startPoint, _endPoint, _hidingPoints);

        gameObject.SetActive(true);
    }

    public void Initialize(Transform startPoint, Transform endPoint, List<HidingPlace> hidingPoints)
    {
        _startPoint = startPoint;
        _endPoint = endPoint;

        Debug.Log($"Initialized at {_startPoint.position}");

        NavMeshAgent.enabled = false;
        transform.position = _startPoint.position;
        transform.rotation = _startPoint.rotation;
        NavMeshAgent.enabled = true;

        Debug.Log($"Located at {transform.position}");

        _hidingPoints = hidingPoints;
        _hidingPointIterator = _hidingPoints.GetEnumerator();
        StartRunning();
    }

    private void SetNextDestination()
    {
        float inverseProb = 1.0f - _hideProbability;
        float probToNotHide = inverseProb;

        while (_hidingPointIterator.MoveNext())
        {
            hidingPlace = _hidingPointIterator.Current;
            Debug.Log($"Checking {hidingPlace.name}");
            if (hidingPlace.InUse)
            {
                continue;
            }
            float randValue = Random.Range(0, 1f);

            if (randValue > probToNotHide)
            {
                Debug.Log($"{this.gameObject.name} is going to {hidingPlace.name}");
                hidingPlace.InUse = true;
                NavMeshAgent.destination = hidingPlace.transform.position;
                return;
            } else
            {
                probToNotHide *= inverseProb;
            }
        }

        // If we've hit this point, we've run out of hiding spots. Go to the end
        NavMeshAgent.destination = _endPoint.transform.position;
    }

    private void StartRunning()
    {
        NavMeshAgent.isStopped = false;

        SetNextDestination();

        _state = fsmStates.Running;
    }

    void RunActions()
    {
        if (NavMeshAgent.remainingDistance < _stoppingThreshold)
        {
            StartHiding();
        }
    }

    private void StartHiding()
    {
        // Start Animation?
        _state = fsmStates.Hiding;
        _hideEndTime = Time.time + _hidingTime + Random.Range(0, 2f);
    }

    void HideActions()
    {
        if (Time.time >= _hideEndTime)
        {
            hidingPlace.InUse = false;
            StartRunning();
        }
    }

    private void StartDeath()
    {
        // Start Animation?
        // Increment score by 50

        NavMeshAgent.isStopped = true;
    }

    void DeathActions()
    {
        // After our death time has elapsed, fade away and return the model
    }

    private void Update()
    {
        switch (_state)
        {
            case fsmStates.Running:
                RunActions(); break;
            case fsmStates.Hiding:
                HideActions(); break;
            case fsmStates.Dead:
                DeathActions(); break;
        }
    }
}
