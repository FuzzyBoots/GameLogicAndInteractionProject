using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

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
    [SerializeField] private int _maxCapacity = 20;

    [SerializeField] List<HidingPlace> _hidingPlaces;

    [SerializeField] GameObject _hidingPlaceContainer;
    private ObjectPool<AI> _newObjectPool;

    public static SpawnManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Get the available hiding places
        _hidingPlaces = _hidingPlaceContainer.GetComponentsInChildren<HidingPlace>().ToList<HidingPlace>();

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
        _newObjectPool = new ObjectPool<AI>(createAIFunc, getAIFunc, releaseAIFunc, destroyAIFunc, true, _amountToPool, _maxCapacity);

        StartSpawning();
    }

    private void destroyAIFunc(AI aiObj)
    {
        Destroy(aiObj);
    }

    private void releaseAIFunc(AI aiObj)
    {
        aiObj.gameObject.SetActive(false);
    }

    private void getAIFunc(AI aiObj)
    {
        aiObj.gameObject.SetActive(true);
    }

    private AI createAIFunc()
    {
        AI tmp = Instantiate(_targetObject);
        tmp.gameObject.SetActive(false);

        return tmp;
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

    private IEnumerator SpawnTargets()
    {
        while (_spawning)
        {
            if (_newObjectPool.CountAll >= _maxCapacity)
            {
                Debug.Log("Failed to spawn additional enemy because our pool is full");
            }
            else
            {

                AI target = _newObjectPool.Get();

                target.gameObject.SetActive(true);
                target.Initialize(_startPoint, _endPoint, _hidingPlaces);
            }

            yield return new WaitForSeconds(_delayTime);
        }

        yield return null;
    }

    public void DeactivateInstance(AI aiObj)
    {
        _newObjectPool.Release(aiObj);
    }

    internal void DeactivateSpawns()
    {
        _newObjectPool.Clear();
    }
}
