using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterElementDisplay Script Renders Character based on CharacterInfo.
/// </summary>
public class CharacterElementDisplay : MonoBehaviour {


    public Text charName;
    public Image spriteHair;
    public Image spriteClothes;
    public Image spriteBody;

    public int index;

    public CharacterInfo charInfo;

    /// <summary>
    /// Initializes this to CharacterInfo in this GameObject.
    /// </summary>
    /// <param name="charInfo">CharacterInfo values to Display.</param>
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

	/// <summary>
    /// Initializer.
    /// </summary>
	void Start () {
        if (charInfo != null)
        {
            init(charInfo);
        }
	}


}
