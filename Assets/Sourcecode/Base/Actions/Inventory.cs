using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Inventory")]
public class Inventory : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        gameController.interactionController.DisplayInventory();
    }
}
