Shader "Custom/RaymarchShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "raymarch_sdf.hlsl"
            #include "noise.hlsl"

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
            uniform float _NoiseScale;

            float4 _WorldSpaceLightPos0;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_LinearClamp);

            float3 GetRayDirection(float2 uv)
            {
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipSpace = float4(ndc, 1.0, 1.0);
                float4 viewSpace = mul(_InverseProjectionMatrix, clipSpace); 
                viewSpace /= viewSpace.w;
                return normalize(viewSpace.xyz);
            }

            half4 raymarchVolume(float3 pos, float3 rayDir, int stepCount, float stepSize, float3 sceneColor)
            {
                float accumulatedDensity = 0;
                float dist = 0;            
                pos += rayDir * .1;

                for (int i = 0; i < stepCount; i++)
                {
                    float3 currentPos = pos + rayDir * dist;

                    float distToBox = sdBox(currentPos, _RaymarchArea[0].xyz, _RaymarchArea[1].xyz);
                    if (distToBox > 0) break;

                    float density = 0;
                    float amplitude = 1.0;
                    float frequency = _NoiseScale;

                    for(int oct = 0; oct < 1; oct++) {
                        density += noise(currentPos * frequency + _Time.y) * amplitude;
                        amplitude *= 0.5;
                        frequency *= 2.0;
                    }

                    density = max(0, density);
                    accumulatedDensity += density * stepSize;
                    dist += stepSize;
                }
            
                float ABSORPTION = .1f;
                float transmittance = exp(-accumulatedDensity * ABSORPTION);
                transmittance = smoothstep(0, 1, transmittance);
                return half4(lerp(_RaymarchArea[3].rgb, sceneColor, transmittance), 1);
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
                    float distToSphere = sdBox(currentPos, _RaymarchArea[0].xyz, _RaymarchArea[1].xyz);
                    
                    if (distanceRay > distanceDepth)
                        break;

                    
                    if (distToSphere < epsilon)
                    {
                        half4 volumetricColor = raymarchVolume(currentPos, directionWS, stepCount, _RaymarchArea[2].x, color.rgb);
                        return volumetricColor;
                    }

                    dist += distToSphere;
                }

                return half4(color.rgb, 1);   
            }

            half4 frag(Varyings input) : SV_Target
            {
                return raymarch(input.uv, _RaymarchArea[2].y);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}