using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text highScoreText;

    //this is for game over screen
    public TMP_Text scoreText;

    //this is for the game score
    public TMP_Text gameScore;

    public Animator titleAnimator;
    public Animator backgroundAnimator;


    //true = started, false = ended
    public static string gameStatus;
    public static float finalScore;

    public GameObject startScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    public GameObject newspaperImage;
    public GameObject uiElements;

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
            //StartCoroutine(PlayTitleAnimation());
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
        scoreText.text = Mathf.FloorToInt(finalScore).ToString();
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
        Time.timeScale = 0;
    }

    public IEnumerator PlayTitleAnimation()
    {
        while(true) {
            // Play the animation
            titleAnimator.Play("titleAnim");
            //Debug.Log(titleAnimator.GetCurrentAnimatorStateInfo(0).length);

            // Wait for the animation to finish (assuming animation length is 1 second, replace with actual length if different)
            yield return new WaitForSecondsRealtime(1f);

            // Wait for an additional 0.5 seconds before replaying
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}