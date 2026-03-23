using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // UI text that shows the current score.
    [SerializeField] private TMP_Text scoreText;

    // UI text that shows the time left.
    [SerializeField] private TMP_Text timerText;

    // UI text that appears when the round ends.
    [SerializeField] private TMP_Text gameOverText;

    // Button that starts the round again.
    [SerializeField] private Button restartButton;

    // How many seconds the game lasts.
    [SerializeField] private float startTime = 30f;

    // Current score value.
    private int score;

    // Time left in the game.
    private float timeLeft;

    // True while the game is still running.
    private bool gameRunning;

    private void Start()
    {
        // Connect the button once so it can restart the round.
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartRound);
        }

        // Start the first round.
        RestartRound();
    }

    private void OnDestroy()
    {
        // Remove the button connection when this object is destroyed.
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartRound);
        }
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
            EndRound();
        }

        // Refresh the timer text on screen.
        UpdateTimerText();
    }

    public bool IsGameRunning()
    {
        return gameRunning;
    }

    public void AddScore()
    {
        // Do not change the score after the timer ends.
        if (!gameRunning)
        {
            return;
        }

        // Add 1 point and update the text on screen.
        score += 1;
        UpdateScoreText();
    }

    public void RestartRound()
    {
        // Reset the game values.
        score = 0;
        timeLeft = startTime;
        gameRunning = true;

        // Hide the end-of-round UI.
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        // Show the fresh score and timer.
        UpdateScoreText();
        UpdateTimerText();

        // Tell every fruit to start a new round too.
        FruitClick[] fruits = FindObjectsOfType<FruitClick>();
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i].ResetFruit();
        }
    }

    private void EndRound()
    {
        // Mark the game as finished.
        gameRunning = false;

        // Make sure the timer shows zero.
        UpdateTimerText();

        // Show the end-of-round UI.
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
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