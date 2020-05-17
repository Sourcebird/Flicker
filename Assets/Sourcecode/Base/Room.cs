using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Room")]
public class Room : ScriptableObject
{
    [TextArea]
    public string description;
    [TextArea]
    public string examineDescription;
    public string roomName;
    public bool SuppressLeave;
    public Exit[] exits;
    public InteractableObject[] interactableObjects;
}
