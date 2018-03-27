using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterElementDisplay : MonoBehaviour {


    public Text charName;
    public Image spriteHair;
    public Image spriteClothes;
    public Image spriteBody;

    public int index;

    public CharacterInfo charInfo;



    public void init(CharacterInfo charInfo)
    {
        this.charInfo = charInfo;
        if (charInfo.char_name != null)
        {
            charName.text = charInfo.char_name;
        }

        spriteHair.sprite = FindObjectOfType<UIAssetList>().GetComponent<UIAssetList>().HairStyles[charInfo.char_hairId];
        spriteClothes.sprite = FindObjectOfType<UIAssetList>().GetComponent<UIAssetList>().ClothesStyles[charInfo.char_clothesId];
        spriteBody.sprite = FindObjectOfType<UIAssetList>().GetComponent<UIAssetList>().BodyStyle[charInfo.char_bodyId];

    }

	// Use this for initialization
	void Start () {
        if (charInfo != null)
        {
            init(charInfo);
        }
	}


}
