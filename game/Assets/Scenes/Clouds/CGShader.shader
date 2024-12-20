Shader "Custom/CGShader"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD;
            };

            v2f vert(appdata v)
            {   
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 _CamToWorld;
            uniform float4x4 _InverseProjectionMatrix;

            float sdSphere(float3 p, float s)
            {
                return length(p)-s;
            }
            
            float3 GetRayDirection(float2 uv)
            {
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipSpace = float4(ndc, 1.0, 1.0);
                float4 viewSpace = mul(_InverseProjectionMatrix, clipSpace); 
                viewSpace /= viewSpace.w;
                return normalize(viewSpace.xyz);
            }

            fixed4 raymarch(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
                float3 rayDirWS = mul((float3x3)_CamToWorld, GetRayDirection(uv));
                float3 rayOriginWS = _WorldSpaceCameraPos.xyz;

                int stepCount = 100;
                float stepSize = .1f;
                float epsilon = .01f;


                float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

                float dist = 0;
                for (int i = 0; i < stepCount; i++)
                {
                    float3 currentPos = rayOriginWS + rayDirWS * dist;
                    float distToSphere = sdSphere(currentPos, 1);

                    if (distToSphere < epsilon)
                    {  
                        // float3 worldPos = ComputeWorldSpacePosition(uv, sceneDepth, _InverseProjectionMatrix);
                        // float distance = length(worldPos - _WorldSpaceCameraPos.xyz);
                        // float distance2 = length(currentPos - _WorldSpaceCameraPos.xyz);
                        // if (distance2 >= distance)
                        //     break;
                        return fixed4(1, 1, 1, 1);
                    }
                    
                    dist += distToSphere;
                }

                return fixed4(sceneDepth, sceneDepth, sceneDepth, 1);
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 raymarchColor = raymarch(i.uv);
                return raymarchColor; 
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
