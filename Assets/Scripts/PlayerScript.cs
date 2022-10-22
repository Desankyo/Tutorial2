using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float speed;
    public float jump;
    private bool facingRight = true;

    //Variables
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;

    // For Texts
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI LivesText;
    private int scoreValue;
    private int livesValue;

    public GameObject WinTextObject;
    public GameObject LoseTextObject;

    //Animations
    public Animator anim;
    

    // Start is called before the first frame update
    void Start()
    {
        //coins and win text
        rd2d = GetComponent<Rigidbody2D>();
        scoreValue = 0;

        SetCountText();
        WinTextObject.SetActive(false);
    
        //lives and lose text
        rd2d = GetComponent<Rigidbody2D>();
        livesValue = 3;

        SetCountText();
        LoseTextObject.SetActive(false);

        //Player animations
        anim = GetComponent<Animator>();
        
    }

    void SetCountText()
    {
        //Increase score
        scoreText.text = "Score: " + scoreValue.ToString();
        if(scoreValue >= 8)
        {
            WinTextObject.SetActive(true);

            //For sound
            MusicScript.PlaySound("Win");
        }

        //Player teleport and lives reset
        if (scoreValue == 4)
        {
            livesValue = 3;
            
            transform.position = new Vector2(90f, 1f);
            
                
        }

        //Decrease lives
        LivesText.text = "Lives: " + livesValue.ToString();
        if(livesValue == 0)
        {
            LoseTextObject.SetActive(true);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));

        //To flip the character
        if (hozMovement > 0 && !facingRight)
        {
            Flip();
        }
        else if (hozMovement < 0 && facingRight)
        {
            Flip();
        }

        //For running animation
       
        if (hozMovement == 0)
        {
            anim.SetBool("IsRunning", false);
        }
        else
        {
            anim.SetBool("IsRunning", true);
        }

        //For jumping animation
        if (vertMovement == 0)
        {
            anim.SetBool("IsJumping", false);
        }

        else if (vertMovement > 0)
        {
            anim.SetBool("IsJumping", true);
        }


        //Ground Check
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    }

    //To optimaze the flip so it is not constantly checking every frame for it
    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Coins pickups
       if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            SetCountText();
            Destroy(collision.collider.gameObject);
        }

        //Enemy Pickups
       if (collision.collider.tag == "Enemy")
        {
            livesValue = livesValue - 1;
            SetCountText();
            Destroy(collision.collider.gameObject);
        }
        //Destroy player after losing
        if (livesValue <= 0)
        {
            Destroy(gameObject);
        }
        //Stop player after winning
        if (scoreValue == 8)
        {
            GetComponent<PlayerScript>().enabled = false;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Jump
        if (collision.collider.tag == "Ground" && isTouchingGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, jump), ForceMode2D.Impulse); 
            }
        }
    }

    //Exiting game
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

