using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

[SerializeField] float runSpeed = 5f;
[SerializeField] float jumpSpeed = 10f;
[SerializeField] float climbSpeed = 2f;
[SerializeField] Vector2 deathKick = new Vector2 (10f, 10f);
[SerializeField] GameObject shadowDash;
[SerializeField] Transform bow;


    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    bool isAlive = true;



    // Start is called before the first frame update
    void Start()
    {
        //calling player variables
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart =myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //states
        if (!isAlive) { return; } 
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
        
    }


    void OnDance (InputValue value)
    {
        if (!isAlive) { return; }
        myAnimator.SetTrigger("Dancing");
    }


    void OnAttack(InputValue value)
    {
        if (!isAlive) { return; }
        myAnimator.SetTrigger("Attack");
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        myAnimator.SetTrigger("Shooting");
        
        Instantiate(shadowDash, bow.position, transform.rotation);
    }


    //get mevement input
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; } 
        moveInput = value.Get<Vector2>();
    }

    // Jump input
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; } 

        // stop doublejump
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {return;}

        //jumpfunction
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2 (0f, jumpSpeed);
        }
    }


    // run
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
    
        //run animation trigger
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

    }


    // make character face forward and backward
    void FlipSprite()
    {

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }

        
    }


    //player Climb function
    void ClimbLadder()
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            
            return;
        }

        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;


        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
       
    }


    void Die()
        {
            if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
            {
                isAlive = false;
                myAnimator.SetTrigger("Dying");
                myRigidbody.velocity = deathKick;
                FindObjectOfType<GameSession>().ProcessPlayerDeath();
            }
        }


}
