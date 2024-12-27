using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using Unity.VisualScripting;

public class DitheringFeature : ScriptableRendererFeature
{
    
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material material;
    }

    public Settings settings;

    class EdgeDetectionPass : ScriptableRenderPass
    {
        private Settings sett;
        public EdgeDetectionPass(Settings settings)
        {
            sett = settings;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            var customEffect = stack.GetComponent<DitheringVolumeComponent>();
            customEffect.IsSceneBound();
            if (!customEffect.IsActive() || customEffect.IsUnityNull())
                return;

            var resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError("can't use intermediate texture!");
                return;
            }

            var source = resourceData.activeColorTexture;
            
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{passName}";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
            
            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, sett.material, 0);
            
            renderGraph.AddBlitPass(para, passName: passName);
            
            resourceData.cameraColor = destination;
        }
    }

    EdgeDetectionPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new EdgeDetectionPass(settings);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}