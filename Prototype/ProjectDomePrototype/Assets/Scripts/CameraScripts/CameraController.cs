using UnityEngine;

public class CameraController : MonoBehaviour
{


    public Transform playerCam, character, centerPoint;

    public float mouseX, mouseY;
    public float mouseSensitivity = 10f;
    public float mouseYPosition = 1f;

    public float zoom;
    public float zoomSpeed = 2;

    public float zoomMin = -2f;
    public float zoomMax = -10f;

    public float rotationSpeed = 5f;


    void Start()
    {

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


        //Limit and set camera rotation based on center point.
        mouseY = Mathf.Clamp(mouseY, 1f, 60f);
        playerCam.LookAt(centerPoint);

        centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

        centerPoint.position = new Vector3(character.position.x, character.position.y + mouseYPosition, character.position.z);

    }
}
