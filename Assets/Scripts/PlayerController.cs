using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float gravityFactor = 0.2f;
    public float maxSpeed = 30f;
    private float maxSideSpeed = 0.06f;
    public float jumpSpeed = 2f;
    private float sideSpeed = 0f;
    public Camera cam;

    private const float baseGravity = 0.2f;
    private float currentSpeed = 0f;                        // Positive means going down
    private bool isInCollision = false;
    public float acceleration = 0.4f;
    public float sideSlowdownFactor = 1f;
    private float sideSpeedFactor = 0f;
    private SpriteRenderer sRenderer;
    private float spriteWidth;
    private float spriteHeight;
    private float leftScreenSide;
    private float rightScreenSide;
    private States currentState = States.START;

    private Animator animator;
    private Collider2D collider2D;
    private AudioSource audio;
    private GameObject startPlatform;
    private GameObject finishPlatform;

    // TEST-Variables
    public bool testBool = false;

    enum Action 
    {
        JUMP,
    }

    enum States
    {
        START,
        PLAYING,
        FINISH,
    }

    void Start()
    {
        // Set Start location
        transform.position = new Vector3(1f, 0f, 0f);
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
        audio = GetComponent<AudioSource>();
        sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x * transform.lossyScale.x;
        spriteHeight = sRenderer.sprite.bounds.size.y * transform.lossyScale.y;
        leftScreenSide = cam.ScreenToWorldPoint(new Vector3(-Screen.width/2, 0f, 0f)).x;
        rightScreenSide = cam.ScreenToWorldPoint(new Vector3(Screen.width/2, 0f, 0f)).x;

        startPlatform = GameObject.FindGameObjectWithTag("StartPlatform");
        finishPlatform = GameObject.FindGameObjectWithTag("FinishPlatform");

    }

    void FixedUpdate()
    {
        if (currentState == States.START)
        {
            HandleStart();
        }
        else if (currentState == States.FINISH)
        {
            HandleFinish();
        }
        HandleFalling();
        HandleSideMovement();
        HandleAnimations();

        if(Input.GetKey("o"))
        {
            currentState = States.FINISH;
        }

        
    }

    void HandleSideMovement()
    {
        if (Input.GetKey("d"))
        {
            sideSpeedFactor = sideSpeed < 0 ? 3 : 1;
            sideSpeed += acceleration  * sideSpeedFactor * Time.fixedDeltaTime;
            sideSpeed = LimitValue(sideSpeed, maxSideSpeed);
        }
        if (Input.GetKey("a"))
        {
            sideSpeedFactor = sideSpeed > 0 ? 3 : 1;
            sideSpeed -= acceleration * sideSpeedFactor * Time.fixedDeltaTime;
            sideSpeed = LimitValue(sideSpeed, maxSideSpeed);
        }
        if (!Input.anyKey)
        {
            if (sideSpeed < -0.01)
            {
                sideSpeed += acceleration / sideSlowdownFactor * Time.fixedDeltaTime;
            }
            else if (sideSpeed > 0.01)
            {
                sideSpeed -= acceleration / sideSlowdownFactor * Time.fixedDeltaTime;
            }
            else 
            {
                sideSpeed = 0;
            }
        }
        HandleOutOfBounds();
        
        transform.Translate(Vector3.right * sideSpeed);
    }

    /**
    * Sets sideSpeed to 0 if player is at screen edges  
    */
    void HandleOutOfBounds()
    {
        Vector2 leftCollider2DEdge = new Vector2(collider2D.bounds.center.x - collider2D.bounds.extents.x, transform.position.y);
        Vector2 rightCollider2DEdge = new Vector2(collider2D.bounds.center.x + collider2D.bounds.extents.x, transform.position.y);
        Vector2 distanceToLeftScreenSide = cam.WorldToScreenPoint(leftCollider2DEdge);
        Vector2 distanceToRightScreenSide = cam.WorldToScreenPoint(rightCollider2DEdge);
        distanceToRightScreenSide.x = Screen.width - distanceToRightScreenSide.x;

        if ((distanceToRightScreenSide.x < 0 && sideSpeed > 0) | (distanceToLeftScreenSide.x < 0 && sideSpeed < 0))
        {
            sideSpeed = 0f;
        }
    }

    void HandleFalling()
    {
        
        /*if (!hasFirstJump)
        {
            return;
        }*/
        
        gravityFactor = (baseGravity - transform.position.y/500) > 0.05f ? (baseGravity - transform.position.y/500): 0.05f;
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += gravityFactor * Time.fixedDeltaTime;
        }
        transform.Translate(Vector3.down * currentSpeed);
    }

    void HandleStart()
    {
        Vector2 bottomCollider2DEdge = new Vector2(transform.position.x, collider2D.bounds.center.y - collider2D.bounds.extents.y);
        Collider2D startPlatformCollider2D = startPlatform.GetComponent<Collider2D>();
        Vector2 topStartPlatformCollider2DEdge = new Vector2(startPlatform.transform.position.x, 
            startPlatformCollider2D.bounds.center.y + startPlatformCollider2D.bounds.extents.y);

        if (bottomCollider2DEdge.y - topStartPlatformCollider2DEdge.y < 0.01)
        {
            //transform.position = new Vector3(transform.position.x, topStartPlatformCollider2DEdge.y + spriteHeight / 2, transform.position.z);
            transform.Translate(Vector3.up * currentSpeed);
            currentSpeed = 0;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            currentState = States.PLAYING;
            HandleJumping();
            SetPlayingVariables();
        }
    }
    
    void HandleFinish()
    {
        Vector2 bottomCollider2DEdge = new Vector2(transform.position.x, collider2D.bounds.center.y - collider2D.bounds.extents.y);
        Collider2D finishPlatformCollider2D = finishPlatform.GetComponent<Collider2D>();
        Vector2 topFinishPlatformCollider2DEdge = new Vector2(finishPlatform.transform.position.x, 
            finishPlatformCollider2D.bounds.center.y + finishPlatformCollider2D.bounds.extents.y);

        if (bottomCollider2DEdge.y - topFinishPlatformCollider2DEdge.y < 0.01)
        {
            //transform.position = new Vector3(transform.position.x, topStartPlatformCollider2DEdge.y + spriteHeight / 2, transform.position.z);
            transform.Translate(Vector3.up * currentSpeed);
            currentSpeed = 0;
        }
    }


    void SetPlayingVariables(){
        sideSlowdownFactor = 4f;
        acceleration = 2f;
        maxSideSpeed = 0.08f;
    }

    void HandleJumping()
    {
        if (currentSpeed < 0) // No jumping when going up
        {
            return;
        }
        currentSpeed = -jumpSpeed;
        animator.SetTrigger("isJump");
        PlaySound(Action.JUMP);
    }

    void PlaySound(Action playerAction)
    {
        switch (playerAction)
        {
            case Action.JUMP:
                audio.Play(0);
                break;
        }        
    }

    void HandleAnimations()
    {
        AllBoolAnimatorParamsFalse();
        switch (currentState)
        {
            case States.START:
            case States.FINISH:
                if (Input.GetKey("d") || sideSpeed > 0.01)
                {
                    animator.SetBool("isRunningLeft", false);
                    animator.SetBool("isRunningRight", true);
                    animator.SetBool("isIdle", false);
                }
                else if(Input.GetKey("a") || sideSpeed < -0.01)
                {
                    animator.SetBool("isRunningLeft", true);
                    animator.SetBool("isRunningRight", false);
                    animator.SetBool("isIdle", false);
                }
                else
                {
                    animator.SetBool("isRunningLeft", false);
                    animator.SetBool("isRunningRight", false);
                    animator.SetBool("isIdle", true);
                }
                break;

            case States.PLAYING:
                if (sideSpeed < -0.02)
                {
                    animator.SetBool("isFallingRight", false);
                    animator.SetBool("isFallingLeft", true);
                }
                else if (sideSpeed > 0.02)
                {
                    animator.SetBool("isFallingRight", true);
                    animator.SetBool("isFallingLeft", false);
                }
                else
                {
                    animator.SetBool("isFallingRight", false);
                    animator.SetBool("isFallingLeft", false);
                }
                if (!Input.anyKey)
                {
                    animator.SetBool("isFallingRight", false);
                    animator.SetBool("isFallingLeft", false);
                }
                break;
        } 
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "FinishTrigger" && currentState == States.FINISH)
        {
            print("FINISH!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInCollision)
        {
            return;
        }
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "StartPlatform")
        {
            HandleJumping();
        }
        else if (collision.gameObject.tag == "FinishPlatform" && currentSpeed > 0)
        {
            currentState = States.FINISH;
        }

        isInCollision = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isInCollision = false;
    }

    public float GetCurrentSpeedPlayer()
    {
        return currentSpeed;
    }

    private float LimitValue(float variable, float limit)
    {
        if (variable > 0 && variable <= limit)
        {
            return variable;
        }
        else if (variable < 0 && -variable <= limit)
        {
            return variable;
        }
        else if (variable < 0)
        {
            return -limit;
        }
        return limit;
    }

    void AllBoolAnimatorParamsFalse()
    {

        foreach(AnimatorControllerParameter parameter in animator.parameters) 
        {   
            animator.SetBool(parameter.name, false);            
        }
    }

}