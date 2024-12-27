//credits: https://ameye.dev/notes/edge-detection-outlines/
Shader "Custom/RaymarchShader"
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

            uniform float4 _OutlineColor;
            uniform float _OutlineThickness;
            uniform float4 _SecondaryColor;
            uniform int _UseSceneColor;

            // Edge detection kernel that works by taking the sum of the squares of the differences between diagonally adjacent pixels (Roberts Cross).
            float RobertsCross(float3 samples[4])
            {
                const float3 difference_1 = samples[1] - samples[2];
                const float3 difference_2 = samples[0] - samples[3];
                return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
            }

            // The same kernel logic as above, but for a single-value instead of a vector3.
            float RobertsCross(float samples[4])
            {
                const float difference_1 = samples[1] - samples[2];
                const float difference_2 = samples[0] - samples[3];
                return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
            }
            
            // Helper function to sample scene normals remapped from [-1, 1] range to [0, 1].
            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            // Helper function to sample scene luminance.
            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).rgb;
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
            
                const float half_width_f = floor(_OutlineThickness * 0.5);
                const float half_width_c = ceil(_OutlineThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);
                uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);
                uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1);
                uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);
                                
                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];

                for (int i = 0; i < 4; i++) {
                    depth_samples[i] = SampleSceneDepth(uvs[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    luminance_samples[i] = SampleSceneLuminance(uvs[i]);
                }

                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);

                float depth_threshold = 1 / 200.0f;
                edge_depth = edge_depth > depth_threshold ? 1 : 0;
                
                float normal_threshold = 1 / 4.0f;
                edge_normal = edge_normal > normal_threshold ? 1 : 0;
                
                float luminance_threshold = 1 / 0.5f;
                edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;

                float edge = max(edge_depth, max(edge_normal, edge_luminance));

                float3 secondaryColor;
                if (_UseSceneColor == 1)
                    secondaryColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).rgb;
                else
                    secondaryColor = _SecondaryColor.rgb;
                return float4(lerp(secondaryColor, _OutlineColor.rgb, edge), 1);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}