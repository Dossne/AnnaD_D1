using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI text that shows the current score.
    [SerializeField] private Text scoreText;

    // UI text that shows the time left.
    [SerializeField] private Text timerText;

    // UI text that shows the current lives.
    [SerializeField] private Text livesText;

    // UI text that appears when the round ends.
    [SerializeField] private Text gameOverText;

    // Button that starts the round again.
    [SerializeField] private Button restartButton;

    // How many seconds the game lasts.
    [SerializeField] private float startTime = 30f;

    // How many misses the player can make.
    [SerializeField] private int startLives = 3;

    // Main camera used to turn mouse position into world position.
    private Camera mainCamera;

    // Current score value.
    private int score;

    // Time left in the game.
    private float timeLeft;

    // Current lives value.
    private int lives;

    // True while the game is still running.
    private bool gameRunning;

    private void Start()
    {
        // Find the main camera once when the game starts.
        mainCamera = Camera.main;

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

        // Count a miss if the player clicks somewhere that is not a fruit.
        if (Input.GetMouseButtonDown(0))
        {
            CheckForMissClick();
        }

        // Stop here if the last click used up the final life.
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

        // Add 1 point and update the text on screen.
        score += 1;
        UpdateScoreText();
    }

    public void RestartRound()
    {
        // Reset the game values.
        score = 0;
        timeLeft = startTime;
        lives = Mathf.Max(0, startLives);
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

        // Show the fresh score, timer, and lives.
        UpdateScoreText();
        UpdateTimerText();
        UpdateLivesText();

        // Tell every fruit to start a new round too.
        FruitClick[] fruits = FindObjectsOfType<FruitClick>();
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i].ResetFruit();
        }
    }

    private void CheckForMissClick()
    {
        // If there is no camera, we cannot test the click position.
        if (mainCamera == null)
        {
            return;
        }

        // Turn the mouse position into a 2D world position.
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clickPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        // Look for all 2D colliders under the click.
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(clickPosition);

        // If any hit belongs to a fruit, this click is not a miss.
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponentInParent<FruitClick>() != null)
            {
                return;
            }
        }

        // No fruit was found under the click, so count it as a miss.
        LoseLife();
    }

    private void LoseLife()
    {
        // Do not change lives after the round is already over.
        if (!gameRunning)
        {
            return;
        }

        // Remove one life and update the screen.
        lives -= 1;
        UpdateLivesText();

        // End the game right away when no lives are left.
        if (lives <= 0)
        {
            lives = 0;
            UpdateLivesText();
            EndRound();
        }
    }

    private void EndRound()
    {
        // Mark the game as finished.
        gameRunning = false;

        // Make sure the timer shows zero when time has run out.
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

    private void UpdateLivesText()
    {
        // Show the remaining lives on screen.
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }
}
