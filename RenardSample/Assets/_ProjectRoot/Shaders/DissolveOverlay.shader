Shader "Custom/DissolveOverlay"
{
    Properties
    {
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend One One  // 加算合成
        ZWrite Off     // Zバッファ書き込みを無効化
        Cull Back      // 背面カリング（通常）

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _DissolveTex;
            float _DissolveAmount;
            fixed4 _EdgeColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dissolveValue = tex2D(_DissolveTex, i.uv).r;
                float threshold = _DissolveAmount;

                // 境界部分のエフェクト
                float edge = smoothstep(threshold - 0.05, threshold, dissolveValue);
                return edge * _EdgeColor;
            }
            ENDCG
        }
    }
}
