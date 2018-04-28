using UnityEngine;

/// <summary>
/// CharacterData is a representation of a Character like it is stored in the database.
/// </summary>
[System.Serializable]
public class CharacterData
{
    public string _id;
    public string userID;
    public string char_name;
    public string char_hairId;
    public string char_bodyId;
    public string char_clothesId;
    public int score;
    public int __v;

}
