using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInScript : MonoBehaviour {

    private InputField usernameInputField;
    private InputField passwordInputField;
    private InputField fullnameInputField;
    private InputField emailInputField;


    // Use this for initialization
    void Start () {
        usernameInputField = GameObject.Find("UsernameInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        fullnameInputField = GameObject.Find("FullNameInputField").GetComponent<InputField>();
        emailInputField = GameObject.Find("EmailInputField").GetComponent<InputField>();

    }

    public void register()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        // Create appropriate structure to send scores over the network
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", usernameInputField.text);
        formFields.Add("password", passwordInputField.text);
        formFields.Add("email",emailInputField.text);
        formFields.Add("full_name",fullnameInputField.text);
        // Create http post request
        UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/register", formFields);

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
            Debug.Log(log.success);

            if (log.success == true)
            {
                SceneManager.LoadScene("LoginScene");
            }

        }
    }
}

[System.Serializable]
public class AccountInfo
{
    public bool success;
    public string message;

}
