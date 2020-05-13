using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

public class InteractableItems : MonoBehaviour
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

    public Color ItemMarkupColor;

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
            markupDescription = Regex.Replace(markupDescription, @"\b" + interactableObject.noun + @"\b", "<color=#" + ColorUtility.ToHtmlStringRGB(ItemMarkupColor) + ">" + interactableObject.noun + "</color>");
            return markupDescription;
        }
        else
            return null;
    }

    public string ReplaceWithMarkup(string replace, Color color)
    {
        string colorstring = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorstring}>{replace}</color>";
    }
    public string GetMarkupString(string description, string replace, Color color)
    {
        return GetMarkupString(description, new string[] { replace }, color);
    }
    public string GetMarkupString(string description, string[] replace, Color color)
    {
        string output = description;
        foreach (string item in replace)
        {
            output = Regex.Replace(output, item, ReplaceWithMarkup(item, color));
        }

        return output;
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
                    gameController.LogAction(item.description);
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
        if (seperatedInput == null)
            return;

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
                        description = GetMarkupString(description, new string[] { seperatedInput[1], seperatedInput[3], recipe.result.noun }, ItemMarkupColor);

                        Debug.Log(description);
                        gameController.LogAction(description);

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
                errorMsg = GetMarkupString(errorMsg, new string[] { seperatedInput[1], seperatedInput[3] }, ItemMarkupColor);
                gameController.LogAction(errorMsg);
            }
            else
                gameController.LogAction("You can't combine non-existing objects.");
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
