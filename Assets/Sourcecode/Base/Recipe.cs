using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flicker/Game/Recipe")]
public class Recipe : ScriptableObject
{
    public InteractableObject ingredientOne;
    public InteractableObject ingredientTwo;
    public InteractableObject result;

    [TextArea]
    public string craftingDescription;
}
