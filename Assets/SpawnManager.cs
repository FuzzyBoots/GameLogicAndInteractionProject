using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    private bool _spawning;
    [SerializeField] private AI _targetObject;
    [SerializeField] private float _delayTime = 5f;

    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;

    [SerializeField] private List<AI> _objectPool;
    [SerializeField] private int _amountToPool = 10;

    public static SpawnManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _objectPool = new List<AI>(_amountToPool);
        for (int i=0; i < _amountToPool; ++i)
        {
            AI tmp = Instantiate(_targetObject);
            tmp.gameObject.SetActive(false);
            _objectPool.Add(tmp);
        }

        Instance.StartSpawning();
    }

    // Hide the constructor
    private SpawnManager() { }

    public void StartSpawning()
    {
        Instance.StartSpawningOnInstance();
    }

    private void StartSpawningOnInstance()
    {
        _spawning = true;
        StartCoroutine(SpawnTargets());
    }

    private AI GetNextPooledObject()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_objectPool[i].gameObject.activeInHierarchy)
            {
                return _objectPool[i];
            }
        }
        
        return null;
    }

    private IEnumerator SpawnTargets()
    {
        while (_spawning)
        {
            AI target = GetNextPooledObject();

            if (target == null)
            {
                Debug.LogWarning("Exceeded our object pool. Spawning a new copy");
                target = Instantiate(_targetObject);
            } else
            {
                // Positioning should be handled by the object's Start
                target.gameObject.SetActive(true);
            }

            AI ai = target.GetComponent<AI>();

            ai.SetStartPoint(_startPoint);
            ai.SetEndPoint(_endPoint);

            yield return new WaitForSeconds(_delayTime);
        }

        yield return null;
    }
}
