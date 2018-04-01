using UnityEngine;

/// <summary>
/// This class is used to control camera movement.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Transforms for the camera itself, the caracter it will be following and the centerpoint at which the camara will be orientated.
    /// </summary>
    /// <value>The values must be valid Transforms </value>
    public Transform playerCam, character, centerPoint;

    /// <summary>
    /// This is used to get or set x and y values for the camera relative to the character.
    /// </summary>
    /// <value>The value can be any valid float</value>
    public float mouseX, mouseY;

    /// <summary>
    /// This is used to get or set the relative Y position to the Center Point.
    /// </summary>
    /// <value>The value can be any valid integer</value>
    public float mouseYPosition = 1f;

    /// <summary>
    /// This is used to get or set camera zoom value.
    /// </summary>
    /// <value>The value can be any valid integer</value>
    public float zoom;

    /// <summary>
    /// This is used to get or set the speed ath which zoom is applied.
    /// </summary>
    /// <value>The value can be any valid float</value>
    public float zoomSpeed = 2;

    /// <summary>
    /// This is used to get or set the minimum zoom value.
    /// </summary>
    /// <value>The value can be any valid float</value>
    public float zoomMin = -2f;

    /// <summary>
    /// This is used to get or set the maximum zoom value.
    /// </summary>
    /// <value>The value can be any valid float</value>
    public float zoomMax = -10f;


    /// <summary>
    /// This private function is inherited from MonoBehaviour and gets called every frame.
    /// </summary>
    /// <summary>
    /// Repositions the camera based on the float values assigned and the position of the Character transform in the scene.
    /// The Script has been adapted to disable camera rotation since it would break inmersion in the 2.5D environment the game
    /// has been set in. 
    /// Ref: https://github.com/rndmized/OkamiBushi/blob/master/OkamiBushi/Assets/Scripts/CameraScripts/CameraController.cs
    /// 2.5D Note: The caracters are 2D sprites while the enviroment is full 3D. Since we lack an artist that can produce 
    /// quality 3D content for the game we have disabled rotation to hide model imperfections and play arount whith lower 
    /// quality models that would not be pleasant in a more detailed environment.
    /// </summary>
    private void Update()
    {

        //Getting Zoom Variation from Input
        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        //Limit Zoom Scope
        if (zoom > zoomMin)
            zoom = zoomMin;

        if (zoom < zoomMax)
            zoom = zoomMax;

        //Set Camera distance on z axis to new zoom. (Closer to the Character.)
        playerCam.transform.localPosition = new Vector3(0, 0, zoom);


        //Limit and set camera rotation based on center point.
        mouseY = Mathf.Clamp(mouseY, 1f, 60f);
        playerCam.LookAt(centerPoint);
        centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
        centerPoint.position = new Vector3(character.position.x, character.position.y + mouseYPosition, character.position.z);

    }
}
