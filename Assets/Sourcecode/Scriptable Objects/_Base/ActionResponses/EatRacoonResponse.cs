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
        gameController.player.HasRacoon = true;
        gameController.LogAction(description);
        return true;
    }
}
