using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Developer Inspect")]
public class Dev : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.interactionController.DevInspectItem(seperatedInput);
    }
}
