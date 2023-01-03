using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public Material material;
    public SpriteRenderer radial;

    private float max;

    public void Setup(float max)
    {
        Material copied = Instantiate(material, transform);
        radial.material = copied;

        this.max = max;
    }

    public void SetFill(float amount)
    {
        radial.material.SetFloat("_Arc2", amount);
        radial.color = Color.Lerp(Color.green, Color.red, amount / 360);
    }
}
