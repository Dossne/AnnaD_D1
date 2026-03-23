using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI text that shows the current score.
    [SerializeField] private Text scoreText;

    // UI text that shows the time left.
    [SerializeField] private Text timerText;

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
        // Set the timer to the starting value when the game begins.
        timeLeft = startTime;

        // Show the first score and timer values on screen.
        UpdateScoreText();
        UpdateTimerText();
    }

    private void Update()
    {
        // Stop updating when the game is over.
        if (!gameRunning)
        {
            return;
        }

        // Count down every frame.
        timeLeft -= Time.deltaTime;

        // Do not let the timer go below zero.
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            gameRunning = false;
        }

        // Refresh the timer text on screen.
        UpdateTimerText();
    }

    public bool IsGameRunning()
    {
        // Other scripts can ask if the game is still active.
        return gameRunning;
    }

    public void AddScore()
    {
        // Do not change the score after the timer ends.
        if (!gameRunning)
        {
            return;
        }

        // Add 1 point and update the screen.
        score += 1;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // Show the score in a simple beginner-friendly format.
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateTimerText()
    {
        // Show the timer as a whole number.
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft);
        }
    }
}
