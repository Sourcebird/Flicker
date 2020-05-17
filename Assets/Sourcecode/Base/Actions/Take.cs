using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/InputActions/Take")]
public class Take : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        Dictionary<string, string> takeDictionary = gameController.interactableItems.Take(seperatedInput);
        if (takeDictionary != null)
        {
            gameController.AddActionLog(gameController.CheckVerbDictionary(takeDictionary, seperatedInput[0], seperatedInput[1]));
        }
    }
}