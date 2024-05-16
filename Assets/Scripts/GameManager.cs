using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private float _startTime;

    private int _score;
    private int _enemiesRemaining;
    private int _enemiesEscaped;
    private bool _isEnded = false;
    [SerializeField] private float _duration = 240f;

    [SerializeField] private int _enemiesToKill = 25;
    [SerializeField] private int _enemyEscapeThreshold = 10;

    public static GameManager Instance
    {
        get;
        private set;
    }

    public float RemainingTime { get { return _duration - (Time.time - _startTime); }
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
        _enemiesRemaining = _enemiesToKill;
        UIManager.Instance.SetEnemiesRemaining(_enemiesRemaining);
        // SpawnManager.Instance.StartSpawning();
    }

    private void Update()
    {
        // Update time
        UIManager.Instance.SetTime(RemainingTime);

        CheckLossCondition();

        if (_isEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            ReloadScene();
        }
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
        if (_enemiesEscaped > _enemyEscapeThreshold || RemainingTime <= 0)
        {
            _isEnded = true;
            UIManager.Instance.ShowLoseScreen();
            SpawnManager.Instance.StopSpawning();
        }
    }

    private void CheckWinCondition()
    {
        if (_enemiesRemaining <= 0)
        {
            _isEnded = true;
            UIManager.Instance.ShowWinScreen();
            SpawnManager.Instance.StopSpawning();
            SpawnManager.Instance.DeactivateSpawns();
        }
    }

    public void ReloadScene()
    {
        Debug.Log("Reloading scene");
        SceneManager.LoadScene(1);
    }
}
