using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMoon : MonoBehaviour
{
    public float MoveSpeed = 100; // Speed of movement around the origin
    public float RotationSpeed = 50; // Rotation speed around its own axis

    void FixedUpdate()
    {
        // Rotation around its own axis
        transform.Rotate(Vector3.up, RotationSpeed * Time.fixedDeltaTime);

        // Calculate orbital position
        float angle = Time.fixedTime * MoveSpeed; // Use Time.fixedTime for consistent movement
        Vector3 orbitPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * transform.position.magnitude;

        // Move the planet
        transform.position = orbitPosition;
    }
}