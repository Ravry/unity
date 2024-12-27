using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class RaymarchFeature : ScriptableRendererFeature
{
    
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material material;
    }

    public Settings settings;

    class RaymarchRenderPass : ScriptableRenderPass
    {
        private Settings sett;
        public RaymarchRenderPass(Settings settings)
        {
            sett = settings;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            RaymarchArea area = GameObject.FindFirstObjectByType<RaymarchArea>();
            
            if (area == null)
                return;

            var cameraData = frameData.Get<UniversalCameraData>();

            sett.material.SetMatrix("_CamToWorld", cameraData.camera.cameraToWorldMatrix);
            sett.material.SetMatrix("_InverseProjectionMatrix", cameraData.camera.projectionMatrix.inverse);
            sett.material.SetMatrix("_RaymarchArea", area.bound);

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

    RaymarchRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new RaymarchRenderPass(settings);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}
