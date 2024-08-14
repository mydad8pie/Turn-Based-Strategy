Shader "Custom/HighlightShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,0.5)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _HighlightColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                col = lerp(col, _HighlightColor, _HighlightColor.a); // Apply highlight color
                return col;
            }
            ENDCG
        }
    }
}
