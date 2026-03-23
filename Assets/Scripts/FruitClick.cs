using UnityEngine;

public class FruitClick : MonoBehaviour
{
    // We keep a reference to the main camera so we can convert screen positions to world positions.
    private Camera mainCamera;

    private void Awake()
    {
        // Find the main camera once when the object is created.
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        // This method is called by Unity when the player clicks this object.
        // It works when the object has a Collider or Collider2D.

        // Show a message in the Console so we know the click worked.
        Debug.Log("Fruit clicked");

        // If there is no main camera, we stop here to avoid errors.
        if (mainCamera == null)
        {
            return;
        }

        // Pick a random point on the screen using viewport coordinates.
        // 0 means the left/bottom side of the screen and 1 means the right/top side.
        float randomX = Random.Range(0.1f, 0.9f);
        float randomY = Random.Range(0.1f, 0.9f);

        // Work out how far this object is from the camera.
        // We use this distance so the new position stays visible.
        float distanceFromCamera = transform.position.z - mainCamera.transform.position.z;

        // Convert the random screen point into a world position.
        Vector3 newPosition = mainCamera.ViewportToWorldPoint(new Vector3(randomX, randomY, distanceFromCamera));

        // Keep the current Z position so the object stays on the same 2D layer.
        newPosition.z = transform.position.z;

        // Move the fruit to the new random position.
        transform.position = newPosition;
    }
}
