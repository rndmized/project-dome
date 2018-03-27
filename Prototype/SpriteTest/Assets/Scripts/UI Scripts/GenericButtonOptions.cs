using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericButtonOptions : MonoBehaviour {

    public void LogOut()
    {
        PlayerProfile.uID = null;
        SceneManager.LoadScene("LoginScene");
    }


    public void Exit()
    {
        Application.Quit();
    }


}
