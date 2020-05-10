using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/InputActions/Eat")]
public class Eat : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        seperatedInput[1] = CombineInput(seperatedInput);
        gameController.interactableItems.EatItem(seperatedInput);
    }
}
