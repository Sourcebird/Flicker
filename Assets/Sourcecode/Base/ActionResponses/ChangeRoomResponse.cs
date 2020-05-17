using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ActionResponses/Change Room")]
public class ChangeRoomResponse : ActionResponse
{
    public Room targetRoom;

    public override bool DoActionResponse(GameController gameController)
    {
        if (gameController.roomNavigation.currentRoom.roomName == requiredKey)
        {
            gameController.roomNavigation.currentRoom = targetRoom;
            gameController.PrintRoomText();
            return true;
        }

        return false;
    }
}
