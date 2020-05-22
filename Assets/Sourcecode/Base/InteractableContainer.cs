using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Game/Container")]
public class InteractableContainer : ScriptableObject
{
    public string keyString;
    [TextArea(15, 20)]
    public string description;
    public InteractableObject[] inventory;


    /*
        Not done
     */
}
