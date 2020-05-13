using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Exit
{
    public string keyString;
    [TextArea]
    public string exitDescription;
    [TextArea]
    public string exitMessage;
    public InteractableObject doorObject;
    public InteractableObject keyObject;
    public bool locked;
    [TextArea]
    public string lockedMessage;
    [TextArea]
    public string unlockMessage;
    public Room valueRoom;
}
