using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    public bool canInteract;
    public abstract void interact();
}
