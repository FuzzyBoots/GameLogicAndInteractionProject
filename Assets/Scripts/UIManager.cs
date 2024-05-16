using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _timeText;
    [SerializeField] TMP_Text _enemiesRemainingText;
    [SerializeField] TMP_Text _enemiesEscapedText;

    [SerializeField] GameObject _winScreen;
    [SerializeField] GameObject _loseScreen;

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
        _timeText.text = $"Time: {time:F2}";
    }

    public void SetScore(int score)
    {
        _scoreText.text = $"Score: {score}";
    }

    public void SetEnemiesRemaining(int enemies) {
        _enemiesRemainingText.text = $"Remaining: {enemies}";
    }

    public void SetEnemiesEscaped(int enemies)
    {
        _enemiesEscapedText.text = $"Escaped: {enemies}";
    }

    public void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        _loseScreen.SetActive(true);
    }
}
