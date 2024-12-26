public class PrototypeInteractable : BaseInteractable
{
    public override void interact()
    {
        canInteract = !canInteract;
    }
}
