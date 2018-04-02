using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInScript : MonoBehaviour
{
    /// <summary>
    /// Reference of Input Field.
    /// </summary>
    private InputField usernameInputField;
    /// <summary>
    /// Reference of Input Field.
    /// </summary>
    private InputField passwordInputField;
    /// <summary>
    /// Reference of Input Field.
    /// </summary>
    private InputField passwordInputFieldValidation;
    /// <summary>
    /// Reference of Input Field.
    /// </summary>
    private InputField fullnameInputField;
    /// <summary>
    /// Reference of Input Field.
    /// </summary>
    private InputField emailInputField;

    /// <summary>
    /// Reference of Error Display Label.
    /// </summary>
    private Text errorDisplay;


    /// <summary>
    /// Initializer.
    /// </summary>
    private void Start()
    {
        /* Sets references to GameObjects. */
        usernameInputField = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        passwordInputFieldValidation = GameObject.Find("PasswordInputFieldValidation").GetComponent<InputField>();
        fullnameInputField = GameObject.Find("FullNameInputField").GetComponent<InputField>();
        emailInputField = GameObject.Find("EmailInputField").GetComponent<InputField>();

        errorDisplay = GameObject.Find("ErrorDisplay").GetComponent<Text>();
        errorDisplay.text = "";

    }

    /// <summary>
    /// Register New User.
    /// </summary>
    public void register()
    {
        if (Validate())
        {
            StartCoroutine(Upload());
        }
    }

    /// <summary>
    /// Low Level Validation Check.
    /// </summary>
    /// <returns>True if Input Fields have appropriate length, false otherwise.</returns>
    private bool Validate()
    {
        if (fullnameInputField.text.Length < 5 || fullnameInputField.text.Length > 16) { errorDisplay.text = "Full Name should be longer \n than 4 characters and shorter than 16!"; return false; }
        if (!emailInputField.text.Contains("@") && (!emailInputField.text.EndsWith(".com") || !emailInputField.text.EndsWith(".ie"))) { errorDisplay.text = "Invalid email address!"; return false; }
        if (usernameInputField.text.Length < 5 || usernameInputField.text.Length > 12) { errorDisplay.text = "Username should be longer \n than 4 characters and shorter than 16!"; return false; }
        if (passwordInputField.text.Length < 8 || passwordInputField.text.Length > 16) { errorDisplay.text = "Password should be longer \n than 7 characters and shorter than 17!";  return false; }
        if (passwordInputField.text != passwordInputFieldValidation.text) { errorDisplay.text = "Password does not match!"; return false; }
        return true;
    }

    /// <summary>
    /// Return to previous menu.
    /// </summary>
    public void ReturnToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    /// <summary>
    /// Asyn IEnumerator Sends User Data to server for Account Creation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Upload()
    {
        /*  Create Dictionary of strings to pass as arguments in the request body. */
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", usernameInputField.text);
        formFields.Add("password", passwordInputField.text);
        formFields.Add("email", emailInputField.text);
        formFields.Add("full_name", fullnameInputField.text);
        
        /* Create http post request */
        UnityWebRequest www = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress() + "/register", formFields);

        /* Await for response */
        yield return www.SendWebRequest();
        
        if (www.isNetworkError || www.isHttpError)
        {
            /* Log error. */
            Debug.Log(www.error);
        }
        else
        {
            /* Parse Server Response */
            AccountInfo log = JsonUtility.FromJson<AccountInfo>(www.downloadHandler.text);
            /* On Success Return to Login Scene*/
            if (log.success == true)
            {
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                /* On Failure, display error message. */
                errorDisplay.text = log.msg;
            }

        }
    }
}

