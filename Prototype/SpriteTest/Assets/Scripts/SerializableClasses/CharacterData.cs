using UnityEngine;

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
