using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPickupInteraction : Interaction
{
    public Ingredient ingredient;

    public override bool CanInteract()
    {
        return stateMachine.ItemHolder.HoldingIngredient != ingredient;
    }

    public override void Interact()
    {
        GameObject correct = (ingredient.IngredientModel) ? ingredient.IngredientModel : gameObject;
        GameObject gc = (ingredient.IngredientMode == IngredientMode.Single) ? correct : Instantiate(correct);

        stateMachine.ItemHolder.SetItem(gc);

        base.Exit();
    }
}
