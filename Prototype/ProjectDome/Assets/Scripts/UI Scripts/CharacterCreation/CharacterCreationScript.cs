using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script that controls Character Creation Process.
/// </summary>
public class CharacterCreationScript : MonoBehaviour {

    /// <summary>
    /// Reference to Character's Transform.
    /// </summary>
    public Transform player;

    /// <summary>
    /// Private Instance of a Chraracter.
    /// </summary>
    private Character character;
    /// <summary>
    /// Reference to the Character Renderer Component of the Character.
    /// </summary>
    private CharacterRenderer char_renderer;
    /// <summary>
    /// Reference to list of Hair Animations.
    /// </summary>
    private List<RuntimeAnimatorController> HairStyles;
    /// <summary>
    /// reference to list of Clothes Animations.
    /// </summary>
    private List<RuntimeAnimatorController> ClothesStyles;
    /// <summary>
    /// Reference to list of Body Animations.
    /// </summary>
    private List<RuntimeAnimatorController> BodyStyle;

    /// <summary>
    /// Reference to Fielf Input for character's name.
    /// </summary>
    private InputField charNameInputField;

    /// <summary>
    /// Index of clothes.
    /// </summary>
    private int clothes;
    /// <summary>
    /// Index of hair styles.
    /// </summary>
    private int hairStyle;
    /// <summary>
    /// Index of body type.
    /// </summary>
    private int body;
    
    /// <summary>
    /// Initializer.
    /// </summary>
    void Start () {

        /* Initialize references to its default values. */

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

    /// <summary>
    /// Hair Style Rotation function.
    /// </summary>
    /// <param name="next">Can be either next or previous</param>
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

    /// <summary>
    /// Clothing Rotation Function.
    /// </summary>
    /// <param name="next">Can be either next or previous</param>
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

    /// <summary>
    /// Calls Async IEnumerator to upload Character to server to store it in the player's account.
    /// </summary>
    public void CreateCharacter()
    {
        StartCoroutine(Upload());
    }

    /// <summary>
    /// Return to Player's Account Menu.
    /// </summary>
    public void ReturnToAccount()
    {
        SceneManager.LoadScene("PlayerAccountScene");
    }

    /// <summary>
    /// Upload new Character data to Server.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Upload()
    {

        /*  Create Dictionary of strings to pass as arguments in the request body. */
        Dictionary<string, string> formFields = new Dictionary<string, string>();
        formFields.Add("username", PlayerProfile.uID);
        formFields.Add("char_name", charNameInputField.text);
        formFields.Add("char_hairId", hairStyle.ToString());
        formFields.Add("char_clothesId", clothes.ToString());
        formFields.Add("char_bodyId", body.ToString());


        // Create http post request
        UnityWebRequest http = UnityWebRequest.Post(PlayerProfile.GetLoginServerAddress().ToString()+"/createCharacter", formFields);
        http.SetRequestHeader("Authorization", "Bearer " + PlayerProfile.token);

        // Await for response
        yield return http.SendWebRequest();
        
        if (http.isNetworkError || http.isHttpError)
        {
            /* Log error. */
            Debug.Log(http.error);
        }
        else
        {
        
            /* Parse Server response. */
            ServerAssertion log = JsonUtility.FromJson<ServerAssertion>(http.downloadHandler.text);

            /* If Server response is successfull return to previous menu. */
            if (log.success == true)
            {
                SceneManager.LoadScene("PlayerAccountScene");
            }
            else
            {
                /* TODO: Notify player. */
                Debug.Log(log.msg);
            }

        }
    }
}
