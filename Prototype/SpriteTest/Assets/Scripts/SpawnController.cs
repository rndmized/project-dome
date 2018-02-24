using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Test class to Spawn Characters
/// </summary>
public class SpawnController : MonoBehaviour {

    public GameObject NPC;

    public int hairStyle;
    public int clothesStyle;


    Character NPC_Char;
    List<AnimatorController> HairStyles;
    List<AnimatorController> ClothesStyles;
    List<AnimatorController> BodyStyle;

    // Use this for initialization
    void Start () {

        /*
            Initialise the list references.
         
         */

        NPC_Char = ScriptableObject.CreateInstance<Character>();
        HairStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().HairStyles;
        ClothesStyles = FindObjectOfType<AssetList>().GetComponent<AssetList>().ClothesStyles;
        BodyStyle = FindObjectOfType<AssetList>().GetComponent<AssetList>().BodyStyle;

    }
	
	// Update is called once per frame
	void Update () {

        /*
      
            On T Key Pressed create an instance of a character, 
            assign values to it from the editor and spawn an instance of player's prefab
            with the given hairstyle and clothing at (0,0,0).
         
         */

        if (Input.GetKeyDown(KeyCode.T))
        {
            NPC_Char = ScriptableObject.CreateInstance<Character>();
            NPC_Char.char_clothesAnimator = ClothesStyles[clothesStyle];
            NPC_Char.char_headAnimator = HairStyles[hairStyle];
            NPC_Char.char_bodyAnimator = BodyStyle[0];
            NPC.GetComponent<CharacterRenderer>().character = NPC_Char;
            Instantiate(NPC, transform.TransformPoint(0, 0, 0), new Quaternion(0,0,0,0));
        }
		
	}
}
