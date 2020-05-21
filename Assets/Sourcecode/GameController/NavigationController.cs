using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public Room currentRoom;
    private GameController gameController;
    private Dictionary<string, Room> exitDictionary = new Dictionary<string, Room>();
    public Dictionary<string, string> lockDictionary = new Dictionary<string, string>();

    private void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            Exit exit = currentRoom.exits[i];
            exitDictionary.Add(exit.keyString, exit.valueRoom);
            string descripton = gameController.interactionController.GetMarkupString(exit.exitDescription, exit.keyString, gameController.interactionController.exitMarkupColor);

            if (exit.exitLock.doorObject != null && exit.exitLock.keyObject != null)
                lockDictionary.Add(exit.exitLock.doorObject.noun, exit.exitLock.keyObject.noun);

            exit.exitLock.currentState = exit.exitLock.locked;

            gameController.interactables.Add(descripton);         
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
                    if (exit.exitLock.currentState == true)
                    {
                        string description = gameController.interactionController.GetMarkupString(exit.exitLock.lockMessage, exit.exitLock.doorObject.noun, gameController.interactionController.itemMarkupColor);
                        gameController.AddActionLog(description);
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
                if (message != null && message != "")
                    gameController.AddActionLog(message);
                else
                    gameController.AddActionLog("You head off to the " + direction);
            }
            gameController.PrintRoomText();
        } 
        else
        {
            gameController.AddActionLog("You try to see a path to the " + direction + " but there simply isn't one");
        }
    }

    public void AttemptUnlock(string key, string door)
    {
        if (lockDictionary.ContainsKey(door))
        {
            for (int i = 0; i < currentRoom.exits.Length; i++)
            {
                Exit exit = currentRoom.exits[i];
                if (exit.exitLock.doorObject.noun == door)
                {
                    if (exit.exitLock.currentState == true)
                    {
                        if (exit.exitLock.keyObject.noun == key)
                        {
                            currentRoom.exits[i].exitLock.currentState = false;
                            gameController.AddActionLog(exit.exitLock.unlockMessage);
                        }
                        else
                            gameController.AddActionLog("This isn't the right place to use " + key);
                    }
                    else
                        gameController.AddActionLog("This path is allready free");
                }
                else
                    continue;
            }
        }
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
        lockDictionary.Clear();
    }
}
