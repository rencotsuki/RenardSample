//This is based on a shader from https://alastaira.wordpress.com/2014/12/30/adding-shadows-to-a-unity-vertexfragment-shader-in-7-easy-steps/

Shader "Custom/MobileARShadow"
{
	SubShader
	{
		Pass
		{
		Tags
		{
			"LightMode" = "ForwardBase" "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True"
		}

		LOD 150
		Blend Zero SrcColor
		ZWrite On

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		#pragma multi_compile_fwdbase

		#include "AutoLight.cginc"


		struct v2f
		{
			float4 pos : SV_POSITION;

			LIGHTING_COORDS(0,1)
		};


		v2f vert(appdata_base v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			TRANSFER_VERTEX_TO_FRAGMENT(o);

			return o;
		}

		fixed4 frag(v2f i) : COLOR {
			float attenuation = LIGHT_ATTENUATION(i);
			return fixed4(1.0,1.0,1.0,1.0) * attenuation;
		}

		ENDCG
	}
	}

		Fallback "VertexLit"

}
