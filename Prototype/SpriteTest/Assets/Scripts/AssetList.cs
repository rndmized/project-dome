using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Test Class to store animation resources (Temporary until assets can be loaded properly)
/// </summary>
public class AssetList : MonoBehaviour {

    public List<AnimatorController> BodyStyle = new List<AnimatorController>();
    public List<AnimatorController> HairStyles = new List<AnimatorController>();
    public List<AnimatorController> ClothesStyles = new List<AnimatorController>();

}
