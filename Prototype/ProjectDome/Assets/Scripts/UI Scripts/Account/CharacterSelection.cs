using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// CharacterSelection Script determine behaviour in the Character Selection Screen.
/// It controls Server requests allowing or denying the following operations:
/// Loads Characters from database for a given user.
/// Handles Character Deletion.
/// </summary>
public class CharacterSelection : MonoBehaviour {

    /// <summary>
    /// List of Characters.
    /// </summary>
    private List<CharacterInfo> characterInfos;
    /// <summary>
    /// Reference to the CharacterDisplayPanels.
    /// </summary>
    public GameObject panelPrefab;
    /// <summary>
    /// Bool update notify changes on the list.
    /// </summary>
    bool update = false;

    /// <summary>
    /// Method Inherited from MonoBehaviour.
    /// It is called once after the constructor.
    /// On Start LoadCharacterList().
    /// </summary>
    private void Start()
    {
        LoadCharacterList();
    }

    /// <summary>
    /// Starts Async IEnumerator that contacts the server to retrieve the list of characters for the logged in user.
    /// </summary>
    private void LoadCharacterList()
    {
        StartCoroutine(LoadCharacters());
    }

    /// <summary>
    /// Calls Async Function to delete Character at index index.
    /// </summary>
    /// <param name="index">Index of character to delete.</param>
    public void RequestCharacterDeletion(int index)
    {
        //Call function to remove character from db
        StartCoroutine(DeleteCharacter(characterInfos[index].char_name));
        //remove char from list
        characterInfos.RemoveAt(index);
    }


    /// <summary>
    /// FixedUpdate is an Inherited method from MonoBehaviour.
    /// It gets called every other frame.
    /// Checks if there is an update needed and updates display accordingly.
    /// Disables Character creation button upon slot completion.
    /// </summary>
    private void FixedUpdate()
    {
        if (update)
        {
            int index = 0;
            foreach (var item in characterInfos)
            {
                panelPrefab.GetComponent<CharacterElementDisplay>().charInfo = item;
                panelPrefab.GetComponent<CharacterElementDisplay>().index = index;
                GameObject Element = Instantiate(panelPrefab);
                Element.transform.SetParent(GameObject.Find("CharactersList").transform, false);
                index++;
            }
            update = false;
        }

        if (characterInfos.Count >= 4)
        {
            GameObject.Find("NewCharacterButton").GetComponent<UnityEngine.UI.Button>().enabled = false;
            GameObject.Find("NewCharacterButton").GetComponentInChildren<UnityEngine.UI.Text>().text = "Character Slots Full";
        }
        else
        {
            GameObject.Find("NewCharacterButton").GetComponent<UnityEngine.UI.Button>().enabled = true;
            GameObject.Find("NewCharacterButton").GetComponentInChildren<UnityEngine.UI.Text>().text = "NEW CHARACTER";
        }
    }

    /// <summary>
    /// Queries server to retrieve list of character for logged in player.
    /// </summary>
    /// <returns>Character List from Server.</returns>
    private IEnumerator LoadCharacters()
    {
        /* Initializes List of CharacterInfo. */
        characterInfos = new List<CharacterInfo>();

        /*  Create Dictionary of strings to pass as arguments in the request body. */
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("uID", PlayerProfile.uID);

        /* Create WebRequest and append token in order to access protected routes. */
        UnityWebRequest http = new UnityWebRequest();
        http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString() + "/getCharacterList", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);


        // Await for response
        yield return http.SendWebRequest();

        /* If no network error occurs */
        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
            Debug.Log(http.responseCode);
        }
        else
        {
            //Fix Json to a JsonUtility Readable Json using JsonHelper
            string json = JsonHelper.fixJson(http.downloadHandler.text);
            CharacterData[] charData = JsonHelper.FromJson<CharacterData>(json);
            /* For each element in the array result, create an instance of CharacteInfo and set its 
             * values to thos of the CharacterData obtaine from server.
             * Append instance to List.
             */
            foreach (var item in charData)
            {
                CharacterInfo c = ScriptableObject.CreateInstance<CharacterInfo>();
                int.TryParse(item.char_bodyId, out c.char_bodyId);
                int.TryParse(item.char_clothesId, out c.char_clothesId);
                int.TryParse(item.char_hairId, out c.char_hairId);
                c.char_name = item.char_name;
                c.userID = item.userID;
                c._id = item._id;
                characterInfos.Add(c);
            }
            /* Notify ready for update. */
            update = true;
        }

    }

    /// <summary>
    /// Remove Character from Server.
    /// </summary>
    /// <param name="char_name">Character name.</param>
    /// <returns></returns>
    private IEnumerator DeleteCharacter(string char_name)
    {
        /*Create string disctionary to pass as arguments in the request body. */
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("uID", PlayerProfile.uID);
        formFields.Add("char_name", char_name);

        /* Create WebRequest and append token in order to access protected routes. */
        UnityWebRequest http = new UnityWebRequest();
        http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString() + "/deleteCharacter", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);

        /* Await for response. */
        yield return http.SendWebRequest();

        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
        }
        else
        {
            ServerAssertion log = JsonUtility.FromJson<ServerAssertion>(http.downloadHandler.text);
        }
    }
}

