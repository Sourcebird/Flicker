using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ActionResponses/Add Item")]
public class AddItemResponse : ActionResponse
{
    public InteractableObject[] items;
    public bool silent;

    public override bool DoActionResponse(GameController gameController)
    {
        foreach (InteractableObject item in items)
        {
            gameController.interactableItems.AddItem(item, silent);
        }
        return true;
    }
}