using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour 
{
    public float jumpForce = 5;             //Player jump force;
    public float gravity = 25;              //Gravity value;
    public float moveSpeed = 10;            //Start movement speed;
    public float maxMoveSpeed = 25;         //Max movement speed;
    public float increaseValue = 1;         //Increse movement speed value. We adding this value to the player speed every second;

    public bool enableJump = true;             //Jump toogle. Jumping possibility is depends on this;
    public float swapOffset = 0;            //Swap postions are calculating automatically depending on player and platform colliders height,
                                            //but we can further adjust it with this value; Usually zero value is ok.
    public bool disableOnDeath;             //Hide player afret death or not. This toogle is usefull with non animated player characters;
    public ParticleSystem deathEffect;      //Death effect particle;

    public AudioClip jumpSFX;               //Jump sound effect;
    public AudioClip swapSFX;               //Swap line sound effect;

    private bool bottom;
    private bool grounded;
    private bool dead;

    private float speed;
    private float jumpDir;
    private float swapDifference;
    private float swapDistance;
    private float velocity;

    private Rigidbody2D rb2D;
    private Transform thisT;
    private Vector3 prevPos;
    private Vector3 defaultScale;
    private Vector2 moveDir;
    private GameManager GM;
    private BoxCollider2D playerCol;
    private AudioSource audioSource;


    void OnEnable()
    {
        //Setting player tag.
        gameObject.tag = "Player";
        //Setting line position to top;
        bottom = false;
    }

	// Use this for initialization
	void Start () 
    {
        //Caching components;
        playerCol = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        thisT = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        GM = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

        //Set player default scale value to the player start scale (for swap lines purposes);
        defaultScale = thisT.localScale;
        //Calculate swap position difference depends on Player's and platform's colliders height. Also we are adding swapOffset value.
        swapDifference = playerCol.bounds.size.y + GM.PlatformHeight() + swapOffset;
        //Setting speed to start move speed;
        speed = moveSpeed;
        //Setting up gravity;
        Physics2D.gravity = new Vector2(0, -gravity);
	}
	
	// Update is called once per frame
	void Update () 
    {
        //If game is over, do nothing;
        if(GameManager.isGameOver)
            return;
        //Changing player scale based on the current moving line (bottom or not);
        thisT.localScale = bottom ? new Vector3(defaultScale.x, -defaultScale.y, defaultScale.z) : defaultScale;
        //Inverse jump direction based on the current moving line (bottom or not);
        jumpDir = rb2D.gravityScale = bottom ? -1 : 1;
        //Inverse swap distance;
        swapDistance = bottom ? -swapDifference : swapDifference;

        //If player is grounded, check for Inputs
        if (grounded)
        {
            if (PlayerInput.Swap)
                Swap();
            else if (PlayerInput.Jump && enableJump)
                Jump();
        }

        //By default, game using Edge colliders for ground platforms. In case you need to use box colliders, this lines of code allows to avoid Box2D bug
        //with ghost vertices and player stucking by checking velocity and pushing player's transform to move dir;
        if (velocity == 0)
            transform.position += Vector3.right * 0.01F;
	}

    //Jump function;
    void Jump()
    {
        Utilities.PlaySFX(audioSource, jumpSFX);                                //Play jump sound effect;
        rb2D.AddForce(Vector3.up * jumpForce * jumpDir, ForceMode2D.Impulse);   //Adding jump force;
        grounded = false;                                                       //Setting grounded flag to false;
    }
    
    //Swap function;
    void Swap()
    {
        Utilities.PlaySFX(audioSource, swapSFX);    //Play swap sound effect;
        bottom = !bottom;                           //Change line flag;
        SwapPosition();                             //Swap player's position;
    }

    void FixedUpdate()
    {
        //Calculate transform's velocity;
        velocity = ((thisT.position - prevPos).magnitude) / Time.deltaTime;
        prevPos = transform.position;

        //Increase speed over time;
        if (speed < maxMoveSpeed)
            speed += increaseValue * Time.deltaTime;
        //Seting move direction based on dead flag;
        moveDir = !dead ? new Vector2(speed * 10 * Time.deltaTime, rb2D.velocity.y) : new Vector2(0, rb2D.velocity.y);
        //Assign move direction to rigidbody;
        rb2D.velocity = moveDir;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //When collison happens and collision object isn't obstacle, set grounded flag to true;
        grounded = true;
        //Otherwise
        if (col.gameObject.CompareTag("Obstacle") && !dead)
        {
            //Call game over function on GameManager;
            GM.SetGameOver();
            //If death effetc particle exist, instatiate it at collision point;
            if(deathEffect)
                Instantiate(deathEffect, col.contacts[0].point, Quaternion.identity);
            //Disable player object depends on 'disableOnDeath' flag;
            gameObject.SetActive(!disableOnDeath);
            //Setting 'dead' flag to true;
            dead = true;
        }
    }

    //Swap position function;
    void SwapPosition()
    {
        thisT.position = new Vector3(thisT.position.x, thisT.position.y - swapDistance, thisT.position.z);
    }
    
    //Reset player function. We are calling this function on the game restart;
    public void Reset()
    {
        //Setting line to top;
        bottom = false;
        //Reset velocity;
        rb2D.velocity = Vector2.zero;
        //Reset speed;
        speed = moveSpeed;
        //Set dead flag to true;
        dead = false;
    }

    //Public functions, allow us to read private player's states from other scripts;
    public bool IsGrounded()
    {
        return grounded;
    }
    public bool IsDead()
    {
        return dead;
    }
    public bool IsBot()
    {
        return bottom;
    }
}
