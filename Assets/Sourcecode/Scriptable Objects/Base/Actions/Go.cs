using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/InputActions/Go")]
public class Go : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.roomNavigation.AttemptToChangeRooms(seperatedInput[1]);
    }
}
