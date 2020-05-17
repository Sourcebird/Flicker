using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector] public NavigationController roomNavigation;
    [HideInInspector] public InteractionController interactableItems;
    [HideInInspector] public List<string> interactionsInRoom = new List<string>();

    private List<string> actionLog = new List<string>();

    public Text displayText;
    public InputAction[] InputActions;

    /* PlayerVars */
    [HideInInspector] public bool hasRacoon = false;

    private void Awake()
    {
        roomNavigation = GetComponent<NavigationController>();
        interactableItems = GetComponent<InteractionController>();
    }

    private void Start()
    {
        PrintRoomText();
        PrintActionLog();
    }

    public void PrintActionLog()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());

        displayText.text = logAsText;
    }

    public void PrintRoomText()
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactionsInRoom.ToArray());

        string combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractions;
        AddActionLog(combinedText);
    }

    public void DisplayRoomExamine()
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactionsInRoom.ToArray());

        string combinedText;
        if (roomNavigation.currentRoom.examineDescription != "")
            combinedText = roomNavigation.currentRoom.examineDescription + "\n" + joinedInteractions;
        else
            combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractions;

        AddActionLog(combinedText);
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
                return "!ignore";
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

    public void AddActionLog(string action)
    {
        if (action == "!ignore")
            return;

        if (action != null && action != "")
        {
            actionLog.Add(action + "\n");
        }
        else
            Debug.LogWarning("Method 'AddActionLog' was given an invalid string | string is null or empty");
    }
}
