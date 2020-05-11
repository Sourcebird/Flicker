using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
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
        gameController.LogAction(userInput);

        char[] delimiterCharacters = { ' ' };
        string[] seperatedInput = userInput.Split(delimiterCharacters);

        for (int i = 0; i < gameController.InputActions.Length; i++)
        {
            InputAction inputAction = gameController.InputActions[i];
            if (inputAction.keyword == seperatedInput[0])
            {
                if (inputAction.multiword == true && seperatedInput.Length == 1)
                    break;

                inputAction.RespondToInput(gameController, seperatedInput);
            }
        }
        InputComplete();
    }

    private void InputComplete()
    {
        gameController.DisplayLog();
        inputField.ActivateInputField();
        inputField.text = "";
    }
}
