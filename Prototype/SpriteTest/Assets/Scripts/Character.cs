using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    public AnimatorController char_headAnimator;

    /// <summary>
    /// Animation Controller for character's body.
    /// </summary>
    public AnimatorController char_bodyAnimator;

    /// <summary>
    /// Animation Controller for character's clothes.
    /// </summary>
    public AnimatorController char_clothesAnimator;



}
