using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItems : MonoBehaviour
{
    public List<InteractableObject> itemList;
    public List<InteractableObject> usableItemList;

    public Dictionary<string, string> examineDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> takeDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> eatDictionary = new Dictionary<string, string>();

    private Dictionary<string, ActionResponse> useDictionary = new Dictionary<string, ActionResponse>();

    [HideInInspector] public List<string> ItemsInRoom = new List<string>();
    private List<string> ItemsInInventory = new List<string>();
    private List<string> ItemsInVoid = new List<string>();
    private GameController gameController;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    public string GetObjectsNotInInventory(Room currentRoom, int iterator)
    {
        InteractableObject interactableObject = currentRoom.interactableObjects[iterator];

        if (!ItemsInInventory.Contains(interactableObject.noun) && !ItemsInVoid.Contains(interactableObject.noun))
        {
            ItemsInRoom.Add(interactableObject.noun);
            return interactableObject.description;
        }
        else
            return null;
    }

    public void AddActionResponsesToUseDictionary()
    {
        for (int i = 0; i < ItemsInInventory.Count; i++)
        {
            string item = ItemsInInventory[i];

            InteractableObject interactableObject = GetInteractableObject(item);
            if (interactableObject == null)
                continue;

            for (int j = 0; j < interactableObject.interactions.Length; j++)
            {
                Interaction interaction = interactableObject.interactions[j];
                if (interaction.ActionResponse == null)
                    continue;

                if (!useDictionary.ContainsKey(item))
                {
                    useDictionary.Add(item, interaction.ActionResponse);
                }
            }
        }
    }

    private InteractableObject GetInteractableObject(string item)
    {
        for (int i = 0; i < usableItemList.Count; i++)
        {
            if (usableItemList[i].noun == item)
            {
                return usableItemList[i];
            }
        }

        return null;
    }

    public void DisplayInventory()
    {
        if (ItemsInInventory.Count > 0)
        {
            gameController.LogAction("A look in your backpack shows: ");
            for (int i = 0; i < ItemsInInventory.Count; i++)
            {
                gameController.LogAction($"> {ItemsInInventory[i]}");
            }
        }
        else
        {
            gameController.LogAction("You backpack seems to be empty");
        }
    }

    public void ClearCollections()
    {
        examineDictionary.Clear();
        takeDictionary.Clear();
        eatDictionary.Clear();
        ItemsInRoom.Clear();
    }

    public Dictionary<string, string> Take (string[] seperatedInput)
    {
        string item = seperatedInput[1];
        if (ItemsInRoom.Contains(item))
        {
            if (takeDictionary.ContainsKey(item))
            {
                InteractableObject interactableObject = itemList.Find(x => x.noun == item);
                Interaction interaction = GetInteraction(interactableObject, "take");
                if (interaction != null)
                {
                    if (interaction.successful == false)
                        return takeDictionary;
                }

                ItemsInInventory.Add(item);
                AddActionResponsesToUseDictionary();
                ItemsInRoom.Remove(item);
                return takeDictionary;
            }
            gameController.LogAction(item + " is too untakable to take");
            return null;
        }
        else
        {
            gameController.LogAction("There is no " + item + " to take.");
            return null;
        }
    }

    public void UseItem(string[] seperatedInput)
    {
        string item = seperatedInput[1];
        if (ItemsInInventory.Contains(item))
        {
            if (useDictionary.ContainsKey(item))
            {
                bool actionResult = useDictionary[item].DoActionResponse(gameController);
                if (!actionResult)
                {
                    gameController.LogAction("Strange. Nothing seems to change.");
                }
                else
                {
                    InteractableObject interactableObject = usableItemList.Find(x => x.noun == item);
                    if (interactableObject.breaksOnUse)
                    {
                        gameController.LogAction(interactableObject.breakDescription);
                        ItemsInRoom.Remove(item);
                    }
                }
            }
            else
            {
                gameController.LogAction("You can't use the " + item);
            }
        }
        else
        {
            gameController.LogAction("You try to use the " + item + " but... Wait. There's no " + item);
        }
    }

    public void EatItem(string[] seperatedInput)
    {

        string item = seperatedInput[1];
        if (ItemsInRoom.Contains(item) || ItemsInInventory.Contains(item))
        {
            if (eatDictionary.ContainsKey(item))
            {
                InteractableObject interactableObject = itemList.Find(x => x.noun == item);
                for (int i = 0; i < interactableObject.interactions.Length; i++)
                {
                    Interaction interaction = interactableObject.interactions[i];
                    if (interaction.inputAction.keyword == "eat")
                    {
                        if (interaction.successful == true)
                        {
                            if (ItemsInRoom.Contains(item))
                                ItemsInRoom.Remove(item);
                            if (ItemsInInventory.Contains(item))
                                ItemsInInventory.Remove(item);

                            ItemsInVoid.Add(item);

                            if (interaction.ActionResponse != null)
                                interaction.ActionResponse.DoActionResponse(gameController);
                            else
                                gameController.LogAction(interaction.textResponse);
                            return;
                        }
                        else
                        {
                            gameController.LogAction(interaction.textResponse);
                            return;
                        }
                    }
                }
            }
            else
            {
                gameController.LogAction("You can't eat " + item);
            }
        }
        else
        {
            gameController.LogAction("There's no " + item + " to eat");
        }
    }

    private Interaction GetInteraction(InteractableObject interactableObject, string keyword)
    {
        for (int i = 0; i < interactableObject.interactions.Length; i++)
        {
            Interaction interaction = interactableObject.interactions[i];
            if (interaction.inputAction.keyword == keyword)
                return interaction;
            else
                continue;
        }

        return null;
    }
}
