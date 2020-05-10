using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputAction : ScriptableObject
{
    public string keyword;

    public string CombineInput(string[] seperatedInput)
    {
        if (seperatedInput.Length > 2)
            return seperatedInput[1] + " " + seperatedInput[2];
        else
            return seperatedInput[1];
    }
    public abstract void RespondToInput(GameController gameController, string[] seperatedInput);
}
