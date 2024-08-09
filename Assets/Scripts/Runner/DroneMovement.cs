using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public float moveSpeed = 0.1f; // Speed of movement, higher value for smoother motion
    public float moveDistance = 0.05f; // Maximum random distance to move per update
    public float smoothTime = 0.5f; // Smoothing time for the movement
    public Vector3 tileBoundsMin; // Minimum bounds of the tile
    public Vector3 tileBoundsMax; // Maximum bounds of the tile
    public float margin = 0.2f; // Margin to keep drones away from tile edges

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero; // Used for smoothing

    void Start()
    {
        targetPosition = transform.position; // Initialize the target position to the starting position
        StartCoroutine(RandomMovement());
    }

    IEnumerator RandomMovement()
    {
        while (true)
        {
            // Randomly decide on a small movement in either x or y direction
            float randomX = Random.Range(-moveDistance, moveDistance);
            float randomY = Random.Range(-moveDistance, moveDistance);

            // Calculate the new target position with small random adjustments
            Vector3 potentialPosition = new Vector3(
                transform.position.x + randomX,
                transform.position.y + randomY,
                transform.position.z
            );

            // Ensure the target position is within the tile bounds
            targetPosition = new Vector3(
                Mathf.Clamp(potentialPosition.x, tileBoundsMin.x, tileBoundsMax.x),
                Mathf.Clamp(potentialPosition.y, tileBoundsMin.y, tileBoundsMax.y),
                potentialPosition.z
            );

            // Smoothly move towards the target position using SmoothDamp
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                yield return null;
            }

            // Pause briefly before the next movement
            yield return new WaitForSeconds(Random.Range(1f, 2f)); // Adjust the time range as needed
        }
    }
}