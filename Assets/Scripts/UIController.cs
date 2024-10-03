using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    public GameObject startScreenOptions;
    public GameObject newspaperImage;
    public GameObject uiElements;
    public Sprite newsImage;
    public GameObject optionsUIElements;
    public Sprite uncheckedSprite;
    public Sprite checkedSprite;
    private bool isSfxButtonPressed = false;
    private bool isMusicButtonPressed = false;
    

    private Animator optionsAnimator;
    private RectTransform rectTransform;
    private bool isOptionsOpen;


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
            AudioController.instance.PlayMusic("InGame");
        }
        else
        {
            isOptionsOpen = false;
            optionsAnimator = startScreenOptions.GetComponent<Animator>();
            rectTransform = startScreenOptions.GetComponent<RectTransform>();

            startScreen.SetActive(true);
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            gameScore.gameObject.SetActive(false);
            Time.timeScale = 0;
            titleAnimator.Play("titleAnim");
            backgroundAnimator.Play("backgroundAnimation");

            AudioController.instance.PlayMusic("MainMenu");
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
            } else
            {
                ResumeGame();
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
        //AudioController.instance.StopMusic();
        AudioController.instance.PlayMusic("InGame");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        gameStatus = "Pause";
        AudioController.instance.PauseMusicAndPlayPauseMenu();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        gameStatus = "Playing";
        AudioController.instance.PlayMusic("InGame");
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

    public void Options()
    {
        if(!isOptionsOpen)
        {
            isOptionsOpen = true;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = 59f;
            rectTransform.sizeDelta = sizeDelta;
            if (optionsAnimator != null)
            {
                Debug.Log("playing animation");
                optionsAnimator.SetTrigger("playOptions");
                StartCoroutine(WaitForAnimationToEnd(optionsAnimator, "playOptions"));
            }
        }
    }

    public void CloseOptions()
    {
        if (isOptionsOpen)
        {
            optionsUIElements.SetActive(false);
            isOptionsOpen = false;
            if (optionsAnimator != null)
            {
                Debug.Log("playing animation");
                optionsAnimator.SetTrigger("closeOptions");
                StartCoroutine(WaitForAnimationToEnd(optionsAnimator, "closeOptions"));
            }
        }
    }


    private IEnumerator WaitForAnimationToEnd(Animator animator, string animationName)
    {
        // Get the current animation clip info to calculate the duration
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = animatorStateInfo.length;

        float elapsedTime = 0f;
        while (elapsedTime < animationLength)
        {
            // We add unscaled delta time to the elapsed time
            elapsedTime += Time.unscaledDeltaTime;

            yield return null;  // Wait for the next frame
        }

        // Now execute the rest of the code after the animation has finished
        if(isOptionsOpen)
        {
            startScreenOptions.GetComponent<Button>().interactable = false;
            optionsUIElements.SetActive(true);
        } else {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = 17f;
            rectTransform.sizeDelta = sizeDelta;
            Debug.Log("changed rect transform height");
            startScreenOptions.GetComponent<Button>().interactable = true;
        }

    }

    public void GameOver()
    {
        //AudioController.instance.StopMusic();
        AudioController.instance.PlayMusic("GameOver");
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

            newspaperImage.GetComponent<Image>().sprite = newsImage;
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

    public void sfxButtonPressed(Button button)
    {
        isSfxButtonPressed = !isSfxButtonPressed;
        if(isSfxButtonPressed)
        {
            //turn off sfx audio
            button.image.sprite = checkedSprite;
        } else
        {
            button.image.sprite = uncheckedSprite;
        }
    }

    public void MusicButtonPressed(Button button)
    {
        isMusicButtonPressed = !isMusicButtonPressed;
        if (isMusicButtonPressed)
        {
            //turn off music audio
            button.image.sprite = uncheckedSprite;
        }
        else
        {
            button.image.sprite = checkedSprite;
        }
    }
}
