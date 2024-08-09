using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class DroneMovement : MonoBehaviour
    {
        public float moveSpeed = 0.5f; // Base speed of movement
        public float moveDistance = 0.1f; // Maximum random distance to move per update

        private Vector3 startPosition;

        void Start()
        {
            startPosition = transform.position; // Store the initial position
            StartCoroutine(RandomMovement());
        }

        IEnumerator RandomMovement()
        {
            while (true)
            {
                // Randomly decide on a small movement in either x or y direction
                float randomX = Random.Range(-moveDistance, moveDistance);
                float randomY = Random.Range(-moveDistance, moveDistance);

                // Calculate the new position with small random adjustments
                Vector3 newPosition = new Vector3(startPosition.x + randomX, startPosition.y + randomY, startPosition.z);

                // Smoothly move to the new position
                float elapsedTime = 0f;
                while (elapsedTime < moveSpeed)
                {
                    transform.position = Vector3.Lerp(transform.position, newPosition, (elapsedTime / moveSpeed));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Pause briefly before the next movement
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f)); // Adjust the time range as needed
            }
        }
    }