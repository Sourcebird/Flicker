using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector] public NavigationController navigationController;
    [HideInInspector] public InteractionController interactionController;
    [HideInInspector] public List<string> interactables = new List<string>();

    private List<string> actionLog = new List<string>();

    public Text displayText;
    public InputAction[] InputActions;

    /* PlayerVars */
    [HideInInspector] public bool hasRacoon = false;

    private void Awake()
    {
        navigationController = GetComponent<NavigationController>();
        interactionController = GetComponent<InteractionController>();
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
        string joinedInteractions = string.Join("\n", interactables.ToArray());

        string combinedText = navigationController.currentRoom.description + "\n" + joinedInteractions;
        AddActionLog(combinedText);
    }

    public void DisplayRoomExamine()
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactables.ToArray());

        string combinedText;
        if (navigationController.currentRoom.examineDescription != "")
            combinedText = navigationController.currentRoom.examineDescription + "\n" + joinedInteractions;
        else
            combinedText = navigationController.currentRoom.description + "\n" + joinedInteractions;

        AddActionLog(combinedText);
    }

    private void UnpackRoom()
    {
        navigationController.UnpackExitsInRoom();
        PrepareObjects(navigationController.currentRoom);
    }

    private void PrepareObjects(Room currentRoom)
    {
        for (int i = 0; i < currentRoom.interactableObjects.Length; i++)
        {
            string itemInRoom = interactionController.GetObjectsNotInInventory(currentRoom, i);
            if (itemInRoom != null)
            {
                interactables.Add(itemInRoom);
            }

            InteractableObject interactableInRoom = currentRoom.interactableObjects[i];
            for (int j = 0; j < interactableInRoom.interactions.Length; j++)
            {
                Interaction interaction = interactableInRoom.interactions[j];
                interactionController.AddItemToDictionary(interactableInRoom.noun, interaction);
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
        interactionController.ClearCollections();
        interactables.Clear();
        navigationController.ClearExits();
    }

    public void AddActionLog(string action)
    {
        if (action != null && action != "")
        {
            if (action == "!ignore")
                return;

            actionLog.Add(action + "\n");
        }
        else
            Debug.LogWarning("Method 'AddActionLog' was given an invalid string | string is null or empty");
    }
}
