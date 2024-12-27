using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("Custom/EdgeDetectionVolumeComponent")]
public class EdgeDetectionVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public BoolParameter _active = new BoolParameter(true);
    public ColorParameter outlineColor = new ColorParameter(Color.black);
    public FloatParameter outlineThickness = new FloatParameter(1.0f);
    public ColorParameter secondaryColor = new ColorParameter(Color.white);
    public BoolParameter useSceneColor = new BoolParameter(false);
    
    public bool IsActive() {
        return _active.value && active;
    }
}
