using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/InputActions/Examine")]
public class Examine : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.LogAction(gameController.CheckVerbDictionary(gameController.interactableItems.examineDictionary, seperatedInput[0], seperatedInput[1]));
    }
}
