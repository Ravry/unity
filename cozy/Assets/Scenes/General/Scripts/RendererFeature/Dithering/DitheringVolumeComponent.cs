using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("Custom/DitheringVolumeComponent")]
public class DitheringVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public BoolParameter _active = new BoolParameter(true);
    
    public bool IsActive() {
        return _active.value && active;
    }
}
