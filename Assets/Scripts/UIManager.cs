using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text enemiesText;

    private static UIManager _instance;
    public static UIManager Instance
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
    public void SetTime(float time)
    {
        timeText.text = $"Time: {time:F2}";
    }

    public void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void SetEnemies(int enemies) {
        enemiesText.text = $"Enemies: {enemies}";
    }
}