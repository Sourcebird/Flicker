using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Action Responses/Change Room")]
public class ChangeRoomResponse : ActionResponse
{
    public Room targetRoom;

    public override bool CheckActionResponse(GameController gameController)
    {
        if (gameController.navigationController.currentRoom.roomName == requiredKey)
        {
            return true;
        }

        return false;
    }

    public override bool DoActionResponse(GameController gameController)
    {
        if (gameController.navigationController.currentRoom.roomName == requiredKey)
        {
            gameController.navigationController.currentRoom = targetRoom;
            gameController.PrintRoomText();
            return true;
        }

        return false;
    }
}
