Shader "Custom/RaymarchShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "LightMode" = "UniversalForward" }
        
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            // #include "raymarch_sdf.hlsl"

            float sdSphere(float3 p, float3 sp, float s)
            {
                return length(p - sp) - s;
            }

            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            uniform float4x4 _CamToWorld;
            uniform float4x4 _InverseProjectionMatrix;
            uniform float4x4 _RaymarchArea;

            float4 _WorldSpaceLightPos0;

            TEXTURE2D(_MainTex);
            // SAMPLER(sampler_LinearClamp);


            float3 GetRayDirection(float2 uv)
            {
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipSpace = float4(ndc, 1.0, 1.0);
                float4 viewSpace = mul(_InverseProjectionMatrix, clipSpace); 
                viewSpace /= viewSpace.w;
                return normalize(viewSpace.xyz);
            }

            float3 getNormal(float3 p) {
                float e = 0.001;
                return normalize(float3(
                    sdSphere(p + float3(e, 0, 0), _RaymarchArea[0].xyz, _RaymarchArea[1].x) - sdSphere(p - float3(e, 0, 0), _RaymarchArea[0].xyz, _RaymarchArea[1].x),
                    sdSphere(p + float3(0, e, 0), _RaymarchArea[0].xyz, _RaymarchArea[1].x) - sdSphere(p - float3(0, e, 0), _RaymarchArea[0].xyz, _RaymarchArea[1].x),
                    sdSphere(p + float3(0, 0, e), _RaymarchArea[0].xyz, _RaymarchArea[1].x) - sdSphere(p - float3(0, 0, e), _RaymarchArea[0].xyz, _RaymarchArea[1].x)
                ));
            }

            half4 raymarch(float2 uv, int stepCount)
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv);
                float sceneDepth = SampleSceneDepth(uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, sceneDepth, UNITY_MATRIX_I_VP);
                float distanceDepth = length(worldPos - _WorldSpaceCameraPos.xyz);
                
                float3 directionCS = GetRayDirection(uv);
                float3 directionWS = mul(_CamToWorld, float4(directionCS.xyz, 0)).xyz;
                float3 originWS = _WorldSpaceCameraPos.xyz;
                
                float epsilon = .001;
                float dist = 0;
                for (int i = 0; i < stepCount; i++)
                {
                    float3 currentPos = originWS + directionWS * dist;
                    float distanceRay = length(currentPos - _WorldSpaceCameraPos.xyz);
                    float distToSphere = sdSphere(currentPos, _RaymarchArea[0].xyz, _RaymarchArea[1].x);
                    
                    if (distanceRay > distanceDepth)
                        break;
                    
                    if (distToSphere < epsilon)
                    {
                        float3 lightDir = _WorldSpaceLightPos0.xyz;
                        float3 normal = getNormal(currentPos);
                        float ambient = 0.1;
                        float diffuse = max(dot(normal, lightDir), 0.0);
                        float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - currentPos);
                        float3 reflection = reflect(-lightDir, normal);
                        float specular = pow(max(dot(reflection, viewDir), 0.0), 16.0);
                        float shade =  ambient + diffuse + specular;
                        half4 volumetricColor = half4(float3(shade, shade, shade) * _RaymarchArea[3].rgb, 1);
                        return volumetricColor;
                    }

                    dist += distToSphere;
                }

                return half4(color.rgb, 1);   
            }

            float3 GetWorldSpacePosition(float2 uv, float depth)
            {
                // Convert UV from [0,1] to NDC [-1,1]
                float2 screenUV = uv * 2.0 - 1.0;
                
                // Construct NDC position
                float4 ndcPos = float4(screenUV, depth, 1.0);
                
                // Convert directly to world space using inverse view-projection matrix
                float4 worldPos = mul(UNITY_MATRIX_I_VP, ndcPos);
                worldPos.xyz /= worldPos.w;
                
                return worldPos.xyz;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return half4(1, 0, 0, 1);
                return raymarch(input.uv, _RaymarchArea[2].y);

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, input.uv);
                float deviceDepth = SampleSceneDepth(input.uv);
                float3 worldPos = GetWorldSpacePosition(input.uv, deviceDepth);
                // float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
                // float shadow =s 1 - MainLightRealtimeShadow(shadowCoord);space
                float shadow = 1.0f;
                half4 shadowColor = half4(shadow, 0, 0, 1);


                return half4(lerp(color.rgb, shadowColor.rgb, .5), 1);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}