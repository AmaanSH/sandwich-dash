using UnityEngine;

public class ItemHolderManager : MonoBehaviour
{
    public Transform holderTransform;
    public Transform rightShoulder;
    public Transform leftShoulder;
    public HoldingPanel holdingPanel;

    public GameObject CurrentHolding { get; private set; }
    public Ingredient HoldingIngredient { get; private set; }
    public Transform OriginalParent { get; private set; }
    public Vector3 OriginalPosition { get; private set; }
    public Vector3 OriginalScale { get; private set; }

    private Quaternion originalRotation;

    public void Start()
    {
        originalRotation = leftShoulder.rotation;
    }

    public void SetItem(GameObject item)
    {
        if (CurrentHolding != null)
        {
            if (HoldingIngredient)
            {
                switch (HoldingIngredient.IngredientMode)
                {
                    case IngredientMode.Single:
                        RestoreItem();
                        break;
                    case IngredientMode.Multiple:
                        RemoveItem();
                        break;
                }
            }
            else if (HoldingIngredient == null)
            {
                RestoreItem();
            }
        }

        OriginalParent = item.transform.parent;
        OriginalPosition = item.transform.localPosition;
        OriginalScale = item.transform.localScale;

        item.transform.SetParent(holderTransform);
        item.transform.localPosition = Vector3.zero;
        //item.transform.localRotation = Quaternion.identity;

        //rightShoulder.Rotate(new Vector3(0, 0, -90));
        //leftShoulder.Rotate(new Vector3(0, 0, -90));

        CurrentHolding = item;
        
        if (item.TryGetComponent<Ingredient>(out Ingredient ingredient))
        {
            HoldingIngredient = ingredient;
            holdingPanel.SetHolding(ingredient.Icon, ingredient.IngredientName);
        }
        else
        {
            HoldingIngredient = null;
            holdingPanel.ClearHolding();
        }
    }
    
    public void RemoveItem()
    {
        if (CurrentHolding.TryGetComponent<Ingredient>(out _))
        {
            HoldingIngredient = null;
            holdingPanel.ClearHolding();
        }

        Destroy(CurrentHolding);
        CurrentHolding = null;

        //rightShoulder.rotation = originalRotation;
        //leftShoulder.rotation = originalRotation;
    }

    public void RestCurrentHolding()
    {
        if (CurrentHolding.TryGetComponent<Ingredient>(out _))
        {
            HoldingIngredient = null;
            holdingPanel.ClearHolding();
        }

        CurrentHolding = null;
    }

    public void RestoreItem()
    {
        CurrentHolding.transform.SetParent(OriginalParent);
        CurrentHolding.transform.localPosition = OriginalPosition;
        CurrentHolding.transform.localRotation = Quaternion.identity;
        CurrentHolding.transform.localScale = OriginalScale;

        if (CurrentHolding.TryGetComponent<Ingredient>(out _))
        {
            HoldingIngredient = null;
            holdingPanel.ClearHolding();
        }

        CurrentHolding = null;
    }
}
