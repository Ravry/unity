using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material material;
    }

    public Settings settings;

    class RaymarchPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier _cameraColorTarget;
        private Settings _settings;
        private ProfilingSampler _profilingSampler;
        private int _temporaryRT;

        public RaymarchPass(Settings settings)
        {
            _settings = settings;
            _profilingSampler = new ProfilingSampler("Raymarch");
            _temporaryRT = Shader.PropertyToID("_TemporaryRT");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (_settings.material == null)
            {
                Debug.LogError("RaymarchPass: Material is missing!");
                return;
            }

            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var colorDesc = renderingData.cameraData.cameraTargetDescriptor;
            colorDesc.depthBufferBits = 0; // No depth buffer needed for this pass
            cmd.GetTemporaryRT(_temporaryRT, colorDesc);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_settings.material == null)
                return;


            RaymarchArea area = GameObject.FindObjectOfType<RaymarchArea>();
            
            if (area == null)
                return;

            _settings.material.SetMatrix("_CamToWorld", renderingData.cameraData.camera.cameraToWorldMatrix);
            _settings.material.SetMatrix("_InverseProjectionMatrix", renderingData.cameraData.camera.projectionMatrix.inverse);
            _settings.material.SetMatrix("_RaymarchArea", area.bound);
            _settings.material.SetFloat("_NoiseScale", area.noiseScale);
            CommandBuffer cmd = CommandBufferPool.Get("RaymarchPass");
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cmd.Blit(_cameraColorTarget, _temporaryRT);
                cmd.Blit(_temporaryRT, _cameraColorTarget, _settings.material);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_temporaryRT);
        }
    }

    RaymarchPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new RaymarchPass(settings);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material != null)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
