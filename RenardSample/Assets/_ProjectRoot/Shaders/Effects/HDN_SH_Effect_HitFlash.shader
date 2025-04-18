// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33177,y:32756,varname:node_3138,prsc:2|emission-2118-OUT,alpha-2313-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32511,y:32535,ptovrint:False,ptlb:Color_Base,ptin:_Color_Base,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4810,x:32511,y:32855,ptovrint:False,ptlb:Texture_Color,ptin:_Texture_Color,varname:node_4810,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:baaa0e1a27f856541a6fabb812e9555e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4276,x:32780,y:32721,varname:node_4276,prsc:2|A-7241-RGB,B-4810-B;n:type:ShaderForge.SFN_Color,id:9898,x:32511,y:32695,ptovrint:False,ptlb:Color_Flash,ptin:_Color_Flash,varname:node_9898,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6714,x:32780,y:32840,varname:node_6714,prsc:2|A-9898-RGB,B-4810-R,C-1887-OUT,D-2810-A;n:type:ShaderForge.SFN_Add,id:2118,x:32969,y:32840,varname:node_2118,prsc:2|A-4276-OUT,B-6714-OUT;n:type:ShaderForge.SFN_Multiply,id:2313,x:32796,y:33089,varname:node_2313,prsc:2|A-4810-B,B-9148-OUT;n:type:ShaderForge.SFN_Vector1,id:9148,x:32796,y:33208,varname:node_9148,prsc:2,v1:0.2;n:type:ShaderForge.SFN_VertexColor,id:2810,x:32511,y:33024,varname:node_2810,prsc:2;n:type:ShaderForge.SFN_Vector1,id:1887,x:32780,y:32964,varname:node_1887,prsc:2,v1:5;proporder:7241-4810-9898;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_Effect_HitFlash" {
    Properties {
        _Color_Base ("Color_Base", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Color ("Texture_Color", 2D) = "white" {}
        _Color_Flash ("Color_Flash", Color) = (0.5,0.5,0.5,1)
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
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color_Base;
            uniform sampler2D _Texture_Color; uniform float4 _Texture_Color_ST;
            uniform float4 _Color_Flash;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Texture_Color_var = tex2D(_Texture_Color,TRANSFORM_TEX(i.uv0, _Texture_Color));
                float3 emissive = ((_Color_Base.rgb*_Texture_Color_var.b)+(_Color_Flash.rgb*_Texture_Color_var.r*5.0*i.vertexColor.a));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Texture_Color_var.b*0.2));
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
            #endif //UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
