using UnityEngine;

public class WaterInteractable : BaseInteractable
{
    [SerializeField] private ParticleSystem waterStream;

    public override void interact()
    {
        if (!waterStream.isStopped)
            waterStream.Stop();
        else 
            waterStream.Play();
    }

}