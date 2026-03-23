using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    }

    private void OnGUI()
    {
        // Draw the score and timer directly on the screen.
        // This keeps the setup simple because we do not need extra UI objects.
        GUI.Label(new Rect(20f, 20f, 200f, 30f), "Score: " + score);
        GUI.Label(new Rect(20f, 55f, 200f, 30f), "Time: " + Mathf.CeilToInt(timeLeft));
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

        // Add 1 point.
        score += 1;
    }
}
