using UnityEngine;

public class CameraController : MonoBehaviour
{

    //References to camera, player, and a center point at wich camera will look.
    public Transform playerCam, character, centerPoint;

    //Camera settings
    private float mouseX, mouseY;
    public float mouseSensitivity = 10f;
    public float mouseYPosition = 1f;

    private float zoom;
    public float zoomSpeed = 2;

    public float zoomMin = -2f;
    public float zoomMax = -10f;

    public float rotationSpeed = 5f;


    
    void Start()
    {
        //Set Initial Zoom Distance
        zoom = -4;

    }

  
    void Update()
    {

        //Getting Zoom Variation from Input
        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        //Limit Zoom Scope
        if (zoom > zoomMin)
            zoom = zoomMin;

        if (zoom < zoomMax)
            zoom = zoomMax;

        //Set Camera distance on z axis to new zoom. (closer to the payer)
        playerCam.transform.localPosition = new Vector3(0, 0, zoom);

        //Get x, y values from input (Mouse right-click)
        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        //Get x, y values from input (Controller)
        mouseX += Input.GetAxis("Camera X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Camera Y") * mouseSensitivity;

        //Limit and set camera rotation based on center point.
        mouseY = Mathf.Clamp(mouseY, 1f, 60f);
        playerCam.LookAt(centerPoint);
        centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
        centerPoint.position = new Vector3(character.position.x, character.position.y + mouseYPosition, character.position.z);

    }
}
