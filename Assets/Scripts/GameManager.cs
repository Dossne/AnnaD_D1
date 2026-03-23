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

    private void Awake()
    {
        // If the text fields were not assigned in the Inspector,
        // try to find them or create them automatically.
        SetupUiIfNeeded();
    }

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

    private void SetupUiIfNeeded()
    {
        // Try to reuse existing UI objects first.
        if (scoreText == null)
        {
            scoreText = FindTextByName("ScoreText");
        }

        if (timerText == null)
        {
            timerText = FindTextByName("TimerText");
        }

        // If both text objects already exist, there is nothing else to do.
        if (scoreText != null && timerText != null)
        {
            return;
        }

        // Find or create a canvas so the text can be visible on screen.
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        // Create the score label only if it does not already exist.
        if (scoreText == null)
        {
            scoreText = CreateText(canvas.transform, "ScoreText", new Vector2(20f, -20f), TextAnchor.UpperLeft);
        }

        // Create the timer label only if it does not already exist.
        if (timerText == null)
        {
            timerText = CreateText(canvas.transform, "TimerText", new Vector2(-20f, -20f), TextAnchor.UpperRight);
        }
    }

    private Text FindTextByName(string objectName)
    {
        GameObject textObject = GameObject.Find(objectName);

        if (textObject == null)
        {
            return null;
        }

        return textObject.GetComponent<Text>();
    }

    private Text CreateText(Transform parent, string objectName, Vector2 anchoredPosition, TextAnchor alignment)
    {
        // Create a simple legacy UI Text object.
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(220f, 40f);

        // Put score on the top-left and timer on the top-right.
        if (alignment == TextAnchor.UpperLeft)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
        }

        rectTransform.anchoredPosition = anchoredPosition;

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 28;
        text.alignment = alignment;
        text.color = Color.black;

        return text;
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
