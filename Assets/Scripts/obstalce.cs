using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle : MonoBehaviour
{
    // Outlet
    Rigidbody2D _rb;

    // Scaling variables
    public float minScale = 0.5f; // Minimum scale at the top of the screen
    public float maxScale = 1.5f; // Maximum scale at the bottom of the screen
    public float topScreenY = 10f; // Y position representing the top of the screen
    public float bottomScreenY = -10f; // Y position representing the bottom of the screen

    // Side-to-side movement variables
    public float frequency = 1f; // Frequency of side-to-side movement
    public float magnitude = 0.5f; // Amplitude of side-to-side movement
    private float initialXPosition;

    // Methods
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Save the initial X position
        initialXPosition = transform.position.x;
    }

    void Update()
    {
        // Move the obstacle down the screen
        _rb.velocity = Vector2.down * GameController.speed;

        // Calculate the normalized position of the object between the top and bottom of the screen
        float normalizedPosition = Mathf.InverseLerp(topScreenY, bottomScreenY, transform.position.y);

        // Calculate the scale factor based on the normalized position
        float scaleFactor = Mathf.Lerp(minScale, maxScale, normalizedPosition);

        // Apply the scaling factor to the object's local scale
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Calculate the new horizontal position using a sine wave for side-to-side movement
        float horizontalOffset = Mathf.Sin(Time.time * frequency) * magnitude;

        // Apply the new position
        transform.position = new Vector3(initialXPosition + horizontalOffset, transform.position.y, transform.position.z);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
