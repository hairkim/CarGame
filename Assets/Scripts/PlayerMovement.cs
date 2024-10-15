using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public UIController uiController;
    public GameController gameController;

    Rigidbody2D rb;
    Animator _animator;

    public float moveSpeedLeft = 40f;
    public float moveSpeedRight = 40f;
    public float accelerationRate = 70f;
    public float maxSpeed = 150f;
    public float defaultSpeed = 40f;

    public bool isFacingRight;
    public bool isFacingLeft;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isFacingLeft = false;
        isFacingRight = false;
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //move left
        if(Input.GetKey(KeyCode.A))
        {
            _animator.SetBool("moveLeft", true);
            moveSpeedLeft = Mathf.Min(moveSpeedLeft + accelerationRate * Time.deltaTime, maxSpeed);
            rb.AddForce(Vector2.left * moveSpeedLeft * Time.deltaTime, ForceMode2D.Impulse);
            //AudioController.instance.PlaySkrtSound();
        }
        else if(Input.GetKeyUp(KeyCode.A))
        {
            _animator.SetBool("moveLeft", false);
            moveSpeedLeft = defaultSpeed;
        }

        //move right
        if (Input.GetKey(KeyCode.D))
        {
            _animator.SetBool("moveRight", true);
            moveSpeedRight = Mathf.Min(moveSpeedRight + accelerationRate * Time.deltaTime, maxSpeed);
            rb.AddForce(Vector2.right * moveSpeedRight * Time.deltaTime, ForceMode2D.Impulse);
            //AudioController.instance.PlaySkrtSound();
        }
        else if(Input.GetKeyUp(KeyCode.D))
        {
            _animator.SetBool("moveRight", false);
            moveSpeedRight = defaultSpeed;
        }


        //do the face switching
        //if(Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    isFacingLeft = !isFacingLeft;
        //    //do animations
        //    _animator.SetBool("isFacingLeft", isFacingLeft);
        //    Debug.Log("currently facing left: " + isFacingLeft);
        //}

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            isFacingLeft = true;
            Debug.Log("facing left");
            //do animation
            _animator.SetBool("isFacingLeft", isFacingLeft);
        } else if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isFacingLeft = false;
            _animator.SetBool("isFacingLeft", isFacingLeft);
            Debug.Log("not facing left");
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            isFacingRight = true;
            Debug.Log("facing right");
            //do animation
            _animator.SetBool("isFacingRight", isFacingRight);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            isFacingRight = false;
            _animator.SetBool("isFacingRight", isFacingRight);
            Debug.Log("not facing left");
        }

    }

    private void OnBecameInvisible()
    {
        gameController.checkHighScore();
        uiController.GameOver();
    }
}
