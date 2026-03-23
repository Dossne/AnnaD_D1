using UnityEngine;

public class FruitClick : MonoBehaviour
{
    [SerializeField] private float autoMoveDelay = 2f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private float moveTimer;
    private GameManager gameManager;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        ResetMoveTimer();
    }

    private void Update()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0f)
        {
            MoveToRandomPosition();
            ResetMoveTimer();
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Fruit clicked");

        if (gameManager != null && gameManager.IsGameRunning())
        {
            gameManager.AddScore();
        }

        MoveToRandomPosition();
        ResetMoveTimer();
    }

    private void MoveToRandomPosition()
    {
        if (mainCamera == null)
            return;

        Vector3 min = mainCamera.ViewportToWorldPoint(
            new Vector3(0f, 0f, Mathf.Abs(mainCamera.transform.position.z))
        );
        Vector3 max = mainCamera.ViewportToWorldPoint(
            new Vector3(1f, 1f, Mathf.Abs(mainCamera.transform.position.z))
        );

        float halfWidth = 0.5f;
        float halfHeight = 0.5f;

        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            halfWidth = bounds.extents.x;
            halfHeight = bounds.extents.y;
        }

        float randomX = Random.Range(min.x + halfWidth, max.x - halfWidth);
        float randomY = Random.Range(min.y + halfHeight, max.y - halfHeight);

        transform.position = new Vector3(randomX, randomY, transform.position.z);
    }

    private void ResetMoveTimer()
    {
        moveTimer = autoMoveDelay;
    }
}