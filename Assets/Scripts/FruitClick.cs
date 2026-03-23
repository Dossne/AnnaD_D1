using System.Collections;
using UnityEngine;

public class FruitClick : MonoBehaviour
{
    [SerializeField] private float autoMoveDelay = 2f;
    [SerializeField] private float eatDuration = 0.15f;
    [SerializeField] private float eatenScaleMultiplier = 0.2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip eatSound;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private float moveTimer;
    private Vector3 normalScale;
    private bool isEating;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        normalScale = transform.localScale;
        ResetMoveTimer();
    }

    private void Update()
    {
        if (isEating)
        {
            return;
        }

        if (gameManager != null && !gameManager.IsGameRunning())
        {
            return;
        }

        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0f)
        {
            MoveToRandomPosition();
            ResetMoveTimer();
        }
    }

    private void OnMouseDown()
    {
        if (isEating)
        {
            return;
        }

        if (gameManager != null && !gameManager.IsGameRunning())
        {
            return;
        }

        Debug.Log("Fruit clicked");

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is NULL");
        }
        else if (eatSound == null)
        {
            Debug.LogError("EatSound is NULL");
        }
        else
        {
            audioSource.clip = eatSound;
            audioSource.Play();
        }

        if (gameManager != null)
        {
            gameManager.AddScore();
        }

        StartCoroutine(PlayEatenEffect());
    }

    public void ResetFruit()
    {
        StopAllCoroutines();
        isEating = false;
        transform.localScale = normalScale;
        ResetMoveTimer();
        MoveToRandomPosition();
    }

    private IEnumerator PlayEatenEffect()
    {
        isEating = true;

        Vector3 startScale = normalScale;
        Vector3 targetScale = normalScale * eatenScaleMultiplier;

        float elapsedTime = 0f;

        while (elapsedTime < eatDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / eatDuration;
            progress = Mathf.Clamp01(progress);

            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            yield return null;
        }

        transform.localScale = targetScale;
        MoveToRandomPosition();
        transform.localScale = normalScale;
        ResetMoveTimer();
        isEating = false;
    }

    private void MoveToRandomPosition()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 newPosition = GetRandomPositionInsideScreen();
        transform.position = newPosition;
    }

    private void ResetMoveTimer()
    {
        moveTimer = Mathf.Max(0.1f, autoMoveDelay);
    }

    private Vector3 GetRandomPositionInsideScreen()
    {
        float distanceFromCamera = transform.position.z - mainCamera.transform.position.z;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));

        float halfWidth = 0f;
        float halfHeight = 0f;

        if (spriteRenderer != null)
        {
            halfWidth = spriteRenderer.bounds.extents.x;
            halfHeight = spriteRenderer.bounds.extents.y;
        }

        float minX = bottomLeft.x + halfWidth;
        float maxX = topRight.x - halfWidth;
        float minY = bottomLeft.y + halfHeight;
        float maxY = topRight.y - halfHeight;

        if (minX > maxX)
        {
            minX = maxX = (bottomLeft.x + topRight.x) * 0.5f;
        }

        if (minY > maxY)
        {
            minY = maxY = (bottomLeft.y + topRight.y) * 0.5f;
        }

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, transform.position.z);
    }
}