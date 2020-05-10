using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Interactable Object")]
public class InteractableObject : ScriptableObject
{
    public string noun = "name";
    [TextArea]
    public string description = "description in room";
    public string devcomment = "yes";
    public bool breaksOnUse = false;
    [TextArea]
    public string breakDescription = "broke";
    public Interaction[] interactions;
}