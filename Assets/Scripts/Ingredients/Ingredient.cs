using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Bread,
    RaspberryJam,
    StrawberryJam,
    Marmalade,
    GrapeJelly
}

public enum IngredientMode
{
    Single,
    Multiple
}

public class Ingredient : MonoBehaviour
{
    [field: SerializeField] public IngredientType IngredientType { get; protected set; }
    [field: SerializeField] public IngredientMode IngredientMode { get; protected set; }
    [field: SerializeField] public string IngredientName { get; protected set; }
    [field: SerializeField] public GameObject IngredientModel { get; protected set; }
    [field: SerializeField] public Sprite Icon { get; protected set; }
}
