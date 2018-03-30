using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreationScript : MonoBehaviour {

    public Transform player;

    Character character;
    CharacterRenderer char_renderer;
    List<RuntimeAnimatorController> HairStyles;
    List<RuntimeAnimatorController> ClothesStyles;
    List<RuntimeAnimatorController> BodyStyle;

    private InputField charNameInputField;

    int clothes;
    int hairStyle;
    int body;
    
    // Use this for initialization
    void Start () {


        character = ScriptableObject.CreateInstance<Character>();
        HairStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().HairStyles;
        ClothesStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().ClothesStyles;
        BodyStyle = FindObjectOfType<AssetList>().GetComponent<AssetList>().BodyStyle;
        character.char_clothesAnimator = ClothesStyles[clothes];
        character.char_headAnimator = HairStyles[hairStyle];
        character.char_bodyAnimator = BodyStyle[0];
        player.GetComponent<CharacterRenderer>().character = character;


        charNameInputField = GameObject.Find("CharacterNameInputField").GetComponent<InputField>();

        

    }

    public void NextHairStyle(string next)
    {
        switch (next)
        {
            case "next":
                hairStyle++;
                if (hairStyle > HairStyles.Count) hairStyle = 0;
                break;
            case "previous":
                hairStyle--;
                if (hairStyle < 0) hairStyle = HairStyles.Count-1;
                break;
        }
        character.char_headAnimator = HairStyles[hairStyle];
        player.GetComponent<CharacterRenderer>().character = character;

    }


    public void NextClothing(string next)
    {
        switch (next)
        {
            case "next":
                clothes++;
                if (clothes > ClothesStyles.Count) clothes = 0;
                break;
            case "previous":
                clothes--;
                if (clothes < 0) clothes = ClothesStyles.Count - 1;
                break;
        }
        character.char_clothesAnimator = ClothesStyles[clothes];
        player.GetComponent<CharacterRenderer>().character = character;

    }


    public void CreateCharacter()
    {
        StartCoroutine(Upload());
    }


    public void ReturnToAccount()
    {
        SceneManager.LoadScene("PlayerAccountScene");
    }


    IEnumerator Upload()
    {

        // Create appropriate structure to send scores over the network
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", "Test");
        formFields.Add("char_name", charNameInputField.text);
        formFields.Add("char_hairId", hairStyle.ToString());
        formFields.Add("char_clothesId", clothes.ToString());
        formFields.Add("char_bodyId", body.ToString());


        // Create http post request
        UnityWebRequest http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString()+"/createCharacter", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);

        // Await for response
        yield return http.SendWebRequest();
        // Log error, if no error return to main menu.
        if (http.isNetworkError || http.isHttpError)
        {
            Debug.Log(http.error);
        }
        else
        {
            ServerAssertion log = JsonUtility.FromJson<ServerAssertion>(http.downloadHandler.text);
            Debug.Log(log.success);

            if (log.success == true)
            {
                SceneManager.LoadScene("PlayerAccountScene");
            }
            else
            {
                Debug.Log(log.msg);
            }

        }
    }
}
