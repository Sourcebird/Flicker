using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Combine")]
public class Combine : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.interactionController.CombineItems(seperatedInput);
    }
}