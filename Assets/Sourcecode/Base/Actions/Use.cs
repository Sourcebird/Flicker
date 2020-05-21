using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Use")]
public class Use : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.interactionController.UseItem(seperatedInput);
    }
}