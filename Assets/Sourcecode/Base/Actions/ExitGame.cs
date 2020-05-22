using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Input Actions/Exit")]
public class ExitGame : InputAction
{
    public override void RespondToInput(GameController gameController, string[] seperatedInput)
    {
        Application.Quit();
    }
}
