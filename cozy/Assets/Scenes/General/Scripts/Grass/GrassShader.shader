Shader "Custom/InstancedShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

        _Amplitude ("Amplitude", Float) = 1.0
        _Frequency ("Frequency", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Amplitude;
            float _Frequency;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                float scalar = v.uv.y;
                v.vertex.x += _Amplitude * cos((_Time.y / 2) * _Frequency) * scalar;
                v.vertex.z += _Amplitude * sin(_Time.y * _Frequency) * scalar;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                // i.uv.y -= .1;
                float4 col = tex2D(_MainTex, i.uv);
                float4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                if (col.a < 0.1)
                    discard;

                // return float4(i.uv.y, 0, 0, 1);
                return col * instanceColor;
            }
            ENDHLSL
        }
    }
}