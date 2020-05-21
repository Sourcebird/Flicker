using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interaction
{
    public InputAction inputAction;
    public bool successful = true;
    [TextArea]
    public string textResponse;
    public ActionResponse ActionResponse = null;
}
