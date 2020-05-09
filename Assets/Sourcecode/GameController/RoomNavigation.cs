using System.Collections;
using System.Collections.Generic;
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
            gameController.interactionsInRoom.Add(currentRoom.exits[i].exitDescription);         
        }
    }

    public void AttemptToChangeRooms(string direction)
    {
        if (exitDictionary.ContainsKey(direction))
        {
            bool silent = currentRoom.SuppressLeave;
            currentRoom = exitDictionary[direction];
            if (!silent)
                gameController.LogAction("You head off to the " + direction);
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
