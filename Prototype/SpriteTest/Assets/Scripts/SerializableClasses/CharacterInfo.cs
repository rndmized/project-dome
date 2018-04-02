using UnityEngine;

/// <summary>
/// CharacterInfo is an ScriptableObject that can be created from the contextual menu in the editor.
/// Unlike regular classes scriptable objects are instantiated using instantiate rather than the keyword
/// new.
/// It is a representation of a Character like it is stored in in-game memory.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string _id;
    public string userID;
    public string char_name;
    public int char_hairId;
    public int char_bodyId;
    public int char_clothesId;
    public int score;
    public int __v;

}
