using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    public abstract void Interact(PlayerInteraction player);
    public abstract bool CanInteract();
}
