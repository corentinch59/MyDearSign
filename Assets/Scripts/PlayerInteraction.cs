using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _interactionRange;
    [SerializeField] private GameObject _prompt;
    [SerializeField] public Transform anchor;
    
    public void ShowPrompt(bool show)
    {
        _prompt.SetActive(show);
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interacting");

        if (!ctx.started) return;
        
        var interactables = FindObjectsOfType<IInteractable>();
        // Find closest that can be interacted
        IInteractable closest = null;
        float closestDistance = float.MaxValue;
        foreach (var interactable in interactables)
        {
            if (!interactable.CanInteract()) continue;
            
            var distance = Vector3.Distance(interactable.transform.position, transform.position);
            if (distance < closestDistance && distance <= _interactionRange)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }
        
        if (closest != null)
        {
            closest.Interact(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Show prompt if there is an interactable in range
        // fuck it I know it's bad code
        var interactables = FindObjectsOfType<IInteractable>();
        foreach (var interactable in interactables)
        {
            if (!interactable.CanInteract()) continue;
            
            if (Vector3.Distance(interactable.transform.position, transform.position) <= _interactionRange)
            {
                ShowPrompt(true);
                return;
            }
        }
        
        ShowPrompt(false);
    }

    public Transform GetInteractableTransformAnchor()
    {
        return anchor;
    }
}
