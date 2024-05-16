using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private float _startTime;

    private int _score;
    private int _enemiesRemaining;
    private int _enemiesEscaped;
    [SerializeField] private float _duration = 240f;

    [SerializeField] private int _enemiesToKill = 25;
    [SerializeField] private int _enemyEscapeThreshold = 10;

    public static GameManager Instance
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
    
    public void StartSession()
    {
        _startTime = Time.time;
<<<<<<< HEAD
        SpawnManager.Instance.
=======
        _enemiesRemaining = _enemiesToKill;
        UIManager.Instance.SetEnemiesRemaining(_enemiesRemaining);
>>>>>>> 16c275a (Win and Loss conditions and the start of a menu system)
    }

    private void Update()
    {
        // Update time
        float timeRemaining = _duration - (Time.time - _startTime);
        UIManager.Instance.SetTime(timeRemaining);

        CheckLossCondition();
    }

    private void Start()
    {
        StartSession();
    }

    public void AdjustScore(int score)
    {
        _score += score;
        UIManager.Instance.SetScore(_score);
    }

    public void AdjustEnemiesRemaining(int enemies) {
        _enemiesRemaining += enemies;
        UIManager.Instance.SetEnemiesRemaining(_enemiesRemaining);

        CheckWinCondition();
    }

    public void AdjustEnemiesEscaped(int enemies)
    {
        _enemiesEscaped += enemies;
        UIManager.Instance.SetEnemiesEscaped(_enemiesEscaped);

        CheckLossCondition();
    }

    private void CheckLossCondition()
    {
        if (_enemiesEscaped > _enemyEscapeThreshold || _duration <= 0)
        {
            Debug.Log("You lose");
            Debug.Break();
        }
    }

    private void CheckWinCondition()
    {
        if (_enemiesRemaining <= 0)
        {
            // Trigger win condition
            Debug.Log("You win!");
            Debug.Break();
        }
    }
}
