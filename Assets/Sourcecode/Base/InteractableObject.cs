using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Game/Interactable Object")]
public class InteractableObject : ScriptableObject
{
    public string noun = "name";
    [TextArea(15,20)]
    public string description = "description in room";
    public string devcomment = "yes";
    public bool breaksOnUse = false;
    [TextArea]
    public string breakDescription = "Message when Object breaks";
    public Interaction[] interactions;
}