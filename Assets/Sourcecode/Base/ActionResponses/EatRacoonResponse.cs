using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/[STORY]ActionResponses/Eat Racoon")]
public class EatRacoonResponse : ActionResponse
{
    [TextArea]
    public string description;

    public override bool DoActionResponse(GameController gameController)
    {
        gameController.hasRacoon = true;
        gameController.AddActionLog(description);
        return true;
    }
}
