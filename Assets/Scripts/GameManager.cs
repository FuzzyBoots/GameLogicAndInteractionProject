using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private float _startTime;

    private int _score;
    private int _enemies;
    private float _duration = 60f;

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
    }

    private void Update()
    {
        // Update time
        float timeRemaining = _duration - (Time.time - _startTime);
        UIManager.Instance.SetTime(timeRemaining);

        if (timeRemaining < 0)
        {
            // Do game end
        }
    }

    private void Start()
    {
        StartSession();
    }

    public void AdjustScore(int score)
    {
        Debug.Log($"Adding {score} to {_score}");
        _score += score;
        UIManager.Instance.SetScore(_score);
    }

    public void AdjustEnemies(int enemies) {
        Debug.Log($"Adding {enemies} to {_enemies}");
        _enemies += enemies;
        UIManager.Instance.SetEnemies(_enemies);
    }
}
