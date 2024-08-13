using UnityEngine;

public class FollowCameraUI : MonoBehaviour
{
    public Camera mainCamera; // Assign the Main Camera here

    void LateUpdate()
    {
        // Set the position to be slightly in front of the camera
        Vector3 cameraPosition = mainCamera.transform.position + mainCamera.transform.forward * 1.0f; // Adjust distance as needed
        transform.position = cameraPosition;

        // Optionally match the rotation to keep the UI facing the player
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}