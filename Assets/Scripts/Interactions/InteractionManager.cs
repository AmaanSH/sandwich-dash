using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InteractionManager : MonoBehaviour
{
    private List<Interaction> interactions = new List<Interaction>();
    private Camera mainCamera;
    public Interaction ActiveInteraction { get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Interaction>(out Interaction interaction))
        {
            interactions.Add(interaction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Interaction>(out Interaction interaction))
        {
            RemoveInteraction(interaction);
        }
    }

    public bool SelectInteraction()
    {
        if (interactions.Count == 0) { return false; }

        Interaction closestTarget = null;
        float closestTargetDistance = Mathf.Infinity;

        foreach(Interaction interaction in interactions)
        {
            // get the viewport position of the item
            Vector2 viewportPos = mainCamera.ScreenToViewportPoint(interaction.transform.position);

            // check if its visible on screen
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.x > 1)
            {
                continue;
            }

            // check if camera has target in view
            Vector2 toCentre = viewportPos - new Vector2(0.5f, 0.5f);
            if (toCentre.sqrMagnitude < closestTargetDistance)
            {
                closestTarget = interaction;
                closestTargetDistance = toCentre.sqrMagnitude;
            }
        }

        if (closestTarget == null) { return false; }

        ActiveInteraction = closestTarget;

        return true;
    }

    public void Cancel()
    {
        if (ActiveInteraction)
        {
            ActiveInteraction.Exit();
            ActiveInteraction = null;
        }
    }

    public void RemoveInteraction(Interaction interaction)
    {
        if (ActiveInteraction == interaction)
        {
            Cancel();
        }

        interactions.Remove(interaction);
    }
}
