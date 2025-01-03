using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    public float normalFOV = 90.0f;
    public float aimedFOV = 70.0f;
    
    struct CineShake {
        public float startTime;
        public float shakeTimer;
        public float shakeIntensity;
    }
    
    private CineShake shakeParams;

    public void CameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cineNoise.AmplitudeGain = intensity;
        shakeParams.shakeTimer = time;
        shakeParams.startTime = time;
        shakeParams.shakeIntensity = intensity;
    }

    public void HandleCameraShake() {
        if (shakeParams.shakeTimer > 0)
        {
            CinemachineBasicMultiChannelPerlin cineNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            shakeParams.shakeTimer -= Time.deltaTime;
            cineNoise.AmplitudeGain = Mathf.Lerp(0, shakeParams.shakeIntensity, shakeParams.shakeTimer / shakeParams.startTime);
        }
    }

}
