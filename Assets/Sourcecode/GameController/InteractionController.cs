using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public List<InteractableObject> itemList;
    public List<InteractableObject> usableItemList;

    public List<Recipe> recipeList;

    public Dictionary<string, string> examineDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> takeDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> eatDictionary = new Dictionary<string, string>();

    private Dictionary<string, ActionResponse> useDictionary = new Dictionary<string, ActionResponse>();

    [HideInInspector] public List<string> ItemsInRoom = new List<string>();
    private List<string> ItemsInInventory = new List<string>();
    private List<string> ItemsInVoid = new List<string>();
    private GameController gameController;

    public Color itemMarkupColor;
    public Color exitMarkupColor;

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

            string markupDescription = interactableObject.description;
            int index = markupDescription.IndexOf(interactableObject.noun);
            markupDescription = Regex.Replace(markupDescription, @"\b" + interactableObject.noun + @"\b", "<color=#" + ColorUtility.ToHtmlStringRGB(itemMarkupColor) + ">" + interactableObject.noun + "</color>");
            return markupDescription;
        }
        else
            return null;
    }

    public string GetMarkupString(string source, string replace, Color color)
    {
        string colorstring = ColorUtility.ToHtmlStringRGB(color);
        return source.Replace(replace, $"<color=#{colorstring}>{replace}</color>");
    }
    public string GetMarkupArray(string source, string[] replaces, Color color)
    {
        string markup = source;
        foreach (string replace in replaces)
        {
            markup = GetMarkupString(markup, replace, color);
        }
        return markup;
    }

    public string SetItemColor(InteractableObject interactableObject, string source)
    {
        throw new NotImplementedException();
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
            gameController.AddActionLog("A look in your backpack shows: ");
            for (int i = 0; i < ItemsInInventory.Count; i++)
            {
                gameController.AddActionLog($"> {ItemsInInventory[i]}");
            }
        }
        else
        {
            gameController.AddActionLog("You backpack seems to be empty");
        }
    }

    public void AddItem(InteractableObject item, bool silent)
    {
        if (item != null)
        {
            if (itemList.Contains(item))
            {
                ItemsInInventory.Add(item.noun);
                foreach (Interaction interaction in item.interactions)
                {
                    AddItemToDictionary(item.noun, interaction);
                }

                if (!silent)
                    gameController.AddActionLog(item.description);
            }
        }
    }

    public void AddItemToDictionary(string noun, Interaction interaction)
    {
        switch (interaction.inputAction.keyword)
        {
            case "examine":
                examineDictionary.Add(noun, interaction.textResponse);
                break;
            case "take":
                takeDictionary.Add(noun, interaction.textResponse);
                break;
            case "eat":
                eatDictionary.Add(noun, interaction.textResponse);
                break;
            default:
                break;
        }
    }

    public void ClearCollections()
    {
        examineDictionary.Clear();
        takeDictionary.Clear();
        eatDictionary.Clear();
        ItemsInRoom.Clear();
    }

    public void DevInspectItem(string[] seperatedInput)
    {
        if (seperatedInput.Length >= 2)
        {
            InteractableObject interactableObject = GetInteractableObject(seperatedInput[1]);
            if (interactableObject != null && interactableObject.devcomment != "")
                gameController.AddActionLog("[Darkyne]: " + interactableObject.devcomment);
        }
    }

    public Dictionary<string, string> Take (string[] seperatedInput)
    {
        if (seperatedInput == null)
            return null;

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
            gameController.AddActionLog(item + " is too untakable to take");
            return null;
        }
        else
        {
            gameController.AddActionLog("There is no " + item + " to take.");
            return null;
        }
    }

    public void UseItem(string[] seperatedInput)
    {
        if (seperatedInput == null)
            return;

        string item = seperatedInput[1];
        if (ItemsInInventory.Contains(item))
        {
            if (seperatedInput.Length >= 4 && gameController.navigationController.lockDictionary.ContainsKey(seperatedInput[3]))
            {
                string target = seperatedInput[3];

                Dictionary<string, string> lockDict = gameController.navigationController.lockDictionary;
                if (lockDict[target] == item)
                {
                    gameController.navigationController.AttemptUnlock(item, target);
                    return;
                }
                else
                {
                    gameController.AddActionLog("This doesn't seem to be the right thing to do.");
                }
            }
            else if (useDictionary.ContainsKey(item))
            {
                bool actionResult = useDictionary[item].CheckActionResponse(gameController);
                if (!actionResult)
                {
                    gameController.AddActionLog("Strange. Nothing seems to change.");
                }
                else
                {
                    InteractableObject interactableObject = usableItemList.Find(x => x.noun == item);
                    if (interactableObject.breaksOnUse)
                    {
                        gameController.AddActionLog(interactableObject.breakDescription);
                        ItemsInRoom.Remove(item);
                        useDictionary[item].DoActionResponse(gameController);
                    }
                }
            }
            else
            {
                gameController.AddActionLog("You can't use the " + item);
            }
        }
        else
        {
            gameController.AddActionLog("You try to use the " + item + " but... Wait. There's no " + item + " in your pockets.");
        }
    }

    public void EatItem(string[] seperatedInput)
    {
        if (seperatedInput == null)
            return;

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
                                gameController.AddActionLog(interaction.textResponse);
                            return;
                        }
                        else
                        {
                            gameController.AddActionLog(interaction.textResponse);
                            return;
                        }
                    }
                }
            }
            else
            {
                gameController.AddActionLog("You can't eat " + item);
            }
        }
        else
        {
            gameController.AddActionLog("There's no " + item + " to eat");
        }
    }

    public void CombineItems(string[] seperatedInput)
    {
        if (seperatedInput == null)
            return;

        if (seperatedInput.Length >= 4)
        {
            if (ItemsInInventory.Contains(seperatedInput[1]) || ItemsInRoom.Contains(seperatedInput[1]) && ItemsInInventory.Contains(seperatedInput[3]) || ItemsInRoom.Contains(seperatedInput[3]))
            {
                foreach (Recipe recipe in recipeList)
                {
                    if (seperatedInput.Contains(recipe.ingredientOne.noun) && seperatedInput.Contains(recipe.ingredientTwo.noun))
                    {
                        AddItem(recipe.result, true);

                        string description = recipe.craftingDescription;
                        description = GetMarkupArray(description, new string[] { seperatedInput[1], seperatedInput[3], recipe.result.noun }, itemMarkupColor);

                        Debug.Log(description);
                        gameController.AddActionLog(description);

                        if (ItemsInInventory.Contains(seperatedInput[1]))
                            ItemsInInventory.Remove(seperatedInput[1]);
                        else
                            ItemsInRoom.Remove(seperatedInput[1]);

                        if (ItemsInInventory.Contains(seperatedInput[3]))
                            ItemsInInventory.Remove(seperatedInput[3]);
                        else
                            ItemsInRoom.Remove(seperatedInput[3]);

                        return;
                    }
                }

                string errorMsg = "you can't combine " + seperatedInput[1] + " with " + seperatedInput[3];
                errorMsg = GetMarkupArray(errorMsg, new string[] { seperatedInput[1], seperatedInput[3] }, itemMarkupColor);
                gameController.AddActionLog(errorMsg);
            }
            else
                gameController.AddActionLog("You can't combine non-existing objects.");
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
