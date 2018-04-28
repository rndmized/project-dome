using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object containing Character core porperties.
/// </summary>
[CreateAssetMenu (fileName ="New Character", menuName ="Character")]
public class Character : ScriptableObject {
    /// <summary>
    /// Character name
    /// </summary>
    public string char_name;
    /// <summary>
    /// Chracter ID
    /// </summary>
    public string ID;

    /// <summary>
    /// Animation Controller for character's head.
    /// </summary>
    public RuntimeAnimatorController char_headAnimator;

    /// <summary>
    /// Animation Controller for character's body.
    /// </summary>
    public RuntimeAnimatorController char_bodyAnimator;

    /// <summary>
    /// Animation Controller for character's clothes.
    /// </summary>
    public RuntimeAnimatorController char_clothesAnimator;



}
