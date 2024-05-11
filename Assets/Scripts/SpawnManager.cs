using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    [SerializeField] List<HidingPlace> _hidingPlaces;

    [SerializeField] GameObject _hidingPlaceContainer;

    public static SpawnManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        _hidingPlaces = _hidingPlaceContainer.GetComponentsInChildren<HidingPlace>().ToList<HidingPlace>();

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

    private void StartSpawning()
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

            ai.Initialize(_startPoint, _endPoint, _hidingPlaces);

            yield return new WaitForSeconds(_delayTime);
        }

        yield return null;
    }
}