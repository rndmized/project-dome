using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInteractionController : MonoBehaviour {

    public void Play()
    {
        PlayerProfile.cID = this.transform.parent.GetComponent<CharacterElementDisplay>().charName.text;
        SceneManager.LoadScene("Test1");
    }

    public void Delete()
    {
        this.transform.parent.transform.gameObject.SetActive(false);
        GameObject.Find("MenuController").GetComponent<CharacterSelection>().RequestCharacterDeletion(this.transform.parent.GetComponent<CharacterElementDisplay>().index);

    }

    public void CreateNewCharacter()
    {
        SceneManager.LoadScene("CharacterCreation");
    }

}
