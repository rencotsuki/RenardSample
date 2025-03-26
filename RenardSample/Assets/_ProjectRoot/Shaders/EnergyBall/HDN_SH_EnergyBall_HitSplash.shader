// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33443,y:32742,varname:node_3138,prsc:2|emission-1548-OUT,alpha-5925-OUT,voffset-4482-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32562,y:32388,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3607,x:31509,y:32727,ptovrint:False,ptlb:Texture_Color,ptin:_Texture_Color,varname:node_3607,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:baaa0e1a27f856541a6fabb812e9555e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1548,x:33078,y:32588,varname:node_1548,prsc:2|A-7241-RGB,B-824-OUT;n:type:ShaderForge.SFN_Multiply,id:5925,x:33223,y:32868,varname:node_5925,prsc:2|A-1866-OUT,B-274-OUT;n:type:ShaderForge.SFN_Vector1,id:274,x:33223,y:32995,varname:node_274,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Clamp01,id:1866,x:33073,y:32868,varname:node_1866,prsc:2|IN-824-OUT;n:type:ShaderForge.SFN_VertexColor,id:5033,x:31957,y:33330,varname:node_5033,prsc:2;n:type:ShaderForge.SFN_Subtract,id:8085,x:31982,y:32635,varname:node_8085,prsc:2|A-3607-G,B-5033-A;n:type:ShaderForge.SFN_NormalVector,id:3161,x:31957,y:33183,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:196,x:32400,y:33057,varname:node_196,prsc:2|A-3161-OUT,B-6046-OUT,C-5033-R,D-3607-B;n:type:ShaderForge.SFN_OneMinus,id:3211,x:32276,y:33464,varname:node_3211,prsc:2|IN-3607-B;n:type:ShaderForge.SFN_Multiply,id:9208,x:32452,y:33322,varname:node_9208,prsc:2|A-3161-OUT,B-6046-OUT,C-5033-G,D-3211-OUT;n:type:ShaderForge.SFN_Add,id:4482,x:32827,y:33176,varname:node_4482,prsc:2|A-196-OUT,B-9208-OUT;n:type:ShaderForge.SFN_Vector1,id:6046,x:32452,y:33212,varname:node_6046,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Fresnel,id:7165,x:32563,y:32948,varname:node_7165,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2937,x:32735,y:32948,varname:node_2937,prsc:2,frmn:0,frmx:1,tomn:3,tomx:0|IN-7165-OUT;n:type:ShaderForge.SFN_Clamp01,id:3263,x:32923,y:32989,varname:node_3263,prsc:2|IN-2937-OUT;n:type:ShaderForge.SFN_Clamp01,id:824,x:32541,y:32635,varname:node_824,prsc:2|IN-2040-OUT;n:type:ShaderForge.SFN_Multiply,id:2040,x:32325,y:32635,varname:node_2040,prsc:2|A-8085-OUT,B-9621-OUT;n:type:ShaderForge.SFN_Vector1,id:9621,x:32310,y:32752,varname:node_9621,prsc:2,v1:2;proporder:7241-3607;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_HitSplash" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Color ("Texture_Color", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif
            
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            //#pragma only_renderers d3d9 d3d11 glcore metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Texture_Color; uniform float4 _Texture_Color_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float node_6046 = 1.5;
                float4 _Texture_Color_var = tex2Dlod(_Texture_Color,float4(TRANSFORM_TEX(o.uv0, _Texture_Color),0.0,0));
                v.vertex.xyz += ((v.normal*node_6046*o.vertexColor.r*_Texture_Color_var.b)+(v.normal*node_6046*o.vertexColor.g*(1.0 - _Texture_Color_var.b)));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Texture_Color_var = tex2D(_Texture_Color,TRANSFORM_TEX(i.uv0, _Texture_Color));
                float node_824 = saturate(((_Texture_Color_var.g-i.vertexColor.a)*2.0));
                float3 emissive = (_Color.rgb*node_824);
                float3 finalColor = emissive;
                return fixed4(finalColor,(saturate(node_824)*0.3));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #ifndef UNITY_PASS_SHADOWCASTER
            #define UNITY_PASS_SHADOWCASTER
            #endif
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            //#pragma only_renderers d3d9 d3d11 glcore metal 
            #pragma target 3.0
            uniform sampler2D _Texture_Color; uniform float4 _Texture_Color_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float node_6046 = 1.5;
                float4 _Texture_Color_var = tex2Dlod(_Texture_Color,float4(TRANSFORM_TEX(o.uv0, _Texture_Color),0.0,0));
                v.vertex.xyz += ((v.normal*node_6046*o.vertexColor.r*_Texture_Color_var.b)+(v.normal*node_6046*o.vertexColor.g*(1.0 - _Texture_Color_var.b)));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
