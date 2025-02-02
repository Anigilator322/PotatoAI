Shader "Custom/CapsuleCut"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0,0,0,1)
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _CapsuleCount ("Capsule Count", int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _FogColor;
            int _CapsuleCount;
            float2 _MapMin;
            float2 _MapMax;

            #define MAX_CAPSULES 10

            float4 _Capsules[MAX_CAPSULES];
            float _CapsuleRadius[MAX_CAPSULES];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; // используем дл€ координат по карте
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // ѕредполагаетс€, что вершины уже заданы в мировых координатах.
                float2 worldPos = v.vertex.xy;
                o.uv = (worldPos - _MapMin) / (_MapMax - _MapMin);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float segmentDistance(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / dot(ba, ba));
                return length(pa - ba * h);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = i.uv;
                float alpha = 1.0;

                for (int j = 0; j < _CapsuleCount; j++)
                {
                    float2 a = _Capsules[j].xy;
                    float2 b = _Capsules[j].zw;
                    float d = segmentDistance(pos, a, b);
                    if (d < _CapsuleRadius[j])
                    {
                        alpha = 0.0;
                        break;
                    }
                }

                fixed4 col = _FogColor;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
