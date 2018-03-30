using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour {

    public List<CharacterInfo> characterInfos;
    public GameObject panelPrefab;
    bool update = false;

    private void Start()
    {

        LoadCharacterList();

    }

    private void LoadCharacterList()
    {
        StartCoroutine(LoadCharacters());
    }

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

    IEnumerator LoadCharacters()
    {
        characterInfos = new List<CharacterInfo>();

        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("uID", "Test");
        UnityWebRequest http = new UnityWebRequest();
        http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString() + "/getCharacterList", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);
        Debug.Log("Sending request to server");
        // Await for response
        yield return http.SendWebRequest();

        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
            Debug.Log(http.responseCode);
        }
        else
        {
            Debug.Log("Data received");
            string json = JsonHelper.fixJson(http.downloadHandler.text);
            CharacterData[] charData = JsonHelper.FromJson<CharacterData>(json);
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
            update = true;
        }

    }


    IEnumerator DeleteCharacter(string char_name)
    {
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("uID", "Test");
        formFields.Add("char_name", char_name);

        UnityWebRequest http = new UnityWebRequest();
        
        http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString() + "/deleteCharacter", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);
        Debug.Log("Sending request to server");
        // Await for response
        yield return http.SendWebRequest();

        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
        }
        else
        {

            ServerAssertion log = JsonUtility.FromJson<ServerAssertion>(http.downloadHandler.text);
            Debug.Log(log.success);
            Debug.Log(log.msg);
        }

    }




    public void RequestCharacterDeletion(int index)
    {
        Debug.Log("removing character: " + characterInfos[index].char_name);
        //Call function to remove character from db
        StartCoroutine(DeleteCharacter(characterInfos[index].char_name));
        //remove char from list
        characterInfos.RemoveAt(index);
    }

}

