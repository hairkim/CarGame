using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {
    public static GameController instance;
    public static bool isCameraShown = false;
    public static float speed;

    //Outlets
    public Transform[] obstacleSpawnPoints;
    public Transform[] cameraSpawnPoints;
    public GameObject[] obstaclePrefabs;
    public GameObject cameraPrefab;

    //for tracking player score
    public TMP_Text textScore;
    public float score;

    public float maxCameraDelay = 5f;
    public float minCameraDelay = 1f;
    public float cameraDelay;

    public float maxObstacleDelay = 3f;
    public float minObstacleDelay = 0.5f;

    public float obstacleDelay;

    public float timeElapsed = 0f;

    //this is for obstacles
    public float minSpeed = 5f;
    public float maxSpeed = 10f;


    public float delay = 30f;
    public float speedDelay = 45f;

    void Awake() {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine("CameraSpawnTimer");
        StartCoroutine("ObstacleSpawnTimer");
        isCameraShown = false;
        score = 0f;
    }

    void Update() {
        //Increment passage of time for each frame of the game
        timeElapsed += Time.deltaTime;

        //Computer Asteroid Delay
        float decreaseDelayOverTime = maxCameraDelay - ((maxCameraDelay - minCameraDelay) / delay * timeElapsed);
        //Debug.Log("decreaseDelayOverTime: " + decreaseDelayOverTime);
        cameraDelay = Mathf.Clamp(decreaseDelayOverTime, minCameraDelay, maxCameraDelay);

        //Obstacle Delay
        float decreaseDelayOverTimeObstacle = maxObstacleDelay - ((maxObstacleDelay - minObstacleDelay) / delay * timeElapsed);
        obstacleDelay = Mathf.Clamp(decreaseDelayOverTimeObstacle, minObstacleDelay, maxObstacleDelay);


        //calculate obstacle speed and increase over time
        //this gets used in obstacle.cs
        float increaseTime = minSpeed + ((maxSpeed - minSpeed) / speedDelay * timeElapsed);
        speed = Mathf.Clamp(increaseTime, minSpeed, maxSpeed);

        if (Time.timeScale > 0)
        {
            score += (Time.deltaTime * 100);
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        textScore.text = Mathf.FloorToInt(score).ToString();
    }

    void SpawnObstacle() {
        int randomObstacleSpawnIndex = Random.Range(0, obstacleSpawnPoints.Length);
        Transform randomObstacleSpawnPoint = obstacleSpawnPoints[randomObstacleSpawnIndex];
        int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject randomObstaclePrefab = obstaclePrefabs[randomObstacleIndex];

        Instantiate(randomObstaclePrefab, randomObstacleSpawnPoint.position, Quaternion.identity);
    }

    void SpawnCamera()
        {
            int randomSpawnIndex = Random.Range(0, cameraSpawnPoints.Length);
            Transform randomSpawnPoint = cameraSpawnPoints[randomSpawnIndex];
            GameObject spawnedCamera = Instantiate(cameraPrefab, randomSpawnPoint.position, Quaternion.identity);

            // Assuming you have two spawn points, index 0 is the left side, and index 1 is the right side.
            if (randomSpawnIndex == 1) // Right side
            {
                // Rotate the camera to face left
                spawnedCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else // Left side
            {
                // No need to rotate, as it already looks to the left
                spawnedCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }


    IEnumerator CameraSpawnTimer()
    {
        while(true)
        {
            if (!isCameraShown)
            {
                isCameraShown = true;
                yield return new WaitForSeconds(cameraDelay);
                SpawnCamera();
            }
            else
            {
                Debug.Log("camera is already being shown");
                yield return null;
            }
        }
    }

    IEnumerator ObstacleSpawnTimer() {
        while(true)
        {
            yield return new WaitForSeconds(obstacleDelay);
            SpawnObstacle();
        }
        //StartCoroutine("ObstacleSpawnTimer");
    }

    public void EarnPoints(int pointAmount)
    {
        score += pointAmount;
        UpdateDisplay();
    }
}
