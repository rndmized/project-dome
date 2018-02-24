using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Player Movement Settings
    public float speed = 3f;
    public float jumpDist = 3.0f;
    public float gravity = 20f;


    private Vector2 input;
    private float verticalVelocity;
    
    //References to player transform and CharacterController components.
    Transform character;
    CharacterController player;

    //References to animators
    public Animator clothesAnimator;
    public Animator bodyAnimator;
    public Animator hairAnimator;


    //Initialize components
    void Start()
    {
        character = GetComponent<Transform>();
        player = character.GetComponent<CharacterController>();
    }



    void Update()
    {
            GetInput();
            Animate();
            ApplyGravity();
            Move();    
    }


    private void Animate()
    {
        clothesAnimator.SetFloat("xInput",input.x);
        clothesAnimator.SetFloat("yInput", input.y);

        bodyAnimator.SetFloat("xInput", input.x);
        bodyAnimator.SetFloat("yInput", input.y);

        hairAnimator.SetFloat("xInput", input.x);
        hairAnimator.SetFloat("yInput", input.y);
    }


    /* This function detects whether the player transfor is airborne and applies velocity downwards
        to mimic gravity (sort of). Get input to jump only if the player is grounded.
     */
    private void ApplyGravity()
    {

        if (player.isGrounded == true)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpDist;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    /* Get input */
    void GetInput()
    {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
    }


    /* Move forward in the direction the transform is facing. Use character controller component to move the transform.  */
    private void Move()
    {
        Vector3 movement = new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        if (!(Mathf.Abs(input.x) < 0.1 && Mathf.Abs(input.y) < 0.1))
        {
            movement += new Vector3(input.x, verticalVelocity, input.y );
        }
        player.Move((movement*speed) * Time.deltaTime);
    }

}
