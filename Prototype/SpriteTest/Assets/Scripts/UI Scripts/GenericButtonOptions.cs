using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script With Generic Options for Button Behaviour.
/// </summary>
public class GenericButtonOptions : MonoBehaviour {

    /// <summary>
    /// Return to Login Menu. (Removes stored Token)
    /// </summary>
    public void LogOut()
    {
        PlayerProfile.uID = null;
        SceneManager.LoadScene("LoginScene");
    }

    /// <summary>
    /// Exits Application.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }


}
