using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class CustomFeature : ScriptableRendererFeature
{
    
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material material;
    }

    public Settings settings;

    class CustomRenderPass : ScriptableRenderPass
    {
        private Settings sett;
        public CustomRenderPass(Settings settings)
        {
            sett = settings;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var _passName = "Custom";

            sett.material.SetMatrix("_CamToWorld", Camera.main.cameraToWorldMatrix);
            sett.material.SetMatrix("_InverseProjectionMatrix", Camera.main.projectionMatrix.inverse);

            var resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError("can't use intermediate texture!");
                return;
            }

            var source = resourceData.activeColorTexture;
            
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{_passName}";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
            
            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, sett.material, 0);
            
            renderGraph.AddBlitPass(para, passName: _passName);
            
            resourceData.cameraColor = destination;
            Debug.Log("blit");
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}
