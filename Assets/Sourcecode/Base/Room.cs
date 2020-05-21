using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Game/Room")]
public class Room : ScriptableObject
{
    [TextArea(15,20)]
    public string description;
    [TextArea(15,20)]
    public string examineDescription;
    public string roomName;
    public bool SuppressLeave;
    public Exit[] exits;
    public InteractableObject[] interactableObjects;
}
