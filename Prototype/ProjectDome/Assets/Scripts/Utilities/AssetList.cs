
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store animation resources (Temporary until assets can be loaded properly)
/// </summary>
public class AssetList : MonoBehaviour {

    public List<RuntimeAnimatorController> BodyStyle = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> HairStyles = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> ClothesStyles = new List<RuntimeAnimatorController>();

}
