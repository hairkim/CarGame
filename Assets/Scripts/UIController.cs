using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text highScoreText;
    public TMP_Text scoreText; // For the game over screen score display
    public TMP_Text gameScore; // For the in-game score display

    public Animator titleAnimator;
    public Animator backgroundAnimator;

    public static string gameStatus;
    public static float finalScore;

    public GameObject startScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    public GameObject newspaperImage;
    public GameObject uiElements;

    private float animationDuration = 1.0f; // Duration for the score animation

    void Start()
    {
        if (gameStatus == "Restart")
        {
            startScreen.SetActive(false);
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            uiElements.SetActive(false);
            gameScore.gameObject.SetActive(true);
            Time.timeScale = 1;
            gameStatus = "Playing";
        }
        else
        {
            startScreen.SetActive(true);
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            gameScore.gameObject.SetActive(false);
            Time.timeScale = 0;
            titleAnimator.Play("titleAnim");
        }
        Debug.Log("Current high score: " + PlayerPrefs.GetFloat("HighScore", 0f).ToString());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStatus != "Pause")
            {
                PauseGame();
            }
        }

        gameScore.text = Mathf.FloorToInt(GameController.score).ToString();
    }

    public void StartGame()
    {
        Debug.Log("Button Clicked!");
        Time.timeScale = 1;
        startScreen.SetActive(false);
        gameScore.gameObject.SetActive(true);
        gameStatus = "Playing";
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        gameStatus = "Pause";
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        gameStatus = "Playing";
    }

    public void RestartGame()
    {
        gameStatus = "Restart";
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        gameStatus = "Quit";
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        // Start the score animation coroutine

        // Display high score immediately
        highScoreText.text = Mathf.FloorToInt(PlayerPrefs.GetFloat("HighScore", 0f)).ToString();

        gameScore.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        uiElements.SetActive(false);

        Animator animator = newspaperImage.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("ShowNewspaper");
            StartCoroutine(WaitForAnimation(animator, "NewspaperAnimator"));
        }


    }

    private IEnumerator WaitForAnimation(Animator animator, string animationName)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        uiElements.SetActive(true);

        StartCoroutine(AnimateScore(finalScore));

    }

    // Coroutine to animate the score from 0 to the final score
    private IEnumerator AnimateScore(float finalScore)
    {
        float elapsedTime = 0f;
        float currentScore = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            currentScore = Mathf.Lerp(0, finalScore, elapsedTime / animationDuration);
            scoreText.text = Mathf.FloorToInt(currentScore).ToString(); // Update score text as it increments
            yield return null;
        }

        // Ensure final score is displayed at the end of the animation
        scoreText.text = Mathf.FloorToInt(finalScore).ToString();
        Time.timeScale = 0;
    }
}
