using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExitLock
{
    public InteractableObject doorObject;
    public InteractableObject keyObject;
    public bool locked = false;
    [HideInInspector] public bool currentState;
    [TextArea(15, 20)]
    public string lockMessage = "Message to be displayed if door is locked";
    [TextArea(15, 20)]
    public string unlockMessage = "Message to be displayed when door is unlocked";
}
