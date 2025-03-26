// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33041,y:32728,varname:node_3138,prsc:2|emission-9626-OUT;n:type:ShaderForge.SFN_NormalVector,id:8260,x:31459,y:32858,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:5762,x:31630,y:32858,varname:node_5762,prsc:2,tffrom:0,tfto:3|IN-8260-OUT;n:type:ShaderForge.SFN_RemapRange,id:1659,x:31800,y:32858,varname:node_1659,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-5762-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:3337,x:31970,y:32858,varname:node_3337,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1659-OUT;n:type:ShaderForge.SFN_Tex2d,id:5016,x:32154,y:32858,ptovrint:False,ptlb:Texture_Ref,ptin:_Texture_Ref,varname:node_5016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a0cdd8aa61b32294eb19031c5423dbb9,ntxv:0,isnm:False|UVIN-3337-OUT;n:type:ShaderForge.SFN_Add,id:870,x:32707,y:32856,varname:node_870,prsc:2|A-8077-OUT,B-3344-OUT;n:type:ShaderForge.SFN_Vector1,id:3344,x:32543,y:33064,varname:node_3344,prsc:2,v1:1;n:type:ShaderForge.SFN_VertexColor,id:4609,x:31459,y:32702,varname:node_4609,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9763,x:32509,y:32633,varname:node_9763,prsc:2|A-3498-OUT,B-870-OUT;n:type:ShaderForge.SFN_Add,id:9626,x:32458,y:32436,varname:node_9626,prsc:2|A-9763-OUT,B-2137-OUT;n:type:ShaderForge.SFN_Multiply,id:2137,x:32208,y:32391,varname:node_2137,prsc:2|A-4742-RGB,B-5016-G;n:type:ShaderForge.SFN_Color,id:4742,x:31864,y:32389,ptovrint:False,ptlb:Col_Ref,ptin:_Col_Ref,varname:node_4742,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.153,c2:0.309,c3:0.359,c4:1;n:type:ShaderForge.SFN_Multiply,id:8077,x:32391,y:32931,varname:node_8077,prsc:2|A-5016-R,B-4508-OUT;n:type:ShaderForge.SFN_Vector1,id:4508,x:32342,y:33064,varname:node_4508,prsc:2,v1:5;n:type:ShaderForge.SFN_Color,id:9043,x:31864,y:32555,ptovrint:False,ptlb:Col_Change,ptin:_Col_Change,varname:node_9043,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.05147058,c3:0.05147058,c4:1;n:type:ShaderForge.SFN_Lerp,id:3498,x:32178,y:32616,varname:node_3498,prsc:2|A-9043-RGB,B-4609-RGB,T-505-OUT;n:type:ShaderForge.SFN_Slider,id:2021,x:31347,y:32620,ptovrint:False,ptlb:Slider_ColorChange,ptin:_Slider_ColorChange,varname:node_2021,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:4417,x:31701,y:32707,varname:node_4417,prsc:2|A-2021-OUT,B-4609-A;n:type:ShaderForge.SFN_Clamp01,id:505,x:31864,y:32707,varname:node_505,prsc:2|IN-4417-OUT;proporder:5016-4742-9043-2021;pass:END;sub:END;*/

Shader "Shader Forge/HD_SH_UI_HD_ST_UI_Gyroscope" {
    Properties {
        _Texture_Ref ("Texture_Ref", 2D) = "white" {}
        _Col_Ref ("Col_Ref", Color) = (0.153,0.309,0.359,1)
        _Col_Change ("Col_Change", Color) = (1,0.05147058,0.05147058,1)
        _Slider_ColorChange ("Slider_ColorChange", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles metal 
            #pragma target 3.0
            uniform sampler2D _Texture_Ref; uniform float4 _Texture_Ref_ST;
            uniform float4 _Col_Ref;
            uniform float4 _Col_Change;
            uniform float _Slider_ColorChange;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float2 node_3337 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb*0.5+0.5).rg;
                float4 _Texture_Ref_var = tex2D(_Texture_Ref,TRANSFORM_TEX(node_3337, _Texture_Ref));
                float3 emissive = ((lerp(_Col_Change.rgb,i.vertexColor.rgb,saturate((_Slider_ColorChange+i.vertexColor.a)))*((_Texture_Ref_var.r*5.0)+1.0))+(_Col_Ref.rgb*_Texture_Ref_var.g));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
