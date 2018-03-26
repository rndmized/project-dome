using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Player Movement Settings
    public float speed = 3f;
    public float jumpDist = 3.0f;
    public float gravity = 20f;


    private Vector2 input;
    private float angle;
    private float verticalVelocity;
    private Quaternion targetRotation;

    //References to player transform and CharacterController components.
    Transform cam, character;
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
        cam = Camera.main.transform;
    }



    void Update()
    {
            GetInput();
            Animate();
            ApplyGravity();
            CalculateDirection();
            Rotate();
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


    // Calculate angle in which the player should face to.
    void CalculateDirection()
    {
        angle = Mathf.Atan2(character.position.x, character.position.z);
        angle = Mathf.Rad2Deg * angle;
        angle = cam.eulerAngles.y;

    }

    // rotate Y axis player to face correct direction.
    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
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
