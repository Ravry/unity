Shader "Custom/RaymarchShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        
        Pass {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            uniform float4x4 _InverseProjectionMatrix;
            uniform float4x4 _CamToWorld;
            uniform float4x4 _RaymarchArea;

            float4 _WorldSpaceLightPos0;

            float sdSphere(float3 p, float3 sp, float s)
            {
                return length(p - sp) - s;
            }

            float3 GetSphereNormal(float3 p, float3 sp, float sr) {
                float e = 0.001;
                return normalize(float3(
                    sdSphere(p + float3(e, 0, 0), sp, sr) - sdSphere(p - float3(e, 0, 0), sp, sr),
                    sdSphere(p + float3(0, e, 0), sp, sr) - sdSphere(p - float3(0, e, 0), sp, sr),
                    sdSphere(p + float3(0, 0, e), sp, sr) - sdSphere(p - float3(0, 0, e), sp, sr)
                ));
            }

            float3 GetRayDirection(float2 uv)
            {
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipSpace = float4(ndc, 1.0, 1.0);
                float4 viewSpace = mul(_InverseProjectionMatrix, clipSpace); 
                viewSpace /= viewSpace.w;
                return normalize(viewSpace.xyz);
            }


            float4 frag(Varyings input) : SV_Target
            {
                float3 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float sceneDepth = SampleSceneDepth(input.texcoord);

                float3 worldPos = ComputeWorldSpacePosition(input.texcoord, sceneDepth, UNITY_MATRIX_I_VP);
                float distanceDepth = length(worldPos - _WorldSpaceCameraPos.xyz);

                float3 directionCS = GetRayDirection(input.texcoord);
                float3 directionWS = mul(_CamToWorld, float4(directionCS.xyz, 0)).xyz;
                float3 originWS = _WorldSpaceCameraPos.xyz;

                float3 spherePos = _RaymarchArea[0].xyz;
                float sphereRadius = _RaymarchArea[1].x;
                float3 sphereColor = _RaymarchArea[3].rgb;

                int stepCount = 100;
                float epsilon = .001;
                float dist = 0;
                for (int i = 0; i < stepCount; i++)
                {
                    float3 currentPos = originWS + directionWS * dist;
                    float distanceRay = length(currentPos - _WorldSpaceCameraPos.xyz);
                    float distToSphere = sdSphere(currentPos, spherePos, sphereRadius);
                 
                    if (distanceRay > distanceDepth)
                        break;

                    if (distToSphere < epsilon)
                    {
                        float3 lightDir = _WorldSpaceLightPos0.xyz;
                        float3 normal = GetSphereNormal(currentPos, spherePos, sphereRadius);
                        float ambient = 0.1;
                        float diffuse = max(dot(normal, lightDir), 0.0);
                        float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - currentPos);
                        float3 reflection = reflect(-lightDir, normal);
                        float specular = pow(max(dot(reflection, viewDir), 0.0), 16.0);
                        float shade =  ambient + diffuse + specular;
                        half4 volumetricColor = half4(float3(shade, shade, shade) * sphereColor, 1);
                        return volumetricColor;
                    }
                    dist += distToSphere;
                }

                // return float4(sceneColor.rgb, 1);
                return float4(lerp(sceneColor.rgb, float3(sceneDepth, sceneDepth, sceneDepth), 1), 1);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}