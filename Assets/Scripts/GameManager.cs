using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // UI text that shows the current score.
    [SerializeField] private TMP_Text scoreText;

    // UI text that shows the time left.
    [SerializeField] private TMP_Text timerText;

    // How many seconds the game lasts.
    [SerializeField] private float startTime = 30f;

    // Current score value.
    private int score;

    // Time left in the game.
    private float timeLeft;

    // True while the game is still running.
    private bool gameRunning = true;

    private void Start()
    {
        timeLeft = startTime;
        UpdateScoreText();
        UpdateTimerText();
    }

    private void Update()
    {
        if (!gameRunning)
        {
            return;
        }

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            gameRunning = false;
        }

        UpdateTimerText();
    }

    public bool IsGameRunning()
    {
        return gameRunning;
    }

    public void AddScore()
    {
        if (!gameRunning)
        {
            return;
        }

        score += 1;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft);
        }
    }
}