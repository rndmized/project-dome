using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Script to control Button Behaviours.
/// </summary>
public class ButtonInteractionController : MonoBehaviour {

    /// <summary>
    /// Gets Character Name of Selected Character and Loads Game Scene.
    /// </summary>
    public void Play()
    {
        PlayerProfile.cID = this.transform.parent.GetComponent<CharacterElementDisplay>().charName.text;
        SceneManager.LoadScene("Test1");
    }

    /// <summary>
    /// Deletes Selected Character.
    /// </summary>
    public void Delete()
    {
        this.transform.parent.transform.gameObject.SetActive(false);
        GameObject.Find("MenuController").GetComponent<CharacterSelection>().RequestCharacterDeletion(this.transform.parent.GetComponent<CharacterElementDisplay>().index);

    }

    /// <summary>
    /// Opens Character Creation Scene.
    /// </summary>
    public void CreateNewCharacter()
    {
        SceneManager.LoadScene("CharacterCreation");
    }

}
