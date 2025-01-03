using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public ParticleSystem muzzleParticleSystem;
    public Transform muzzleTransform;
    public float distance;
    public float shootDelay;
    public float aimDuration;
    public float shootRecoil;

    public abstract void StartWeapon();
    public abstract void UpdateWeapon();


    void Start()
    {
        StartWeapon();
    }

    void Update()
    {
        UpdateWeapon();
    }
}
