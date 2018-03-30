using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour {

    private InputField usernameInputField;
    private InputField passwordInputField;

    private void Start()
    {
        usernameInputField = GameObject.Find("UserInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
    }

    public void login()
    {
        Debug.Log(usernameInputField.text);
        Debug.Log(passwordInputField.text);
        StartCoroutine(Upload());
    }

    public void NavigateToAccountCreation()
    {
        SceneManager.LoadScene("AccountCreationScene");
    }

    IEnumerator Upload()
    {
        // Create appropriate structure to send scores over the network
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", usernameInputField.text);
        formFields.Add("password", passwordInputField.text);
        // Create http post request
        UnityWebRequest http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString()+"/login", formFields);

        // Await for response
        yield return http.SendWebRequest();
        // Log error, if no error return to main menu.
        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
        }
        else
        {
            LoginInfo log = JsonUtility.FromJson<LoginInfo>(http.downloadHandler.text);
            Debug.Log(log.success);

            if (log.success == true)
            {
                PlayerProfile.uID = usernameInputField.text;
                PlayerProfile.token = log.token;
                Debug.Log(log.token);
                SceneManager.LoadScene("PlayerAccountScene");
            }
            
        }
    }

}

