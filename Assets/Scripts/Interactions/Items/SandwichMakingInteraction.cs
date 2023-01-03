using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SandwichStage
{
    BaseBread,
    Jam,
    TopBread,
    Transfer
}

[System.Serializable]
public class JamModel
{
    public IngredientType jamType;
    public GameObject gameObject;
}

public class SandwichMakingInteraction : Interaction
{
    public Transform holder;
    public JamModel[] jamModels;

    private SandwichStage stage = SandwichStage.BaseBread;
    private List<Ingredient> currentIngredients = new List<Ingredient>();

    public override bool CanInteract()
    {
        return stateMachine.ItemHolder.HoldingIngredient && !stateMachine.HasCompletedOrder;
    }

    public override void Interact()
    {
        // OKAY.. so first lets see what stage we're on
        Ingredient currentHolding = stateMachine.ItemHolder.HoldingIngredient;

        switch (stage)
        {
            case SandwichStage.BaseBread:
            case SandwichStage.TopBread:
                if (currentHolding.IngredientType == IngredientType.Bread)
                {
                    // store the current ingredient in the list
                    currentIngredients.Add(currentHolding);

                    // move this to the transform
                    stateMachine.ItemHolder.CurrentHolding.transform.SetParent(holder);
                    stateMachine.ItemHolder.CurrentHolding.transform.localPosition = Vector3.zero;

                    if (stage == SandwichStage.TopBread)
                    {
                        stateMachine.ItemHolder.CurrentHolding.transform.localPosition = stateMachine.ItemHolder.CurrentHolding.transform.localPosition + new Vector3(0f, 0.02f, 0f);
                    }

                    stateMachine.ItemHolder.CurrentHolding.transform.rotation = Quaternion.identity;
                    stateMachine.ItemHolder.CurrentHolding.transform.rotation = Quaternion.Euler(-90, 0, 0);

                    stateMachine.ItemHolder.RestCurrentHolding();

                    if (stage == SandwichStage.BaseBread)
                    {
                        stage = SandwichStage.Jam;
                    }
                    else
                    {
                        stage = SandwichStage.Transfer;
                    }
                }
                break;
            case SandwichStage.Jam:
                if (currentHolding.IngredientType != IngredientType.Bread)
                {
                    // store the current ingredient in the list
                    currentIngredients.Add(currentHolding);

                    // move this to the transform
                    stateMachine.ItemHolder.RestoreItem();

                    // find jam object
                    GameObject jam = GetJamModel(currentHolding.IngredientType);
                    GameObject cloned = Instantiate(jam, holder);
                    cloned.transform.position = cloned.transform.position + new Vector3(0f, 0.01f, 0f);

                    // move to the next stage
                    stage = SandwichStage.TopBread;
                }
                break;
            case SandwichStage.Transfer:
                if (currentHolding.IngredientType == IngredientType.Plate)
                {
                    Transform sandwichPlateHolder = stateMachine.ItemHolder.CurrentHolding.transform.GetChild(0);

                    Instantiate(holder.gameObject, sandwichPlateHolder);

                    // HACKY!
                    sandwichPlateHolder.localScale = new Vector3(10f, 10f, 10f);

                    foreach (Transform child in holder)
                    {
                        Destroy(child.gameObject);
                    }

                    stateMachine.SetOrderReady(true);
                    stateMachine.SetOrderJam(currentIngredients[1].IngredientType);

                    currentIngredients.Clear();
                    stage = SandwichStage.BaseBread;
                }
                break;
        }

        base.Exit();
    }

    private GameObject GetJamModel(IngredientType jam)
    {
        foreach (JamModel jamModel in jamModels)
        {
            if (jamModel.jamType == jam)
            {
                return jamModel.gameObject;
            }
        }

        return null;
    }
}