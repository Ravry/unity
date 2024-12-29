Shader "Custom/SSRShader"
{
    Properties
    {
        _StepCount ("Raymarch Step Count", Range(16, 256)) = 64
        _StepSize ("Raymarch Step Size", Range(0.01, 1)) = 0.1
        _DepthThreshold ("Depth Threshold", Range(0.001, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        
        Pass {
            Name "Reflection Pass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            
            float4 TransformViewSpaceToScreenSpace(float3 posVS)
            {
                float4 currentPosUV = mul(UNITY_MATRIX_P, float4(posVS, 1.0));
                currentPosUV.xyz /= currentPosUV.w;
                currentPosUV.xy = (currentPosUV.xy + 1.0) * 0.5;
                return currentPosUV;
            }

            float3 TransformWorldToViewSpace(float3 posWS)
            {
                float3 posVS = mul(UNITY_MATRIX_V, float4(posWS, 0.0));
                posVS.y *= -1;
                return posVS;
            }
            
            bool IsInScreen(float2 uv)
            {
                return uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1;
            }



            float _StepSize;
            int _StepCount;
            float _DepthThreshold;

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                // Sample scene color, depth, and normals
                float3 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).rgb;
                float sceneDepth = SampleSceneDepth(uv);
                float3 normalWS = normalize(SampleSceneNormals(uv));

                // If depth is invalid, return the original scene color
                if (sceneDepth <= 0)
                {
                    return float4(sceneColor, 1);
                }

                // Compute world-space position of the fragment
                float3 fragmentPosWS = ComputeWorldSpacePosition(uv, sceneDepth, UNITY_MATRIX_I_VP);

                // Calculate view direction and reflection direction in world space
                float3 viewDirWS = normalize(fragmentPosWS - _WorldSpaceCameraPos.xyz);
                float3 reflectionDirWS = reflect(viewDirWS, normalWS);

                // Initialize ray marching variables
                float3 currentPosWS = fragmentPosWS;
                bool hit = false;
                float3 hitColor = float3(0, 0, 0);
                
                [unroll(100)]
                for (int i = 0; i < _StepCount; i++)    
                {
                    currentPosWS += reflectionDirWS * _StepSize;

                    // Transform to screen space
                    float4 currentPosHCS = mul(UNITY_MATRIX_VP, float4(currentPosWS, 1));
                    currentPosHCS /= currentPosHCS.w;

                    // Convert to UV coordinates
                    float2 currentPosUV = (currentPosHCS.xy * 0.5) + 0.5;

                    // Check if the position is on screen
                    if (currentPosUV.x < 0 || currentPosUV.x > 1 || currentPosUV.y < 0 || currentPosUV.y > 1)
                    {
                        break;
                    }

                    // Sample depth at the current UV
                    float sampledDepth = SampleSceneDepth(currentPosUV);
                    float currentDepth = currentPosHCS.z;

                    // Check depth difference to determine a hit
                    if (currentDepth > sampledDepth && abs(currentDepth - sampledDepth) < _DepthThreshold)
                    {
                        hit = true;
                        hitColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, currentPosUV).rgb;
                        break;
                    }
                }

                // Blend reflection color with the original scene color
                if (hit)
                {
                    return float4(hitColor, 1);
                }
                else
                {
                    return float4(sceneColor, 1);
                }
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}