using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This Class controls character movement and animation.
/// </summary>
public class NPCController : MonoBehaviour {

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

	private NavMeshAgent agent;

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
        agent = character.GetComponent<NavMeshAgent>();
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
            CalculateDirection();
            Rotate();
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

    }

    /// <summary>
    /// Get player Input.
    /// </summary>
    private void GetInput()
    {
		if (agent.destination.x > character.position.x)
		{
			//moving right
			input.x = 1;
		}
		else if (agent.destination.x < character.position.x)
		{
			//moving left
			input.x = -1;
		}
		else
		{
			input.x = 0;
		}

		if (agent.destination.z > character.position.z)
		{
			//moving right
			input.y = 1;
		}
		else if (agent.destination.z < character.position.z)
		{
			//moving left
			input.y = -1;
		}
		else
		{
			input.y = 0;
		}
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

    }

}
