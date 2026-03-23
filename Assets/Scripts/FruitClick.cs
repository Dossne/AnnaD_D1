using System.Collections;
using UnityEngine;

public class FruitClick : MonoBehaviour
{
    // Time in seconds before the food moves by itself.
    [SerializeField] private float autoMoveDelay = 2f;

    // How long the eaten effect lasts.
    [SerializeField] private float eatDuration = 0.15f;

    // How small the food becomes while being eaten.
    [SerializeField] private float eatenScaleMultiplier = 0.2f;

    // We keep a reference to the main camera so we can read the visible screen area.
    private Camera mainCamera;

    // We use the SpriteRenderer to find out how big the food looks on screen.
    private SpriteRenderer spriteRenderer;

    // We talk to the GameManager to add score and check if the game is still running.
    private GameManager gameManager;

    // This timer counts down until the food should move automatically.
    private float moveTimer;

    // The normal starting scale of the food.
    private Vector3 normalScale;

    // True while the eaten effect is playing.
    private bool isEating;

    private void Awake()
    {
        // Find the main camera once when the object is created.
        mainCamera = Camera.main;

        // Find the SpriteRenderer on this object.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the GameManager in the scene.
        gameManager = FindObjectOfType<GameManager>();

        // Save the normal scale so we can restore it later.
        normalScale = transform.localScale;

        // Start the timer.
        ResetMoveTimer();
    }

    private void Update()
    {
        // Do nothing while the eaten effect is playing.
        if (isEating)
        {
            return;
        }

        // Stop moving when the game is over.
        if (gameManager != null && !gameManager.IsGameRunning())
        {
            return;
        }

        // Count down every frame.
        moveTimer -= Time.deltaTime;

        // When the timer reaches zero, move the food and start the timer again.
        if (moveTimer <= 0f)
        {
            MoveToRandomPosition();
            ResetMoveTimer();
        }
    }

    private void OnMouseDown()
    {
        // This method is called by Unity when the player clicks this object.
        // It works when the object already has a Collider2D in the scene.

        // Ignore clicks while the eaten effect is already playing.
        if (isEating)
        {
            return;
        }

        // If the game has ended, ignore clicks.
        if (gameManager != null && !gameManager.IsGameRunning())
        {
            return;
        }

        // Show a message in the Console so we know the click worked.
        Debug.Log("Fruit clicked");

        // Add 1 point to the score.
        if (gameManager != null)
        {
            gameManager.AddScore();
        }

        // Play the eaten effect instead of moving right away.
        StartCoroutine(PlayEatenEffect());
    }

    public void ResetFruit()
    {
        // Stop any old eaten effect when the round restarts.
        StopAllCoroutines();

        // Make sure the food is back to its normal state.
        isEating = false;
        transform.localScale = normalScale;

        // Start a fresh timer for the new round.
        ResetMoveTimer();

        // Move the food to a new random position.
        MoveToRandomPosition();
    }

    private IEnumerator PlayEatenEffect()
    {
        // Mark that the food is currently being eaten.
        isEating = true;

        // Save the start and end scale for the animation.
        Vector3 startScale = normalScale;
        Vector3 targetScale = normalScale * eatenScaleMultiplier;

        float elapsedTime = 0f;

        // Shrink the food over a very short time.
        while (elapsedTime < eatDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / eatDuration;
            progress = Mathf.Clamp01(progress);

            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            yield return null;
        }

        // Make sure it finishes at the small scale.
        transform.localScale = targetScale;

        // Move the food after it has been "eaten".
        MoveToRandomPosition();

        // Restore the normal scale at the new position.
        transform.localScale = normalScale;

        // Start the move timer again.
        ResetMoveTimer();

        // The eaten effect is finished.
        isEating = false;
    }

    private void MoveToRandomPosition()
    {
        // If there is no main camera, we stop here to avoid errors.
        if (mainCamera == null)
        {
            return;
        }

        // Ask for a new random position that stays inside the camera view.
        Vector3 newPosition = GetRandomPositionInsideScreen();

        // Move the food to the new random position.
        transform.position = newPosition;
    }

    private void ResetMoveTimer()
    {
        // Make sure the timer never goes too low.
        moveTimer = Mathf.Max(0.1f, autoMoveDelay);
    }

    private Vector3 GetRandomPositionInsideScreen()
    {
        // Work out how far this object is from the camera.
        float distanceFromCamera = transform.position.z - mainCamera.transform.position.z;

        // Get the world position of the bottom-left and top-right corners of the screen.
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));

        // Start with no padding.
        float halfWidth = 0f;
        float halfHeight = 0f;

        // If the object has a SpriteRenderer, use half of its size as padding.
        // This keeps the full food inside the screen instead of letting it go off the edge.
        if (spriteRenderer != null)
        {
            halfWidth = spriteRenderer.bounds.extents.x;
            halfHeight = spriteRenderer.bounds.extents.y;
        }

        // Build safe movement limits.
        float minX = bottomLeft.x + halfWidth;
        float maxX = topRight.x - halfWidth;
        float minY = bottomLeft.y + halfHeight;
        float maxY = topRight.y - halfHeight;

        // If the food is larger than the visible area, fall back to the screen center on that axis.
        if (minX > maxX)
        {
            minX = maxX = (bottomLeft.x + topRight.x) * 0.5f;
        }

        if (minY > maxY)
        {
            minY = maxY = (bottomLeft.y + topRight.y) * 0.5f;
        }

        // Pick a random position inside those safe limits.
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        // Keep the food on the same Z layer.
        return new Vector3(randomX, randomY, transform.position.z);
    }
}