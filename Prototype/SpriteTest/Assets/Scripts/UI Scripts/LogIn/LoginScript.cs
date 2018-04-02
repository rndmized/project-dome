using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script that controls Log In Process.
/// </summary>
public class LoginScript : MonoBehaviour {

    /// <summary>
    /// Reference to User's name Input Field.
    /// </summary>
    private InputField usernameInputField;
    /// <summary>
    /// Reference to User's password Input Field.
    /// </summary>
    private InputField passwordInputField;

    /// <summary>
    /// Initializer.
    /// </summary>
    private void Start()
    {
        /* Assign references to its Components. */
        usernameInputField = GameObject.Find("UserInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
    }

    /// <summary>
    /// LogIn Function.
    /// </summary>
    public void login()
    {
        if(Validation()) StartCoroutine(Upload());
    }

    /// <summary>
    /// Low level Validation Function.
    /// </summary>
    /// <returns>True if conditions in Input Fields are met, False otherwise.</returns>
    private bool Validation()
    {
        if (usernameInputField.text.Length < 5 || usernameInputField.text.Length > 16) { return false; }
        if (passwordInputField.text.Length < 8 || passwordInputField.text.Length > 17) { return false; }
        return true;
    }

    /// <summary>
    /// Navigate to Account Creation Scene.
    /// </summary>
    public void NavigateToAccountCreation()
    {
        SceneManager.LoadScene("AccountCreationScene");
    }

    /// <summary>
    /// Asyn IEnumerator function that sends user information to server to check 
    /// whether can access or not to the application content.
    /// </summary>
    private IEnumerator Upload()
    {

        /*  Create Dictionary of strings to pass as arguments in the request body. */
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", usernameInputField.text);
        formFields.Add("password", passwordInputField.text);

        /* Create WebRequest. */
        UnityWebRequest http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString()+"/login", formFields);

        // Await for response
        yield return http.SendWebRequest();

        
        if (http.isNetworkError || http.isHttpError)
        {
            // Log error
            Debug.Log(http.error);
        }
        else
        {
            /* Parse Server Response */
            LoginInfo log = JsonUtility.FromJson<LoginInfo>(http.downloadHandler.text);

            /* On Success Save token, user Id and move to next Scene. */
            if (log.success == true)
            {
                PlayerProfile.uID = usernameInputField.text;
                PlayerProfile.token = log.token;
                SceneManager.LoadScene("PlayerAccountScene");
            }
            /* TODO: Notify user of Failure Reasons. */
            
        }
    }

}

