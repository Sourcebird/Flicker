using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Game/Room")]
public class Room : ScriptableObject
{
    public string roomName;
    [TextArea(15,20)]
    public string description;
    [TextArea(15,20)]
    public string examineDescription;
    public bool SuppressLeave;
    public Exit[] exits;
    public InteractableObject[] interactableObjects;
}
