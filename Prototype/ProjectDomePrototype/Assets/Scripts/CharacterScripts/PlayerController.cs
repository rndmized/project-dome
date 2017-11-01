using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Player Movement Settings
    public float speed = 3f;
    public float turnSpeed = 10f;
    public float jumpDist = 3.0f;
    public float gravity = 20f;


    private Vector2 input;
    private float angle;
    private float verticalVelocity;

    Quaternion targetRotation;
    
    //References to player transform and CharacterController components.
    Transform character;
    CharacterController player;


    //Initialize components
    void Start()
    {

        character = GetComponent<Transform>();
        player = character.GetComponent<CharacterController>();
    }



    void Update()
    {

            GetInput();
            ApplyGravity();
            Move();    
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

    /* Calculate rotation angle using camera direction */
    void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
    }

    /* Use angle and quaternion to rotate transform. Slerp to smooth transition. */
    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    /* Move forward in the direction the transform is facing. Use character controller component to move the transform.  */
    private void Move()
    {
        Vector3 movement = new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        if (!(Mathf.Abs(input.x) < 0.1 && Mathf.Abs(input.y) < 0.1))
        {
            //Claculate direction to face to.
            CalculateDirection();
            //Rotate transform
            Rotate();
            movement += transform.forward * speed * Time.deltaTime;
        }
        player.Move(movement);
    }

}
