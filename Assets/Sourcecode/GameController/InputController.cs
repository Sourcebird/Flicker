using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    private GameController gameController;
    public InputField inputField;
    private void Awake()
    {
        gameController = GetComponent<GameController>();
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

    private void AcceptStringInput(string userInput)
    {
        userInput = userInput.ToLower();

        char[] delimiterCharacters = { ' ' };
        string[] seperatedInput = userInput.Split(delimiterCharacters);


        bool hit = false;
        for (int i = 0; i < gameController.InputActions.Length; i++)
        {
            InputAction inputAction = gameController.InputActions[i];
            if (inputAction.keyword == seperatedInput[0])
            {
                hit = true;

                if (inputAction.multiword == true && seperatedInput.Length == 1)
                    break;

                string[] itemNames = Regex.Split(Regex.Replace(string.Join(" ", seperatedInput), $"{inputAction.keyword} ", ""), " with ");
                seperatedInput[0] = inputAction.keyword;

                if (inputAction.multiword == true)
                {
                    seperatedInput[1] = itemNames[0]; seperatedInput[1] = itemNames[0];
                    if (seperatedInput.Length >= 4 && itemNames.Length > 1)
                    {
                        seperatedInput[2] = "with";
                        seperatedInput[3] = itemNames[1];
                    }
                }

                gameController.AddActionLog(userInput);
                inputAction.RespondToInput(gameController, seperatedInput);
            }
        }

        if (hit == false && seperatedInput[0] != "")
            gameController.AddActionLog("Unknown command: " + seperatedInput[0]);

        InputComplete();
    }

    private void InputComplete()
    {
        gameController.PrintActionLog();
        inputField.ActivateInputField();
        inputField.text = "";
    }
}
