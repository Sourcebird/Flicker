using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{
    public Room currentRoom;
    private GameController gameController;
    Dictionary<string, Room> exitDictionary = new Dictionary<string, Room>();

    private void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            exitDictionary.Add(currentRoom.exits[i].keyString, currentRoom.exits[i].valueRoom);
            string descripton = gameController.interactableItems.GetMarkupString(currentRoom.exits[i].exitDescription, currentRoom.exits[i].keyString, gameController.interactableItems.ItemMarkupColor);
            gameController.interactionsInRoom.Add(descripton);         
        }
    }

    public void AttemptToChangeRooms(string direction)
    {
        if (exitDictionary.ContainsKey(direction))
        {
            bool silent = currentRoom.SuppressLeave;
            string message = null;

            for (int i = 0; i < currentRoom.exits.Length; i++)
            {
                Exit exit = currentRoom.exits[i];
                if (exit.keyString == direction)
                { 
                    if (exit.locked == true)
                    {
                        gameController.LogAction(exit.lockedMessage);
                        return;
                    }
                    else
                        message = exit.exitMessage;
                }
                else
                    continue;
            }

            currentRoom = exitDictionary[direction];
            if (!silent)
            {
                if (message != null)
                    gameController.LogAction(message);
                else
                    gameController.LogAction("You head off to the " + direction);
            }
            gameController.DisplayRoomText();
        } 
        else
        {
            gameController.LogAction("You try to see a path to the " + direction + " but there simply isn't one");
        }
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }
}
