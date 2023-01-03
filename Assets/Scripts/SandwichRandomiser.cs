using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JamSprite
{
    public IngredientType jamType;
    public Sprite sprite;
}

public class SandwichRandomiser : MonoBehaviour
{
    public Sprite sandwichSprite;
    public JamSprite[] jams;

    public Order GenerateOrder()
    {
        IngredientType jam = GetJam();

        return new Order
        {
            jam = jam,
            jamName = GetNameFromEnum(jam),
            maxTime = Mathf.FloorToInt(GetOrderTime())
        };
    }

    private float GetOrderTime()
    {
        // get some random seconds for the order based of how long the game has been running for a challenge
        return Random.Range(20f, 30f);
    }

    private IngredientType GetJam()
    {
        int random = Random.Range(0, 4);
        
        switch(random)
        {
            case 0:
                return IngredientType.RaspberryJam;
            case 1:
                return IngredientType.StrawberryJam;
            case 2:
                return IngredientType.Marmalade;
            case 3:
                return IngredientType.GrapeJelly;
        }

        return IngredientType.RaspberryJam;
    }

    private string GetNameFromEnum(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Bread:
                return "Bread";
            case IngredientType.RaspberryJam:
                return "Raspberry Jam";
            case IngredientType.StrawberryJam:
                return "Strawberry Jam";
            case IngredientType.Marmalade:
                return "Marmalade";
            case IngredientType.GrapeJelly:
                return "Grape Jelly";
        }

        return "Raspberry Jam";
    }
}
