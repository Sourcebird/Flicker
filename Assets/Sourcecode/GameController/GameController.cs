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
        string joinedLog = string.Join("\n", actionLog.ToArray());

        displayText.text = joinedLog;
    }

    public void PrintRoomText(bool examine = false)
    {
        ClearCollections();
        UnpackRoom();
        string joinedInteractions = string.Join("\n", interactables.ToArray());

        string combinedText;
        if (examine == true)
        {
            if (navigationController.currentRoom.examineDescription != "")
                combinedText = navigationController.currentRoom.examineDescription + "\n" + joinedInteractions;
            else
                combinedText = navigationController.currentRoom.description + "\n" + joinedInteractions;
        }
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
                interactables.Add(itemInRoom);

            InteractableObject interactableObject = currentRoom.interactableObjects[i];
            for (int j = 0; j < interactableObject.interactions.Length; j++)
            {
                Interaction interaction = interactableObject.interactions[j];
                interactionController.AddItemToDictionary(interactableObject.noun, interaction);
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
                PrintRoomText(true);
                return "!ignore";
            }

            return $"You can't {verb} {noun}";
        }
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

    private void ClearCollections()
    {
        interactionController.ClearCollections();
        interactables.Clear();
        navigationController.ClearExits();
    }
}
