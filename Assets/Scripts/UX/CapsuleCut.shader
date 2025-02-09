Shader "Custom/CapsuleCut"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0,0,0,1)
        _MainTex ("Sprite Texture", 2D) = "white" {}
        // _CapsuleCount задаётся из кода
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            // Требуется поддержка минимум shader model 4.5 для StructuredBuffer
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _FogColor;
            int _CapsuleCount;

            // Структура данных, должна соответствовать структуре из C#
            struct CapsuleData {
                float4 points; // xy: start, zw: end (в нормализованных координатах)
                float4 extra;  // x: нормализованный радиус
            };

            StructuredBuffer<CapsuleData> capsuleBuffer;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                // Если ваш объект тумана покрывает всю карту и имеет корректные uv,
                // их можно использовать напрямую
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Функция для вычисления расстояния от точки до отрезка
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
                // Перебираем все капсулы из StructuredBuffer
                for (int j = 0; j < _CapsuleCount; j++)
                {
                    CapsuleData cd = capsuleBuffer[j];
                    float2 a = cd.points.xy;
                    float2 b = cd.points.zw;
                    float radius = cd.extra.x;
                    if (segmentDistance(pos, a, b) < radius)
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
