// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32818,y:32710,varname:node_3138,prsc:2|emission-7216-OUT;n:type:ShaderForge.SFN_NormalVector,id:5403,x:31465,y:32756,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:2738,x:31637,y:32756,varname:node_2738,prsc:2,tffrom:0,tfto:3|IN-5403-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6877,x:31982,y:32756,varname:node_6877,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-110-OUT;n:type:ShaderForge.SFN_RemapRange,id:110,x:31813,y:32756,varname:node_110,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-2738-XYZ;n:type:ShaderForge.SFN_Tex2d,id:2594,x:32157,y:32756,ptovrint:False,ptlb:Texture_Ref,ptin:_Texture_Ref,varname:node_2594,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a0cdd8aa61b32294eb19031c5423dbb9,ntxv:0,isnm:False|UVIN-6877-OUT;n:type:ShaderForge.SFN_Tex2d,id:3028,x:32257,y:32975,ptovrint:False,ptlb:Texture_Mask,ptin:_Texture_Mask,varname:node_3028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e00dd749a507aa644a0a8b6a63dd134f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7216,x:32571,y:32796,varname:node_7216,prsc:2|A-7126-OUT,B-3028-R;n:type:ShaderForge.SFN_VertexColor,id:7665,x:32199,y:32254,varname:node_7665,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3313,x:32378,y:32569,varname:node_3313,prsc:2|A-4800-RGB,B-2594-B;n:type:ShaderForge.SFN_Add,id:7126,x:32605,y:32485,varname:node_7126,prsc:2|A-7665-RGB,B-3313-OUT;n:type:ShaderForge.SFN_Color,id:4800,x:32096,y:32465,ptovrint:False,ptlb:Col_Bright,ptin:_Col_Bright,varname:node_4800,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.047,c2:0.597,c3:0.253,c4:1;proporder:2594-3028-4800;pass:END;sub:END;*/

Shader "Shader Forge/HD_SH_UI_HD_ST_UI_Gyroscope_Alpha" {
    Properties {
        _Texture_Ref ("Texture_Ref", 2D) = "white" {}
        _Texture_Mask ("Texture_Mask", 2D) = "white" {}
        _Col_Bright ("Col_Bright", Color) = (0.047,0.597,0.253,1)
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
            Blend One One
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
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform sampler2D _Texture_Ref; uniform float4 _Texture_Ref_ST;
            uniform sampler2D _Texture_Mask; uniform float4 _Texture_Mask_ST;
            uniform float4 _Col_Bright;
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
                float2 node_6877 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb*0.5+0.5).rg;
                float4 _Texture_Ref_var = tex2D(_Texture_Ref,TRANSFORM_TEX(node_6877, _Texture_Ref));
                float4 _Texture_Mask_var = tex2D(_Texture_Mask,TRANSFORM_TEX(i.uv0, _Texture_Mask));
                float3 emissive = ((i.vertexColor.rgb+(_Col_Bright.rgb*_Texture_Ref_var.b))*_Texture_Mask_var.r);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
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
