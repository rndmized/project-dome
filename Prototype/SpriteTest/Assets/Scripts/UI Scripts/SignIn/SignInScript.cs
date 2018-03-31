using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInScript : MonoBehaviour
{

    private InputField usernameInputField;
    private InputField passwordInputField;
    private InputField passwordInputFieldValidation;
    private InputField fullnameInputField;
    private InputField emailInputField;

    private Text errorDisplay;
    private bool errorMessage;

    // Use this for initialization
    private void Start()
    {
        usernameInputField = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        passwordInputFieldValidation = GameObject.Find("PasswordInputFieldValidation").GetComponent<InputField>();
        fullnameInputField = GameObject.Find("FullNameInputField").GetComponent<InputField>();
        emailInputField = GameObject.Find("EmailInputField").GetComponent<InputField>();

        errorDisplay = GameObject.Find("ErrorDisplay").GetComponent<Text>();
        errorDisplay.text = "";
        errorMessage = false;
    }

    public void register()
    {
        if (Validate())
        {
            StartCoroutine(Upload());
        }
    }

    private bool Validate()
    {
        if (fullnameInputField.text.Length < 5 || fullnameInputField.text.Length > 16) { errorDisplay.text = "Full Name should be longer \n than 4 characters and shorter than 16!"; return false; }
        if (!emailInputField.text.Contains("@") && (!emailInputField.text.EndsWith(".com") || !emailInputField.text.EndsWith(".ie"))) { errorDisplay.text = "Invalid email address!"; return false; }
        if (usernameInputField.text.Length < 5 || usernameInputField.text.Length > 12) { errorDisplay.text = "Username should be longer \n than 4 characters and shorter than 16!"; return false; }
        if (passwordInputField.text.Length < 8 || passwordInputField.text.Length > 16) { errorDisplay.text = "Password should be longer \n than 7 characters and shorter than 17!";  return false; }
        if (passwordInputField.text != passwordInputFieldValidation.text) { errorDisplay.text = "Password does not match!"; return false; }
        return true;
    }

    public void ReturnToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    private void FixedUpdate()
    {

    }

    private IEnumerator Upload()
    {
        // Create appropriate structure to send scores over the network
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", usernameInputField.text);
        formFields.Add("password", passwordInputField.text);
        formFields.Add("email", emailInputField.text);
        formFields.Add("full_name", fullnameInputField.text);
        // Create http post request
        UnityWebRequest www = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress() + "/register", formFields);

        // Await for response
        yield return www.SendWebRequest();
        // Log error, if no error return to main menu.
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AccountInfo log = JsonUtility.FromJson<AccountInfo>(www.downloadHandler.text);

            if (log.success == true)
            {
                SceneManager.LoadScene("LoginScene");
            }
            else
            {

                switch (log.code)
                {
                    case "MF":
                        errorDisplay.text = log.msg;
                        break;
                    case "DU":
                        //Display message: Duplicate username
                        errorDisplay.text = log.msg;
                        break;
                    case "D@":
                        //Display message: Duplicate email account
                        errorDisplay.text = log.msg;
                        break;
                    case "UE":
                        // Display message: Unexpected Error.
                        errorDisplay.text = log.msg;
                        break;
                    default:
                        break;
                }
            }

        }
    }
}

