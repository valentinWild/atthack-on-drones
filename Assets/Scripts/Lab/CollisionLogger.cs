using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLogger : MonoBehaviour
{
    // This will be called when the potion starts colliding with another object
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name} collided with {collision.gameObject.name} at {Time.time} seconds.");
    }

    // This will be called when the potion stays in contact with another object
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"{gameObject.name} is still colliding with {collision.gameObject.name} at {Time.time} seconds.");
    }

    // This will be called when the potion stops colliding with another object
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"{gameObject.name} stopped colliding with {collision.gameObject.name} at {Time.time} seconds.");
    }
}