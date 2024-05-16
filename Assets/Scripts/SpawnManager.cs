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
        StartSpawning();
    }

    // Hide the constructor
    private SpawnManager() { }

    public void StartSpawning()
    {
        if (!_spawning)
        {
            _spawning = true;
            StartCoroutine(SpawnTargets());
        }
    }

    public void StopSpawning ()
    {
        _spawning = false;
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
                Debug.LogWarning("Exceeded our object pool. Not spawning.");
            } else
            {
                target.gameObject.SetActive(true);
                target.Initialize(_startPoint, _endPoint, _hidingPlaces);
            }

            yield return new WaitForSeconds(_delayTime);
        }

        yield return null;
    }

    internal void DeactivateSpawns()
    {
        foreach (AI item in _objectPool)
        {
            item.gameObject.SetActive(false);
        }
    }
}
