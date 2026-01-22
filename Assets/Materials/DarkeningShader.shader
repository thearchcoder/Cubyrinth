Shader "Custom/WorldSpaceBottomDarken"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.392, 0.208, 0, 1)
        _DarkenAmount ("Darken Amount", Range(0,40)) = 0.2
        _FadeHeight ("Fade Height", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            fixed4 _BaseColor;
            float _DarkenAmount;
            float _FadeHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // calculate gradient based on world Y
                float shade = saturate((i.worldPos.y + 0.5) / _FadeHeight);
                shade = lerp(1 - _DarkenAmount, 1, shade);

                fixed4 col = fixed4(_BaseColor.rgb * shade * 1.3, _BaseColor.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
