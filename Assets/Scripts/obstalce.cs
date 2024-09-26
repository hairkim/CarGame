using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Outlet
    Rigidbody2D _rb;

    // Sprite randomization
    public Sprite[] obstacleSprites; // Array to store different obstacle sprites
    private SpriteRenderer spriteRenderer;

    // Scaling variables
    public float initialScale = 1.5f; // Initial scale when the obstacle spawns
    public float minScale = 0.5f; // Minimum scale at the top of the screen
    public float maxScale = 1.5f; // Maximum scale at the bottom of the screen
    public float topScreenY = 10f; // Y position representing the top of the screen
    public float bottomScreenY = -10f; // Y position representing the bottom of the screen

    // Lane switching variables
    public float switchInterval = 2f; // Time interval between lane switches
    public float laneSwitchSpeed = 1f; // Speed of switching lanes
    private float switchTimer;
    private float[] lanePositions = { -5.69f, -4.54f, -3.24f, -2.26f }; // X positions of lanes
    private int currentLane;

    private float timeElapsed;
    public float maxInterval = 1f;
    public float minInterval = 0.5f;
    private float delay = 60f;

    // Methods
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Randomize the sprite
        RandomizeSprite();

        currentLane = System.Array.IndexOf(lanePositions, transform.position.x);
        switchTimer = switchInterval;
        
        // Set the initial scale
        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        timeElapsed = 0;
    }

    void Update()
    {
        //timeElapsed += Time.deltaTime;

        // Move the obstacle down the screen
        _rb.velocity = Vector2.down * GameController.speed;

        // Calculate the normalized position of the object between the top and bottom of the screen
        float normalizedPosition = Mathf.InverseLerp(topScreenY, bottomScreenY, transform.position.y);

        // Calculate the scale factor based on the normalized position
        float scaleFactor = Mathf.Lerp(minScale, maxScale, normalizedPosition);

        // Apply the scaling factor to the object's local scale
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Update the switch timer
        switchTimer -= Time.deltaTime;
        if (switchTimer <= 0)
        {
            //this should randomly decide if the car will swap lanes or not
            bool swapLane = Random.Range(0, 2) == 1;
            if(swapLane)
            {
                // If on the leftmost side, change lanes to the right
                if (currentLane == 0)
                {
                    currentLane = Random.Range(0, 2);
                }
                else if (currentLane == (lanePositions.Length - 1))
                {
                    currentLane = Random.Range(lanePositions.Length - 2, lanePositions.Length);
                }
                else
                {
                    // Switch to a new lane
                    int newLane = Random.Range(0, 2) * 2 - 1;
                    currentLane += newLane;
                }
            }
            switchTimer = switchInterval;
        }

        // Smoothly move towards the current lane position
        Vector2 targetPosition = new Vector2(lanePositions[currentLane], transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);

        //decrease the interval time so they switch faster
        //float decreaseIntervalTime = minInterval + ((maxInterval - minInterval) / delay * timeElapsed);
        //switchInterval = Mathf.Clamp(decreaseIntervalTime, minInterval, maxInterval);
    }

    void RandomizeSprite()
    {
        if (obstacleSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, obstacleSprites.Length);
            spriteRenderer.sprite = obstacleSprites[randomIndex];
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call the OnPlayerHit method on the PlayerRecovery script
            collision.gameObject.GetComponent<PlayerRecovery>().OnPlayerHit();
        }
    }
}
