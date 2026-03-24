using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private float startTime = 30f;
    [SerializeField] private int startLives = 3;
    [SerializeField] private FruitSpawner fruitSpawner;

    private Camera mainCamera;
    private int score;
    private float timeLeft;
    private int lives;
    private bool gameRunning;

    private void Start()
    {
        mainCamera = Camera.main;

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartRound);
        }

        if (fruitSpawner == null)
        {
            fruitSpawner = FindObjectOfType<FruitSpawner>();
        }

        RestartRound();
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartRound);
        }
    }

    private void Update()
    {
        if (!gameRunning)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckForMissClick();
        }

        if (!gameRunning)
        {
            return;
        }

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndRound();
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

    public void RestartRound()
    {
        score = 0;
        timeLeft = startTime;
        lives = Mathf.Max(0, startLives);
        gameRunning = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        UpdateScoreText();
        UpdateTimerText();
        UpdateLivesText();

        FruitClick[] fruits = FindObjectsOfType<FruitClick>();
        for (int i = 0; i < fruits.Length; i++)
        {
            Destroy(fruits[i].gameObject);
        }

        if (fruitSpawner != null)
        {
            fruitSpawner.SpawnWave();
        }
    }

    private void CheckForMissClick()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clickPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        Collider2D[] hitColliders = Physics2D.OverlapPointAll(clickPosition);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponentInParent<FruitClick>() != null)
            {
                return;
            }
        }

        LoseLife();
    }

    private void LoseLife()
    {
        if (!gameRunning)
        {
            return;
        }

        lives -= 1;
        UpdateLivesText();

        if (lives <= 0)
        {
            lives = 0;
            UpdateLivesText();
            EndRound();
        }
    }

    private void EndRound()
    {
        gameRunning = false;
        UpdateTimerText();

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

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }
}