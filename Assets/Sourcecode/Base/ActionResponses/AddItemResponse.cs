using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Action Responses/Add Item")]
public class AddItemResponse : ActionResponse
{
    public InteractableObject[] items;
    public bool silent;

    public override bool CheckActionResponse(GameController gameController)
    {
        return true;
    }

    public override bool DoActionResponse(GameController gameController)
    {
        foreach (InteractableObject item in items)
        {
            gameController.interactionController.AddItem(item, silent);
        }
        return true;
    }
}