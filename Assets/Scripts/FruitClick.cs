using UnityEngine;

public class FruitClick : MonoBehaviour
{
    // Time in seconds before the fruit moves by itself.
    [SerializeField] private float autoMoveDelay = 2f;

    // We keep a reference to the main camera so we can read the visible screen area.
    private Camera mainCamera;

    // We use the SpriteRenderer to find out how big the fruit looks on screen.
    private SpriteRenderer spriteRenderer;

    // This timer counts down until the fruit should move automatically.
    private float moveTimer;

    private void Awake()
    {
        // Find the main camera once when the object is created.
        mainCamera = Camera.main;

        // Find the SpriteRenderer on this object.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start the timer.
        ResetMoveTimer();
    }

    private void Update()
    {
        // Count down every frame.
        moveTimer -= Time.deltaTime;

        // When the timer reaches zero, move the fruit and start the timer again.
        if (moveTimer <= 0f)
        {
            MoveToRandomPosition();
            ResetMoveTimer();
        }
    }

    private void OnMouseDown()
    {
        // This method is called by Unity when the player clicks this object.
        // It works when the object has a Collider or Collider2D.

        // Show a message in the Console so we know the click worked.
        Debug.Log("Fruit clicked");

        // Move the fruit right away after the click.
        MoveToRandomPosition();

        // Start the timer again so the fruit waits before moving automatically.
        ResetMoveTimer();
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

        // Move the fruit to the new random position.
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
        // This keeps the full fruit inside the screen instead of letting it go off the edge.
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

        // If the fruit is larger than the visible area, fall back to the screen center on that axis.
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

        // Keep the fruit on the same Z layer.
        return new Vector3(randomX, randomY, transform.position.z);
    }
}
