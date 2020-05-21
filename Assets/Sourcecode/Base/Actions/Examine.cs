using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Examine")]
public class Examine : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.AddActionLog(gameController.CheckVerbDictionary(gameController.interactionController.examineDictionary, seperatedInput[0], seperatedInput[1]));
    }
}