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
    public Room valueRoom;
}
