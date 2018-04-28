using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This Class controls character movement and animation.
/// </summary>
public class PlayerController : MonoBehaviour {

    //Player Movement Settings
    /// <summary>
    /// This float controls the movement speed of the character in the scene.
    /// It is public so it can be accessed form the editor.
    /// </summary>
    public float speed = 3f;
    /// <summary>
    /// This float controls jumping distance of the character.
    /// It is public so it can be accessed form the editor.
    /// </summary>
    public float jumpDist = 3.0f;
    /// <summary>
    /// This float controls the gravity pull that affects the player bringing it 
    /// closer to the ground every update.
    /// It is public so it can be accessed form the editor.
    /// </summary>
    public float gravity = 20f;

    // Private Class variables.
    /// <summary>
    /// Vector2 used to hold x and y values, where x and y are input from the player.
    /// </summary>
    private Vector2 input;
    /// <summary>
    /// Resulting angle from character's local rotation.
    /// </summary>
    private float angle;
    /// <summary>
    /// Variable float used to store the speed at which the gravity pulls the character downwards.
    /// </summary>
    private float verticalVelocity;
    /// <summary>
    /// Quaterion to store character rotation.
    /// </summary>
    private Quaternion targetRotation;

    //References to player transform and CharacterController components.
    /// <summary>
    /// References to the Main Camera Transform and to its own Transform. 
    /// (Own is the Transfrom of the GameObject where this script is attached to.)
    /// </summary>
    private Transform cam, character;

    /// <summary>
    /// Reference to the CharacterController compontent that handles character 
    /// movement whithin the scene.
    /// </summary>
    private CharacterController player;

    //References to animators
    /// <summary>
    /// Animator Component reference for the character's clothes.
    /// </summary>
    public Animator clothesAnimator;
    /// <summary>
    /// Animator Component reference for the character's body. 
    /// </summary>
    public Animator bodyAnimator;
    /// <summary>
    /// Animator Component reference for the character's hair;
    /// </summary>
    public Animator hairAnimator;


    //Initialize components
    /// <summary>
    /// Inherited Method form MonoBehaviour;
    /// It is called after the constructor, once.
    /// Intialises the references of Character, CharacterController and Main Camera 
    /// to the components of the GameObject this script is attached to.
    /// </summary>
    private void Start()
    {
        character = GetComponent<Transform>();
        player = character.GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }


    /// <summary>
    /// Inherited Method form MonoBehaviour;
    /// It is called every frame.
    /// Every frame performs a series of operations:
    /// * Gets input from user.
    /// * Passes to the Animators values based on Input to animate the different graphical components of the character.
    /// * Applies Gravity to the Character.
    /// * Calculates Rotation direction.
    /// * Rotates transform based on previous calculation.
    /// * Finally, moves the character in the given direction.
    /// </summary>
    private void Update()
    {
            GetInput();
            Animate();
            ApplyGravity();
            CalculateDirection();
            Rotate();
            Move();    
    }

    /// <summary>
    /// Sets Animation values to those provided by input.
    /// </summary>
    private void Animate()
    {
        clothesAnimator.SetFloat("xInput",input.x);
        clothesAnimator.SetFloat("yInput", input.y);

        bodyAnimator.SetFloat("xInput", input.x);
        bodyAnimator.SetFloat("yInput", input.y);

        hairAnimator.SetFloat("xInput", input.x);
        hairAnimator.SetFloat("yInput", input.y);
    }

    /// <summary>
    /// This function detects whether the player transfor is airborne and applies velocity downwards
    /// to mimic gravity. Get input to jump only if the player is grounded.
    /// </summary>
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

    /// <summary>
    /// Get player Input.
    /// </summary>
    private void GetInput()
    {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
    }


    /// <summary>
    /// Calculate angle in which the character should face to.
    /// </summary>
    private void CalculateDirection()
    {
        angle = Mathf.Atan2(character.position.x, character.position.z);
        angle = Mathf.Rad2Deg * angle;
        angle = cam.eulerAngles.y;
    }

    /// <summary>
    /// Rotates character on Y axis to match the camera.
    /// </summary>
    private void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }


    /// <summary>
    /// Move forward in the direction the transform is facing. 
    /// Use character controller component to move the transform.
    /// </summary>
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
