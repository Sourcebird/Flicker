using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector] public RoomNavigation roomNavigation;
    [HideInInspector] public InteractableItems interactableItems;
    [HideInInspector] public Player player;
    [HideInInspector] public List<string> interactionsInRoom = new List<string>();
    public Text displayText;
    List<string> actionLog = new List<string>();
    public InputAction[] InputActions;

    private void Awake()
    {
        roomNavigation = GetComponent<RoomNavigation>();
        interactableItems = GetComponent<InteractableItems>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        DisplayRoomText();
        DisplayLog();
    }

    public void DisplayLog()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());

        displayText.text = logAsText;
    }

    public void DisplayRoomText()
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactionsInRoom.ToArray());

        string combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractions;
        LogAction(combinedText);
    }

    public void DisplayRoomExamine()
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactionsInRoom.ToArray());

        string combinedText = "";
        if (roomNavigation.currentRoom.examineDescription != "")
            combinedText = roomNavigation.currentRoom.examineDescription + "\n" + joinedInteractions;
        else
            combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractions;

        LogAction(combinedText);
    }

    private void UnpackRoom()
    {
        roomNavigation.UnpackExitsInRoom();
        PrepareObjects(roomNavigation.currentRoom);
    }

    private void PrepareObjects(Room currentRoom)
    {
        for (int i = 0; i < currentRoom.interactableObjects.Length; i++)
        {
            string itemInRoom = interactableItems.GetObjectsNotInInventory(currentRoom, i);
            if (itemInRoom != null)
            {
                interactionsInRoom.Add(itemInRoom);
            }

            InteractableObject interactableInRoom = currentRoom.interactableObjects[i];
            for (int j = 0; j < interactableInRoom.interactions.Length; j++)
            {
                Interaction interaction = interactableInRoom.interactions[j];
                interactableItems.AddItemToDictionary(interactableInRoom.noun, interaction);
            }
        }
    }

    public string CheckVerbDictionary(Dictionary<string, string> verbDictionary, string verb, string noun)
    {
        if (verbDictionary.ContainsKey(noun))
        {
            return verbDictionary[noun];
        }
        else
        {
            if (verb == "examine" && noun == "room")
            {
                DisplayRoomExamine();
                return null;
            }

            return $"You can't {verb} {noun}";
        }
    }

    private void ClearCollections()
    {
        interactableItems.ClearCollections();
        interactionsInRoom.Clear();
        roomNavigation.ClearExits();
    }

    public void LogAction(string action)
    {
        if (action != null)
        {
            actionLog.Add(action + "\n");
        }
    }
}
