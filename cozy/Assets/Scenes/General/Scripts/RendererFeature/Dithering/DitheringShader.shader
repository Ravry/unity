Shader "Custom/DitheringShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            

            float GetBayerFromCoord(float2 uv)
            {
                float bayerMatrix[16] = {
                    0.0 / 16.0, 8.0 / 16.0, 2.0 / 16.0, 10.0/ 16.0,
                    12.0/ 16.0, 4.0 / 16.0, 14.0/ 16.0, 6.0 / 16.0,
                    3.0 / 16.0, 11.0/ 16.0, 1.0 / 16.0, 9.0 / 16.0,
                    15.0/ 16.0, 7.0 / 16.0, 13.0/ 16.0, 5.0 / 16.0
                };

                float2 screenCoord = uv * _ScreenParams.xy;
                int x = int(screenCoord.x) % 4;
                int y = int(screenCoord.y) % 4;
                return bayerMatrix[y * 4 + x];
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float3 base = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).rgb;
                float gray = dot(base, float3(0.299, 0.587, 0.114));
                float threshold = GetBayerFromCoord(uv);
                float dithered = gray > threshold ? 1 : 0;
                float3 ditheredCol = float3(dithered, dithered, dithered);
                return float4(ditheredCol * base, 1);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}