using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InteractionManager : MonoBehaviour
{
    public PlayerStateMachine playerStateMachine;
    public List<Interaction> interactions = new List<Interaction>();
    private Camera mainCamera;
    
    public Interaction ActiveInteraction { get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interaction interaction))
        {
            interaction.Init(playerStateMachine);

            if (interaction.CanInteract())
            {
                interactions.Add(interaction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interaction interaction))
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
            float distance = Vector3.Distance(transform.position, interaction.transform.position);
            if (distance < closestTargetDistance)
            {
                closestTarget = interaction;
                closestTargetDistance = distance;
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
