// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33491,y:32783,varname:node_3138,prsc:2|emission-1548-OUT,alpha-7151-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:33037,y:32388,ptovrint:False,ptlb:Color_Base,ptin:_Color_Base,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9767,x:32509,y:32693,ptovrint:False,ptlb:Texture_Color,ptin:_Texture_Color,varname:node_9767,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-307-OUT;n:type:ShaderForge.SFN_Multiply,id:4801,x:33037,y:32547,varname:node_4801,prsc:2|A-7241-RGB,B-1317-R;n:type:ShaderForge.SFN_Multiply,id:7151,x:33211,y:32980,varname:node_7151,prsc:2|A-1317-R,B-2985-OUT,C-4663-OUT;n:type:ShaderForge.SFN_Time,id:8607,x:31290,y:33154,varname:node_8607,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9560,x:31460,y:33154,varname:node_9560,prsc:2|A-8607-T,B-6424-OUT;n:type:ShaderForge.SFN_Vector1,id:6424,x:31460,y:33271,varname:node_6424,prsc:2,v1:-5;n:type:ShaderForge.SFN_RemapRange,id:460,x:32490,y:33049,varname:node_460,prsc:2,frmn:0,frmx:1,tomn:0.9,tomx:1.5|IN-5055-OUT;n:type:ShaderForge.SFN_TexCoord,id:1277,x:31447,y:32710,varname:node_1277,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5532,x:32086,y:32730,varname:node_5532,prsc:2|A-1277-U,B-460-OUT;n:type:ShaderForge.SFN_Append,id:307,x:32317,y:32794,varname:node_307,prsc:2|A-5532-OUT,B-1277-V;n:type:ShaderForge.SFN_Tex2d,id:8143,x:31997,y:33046,ptovrint:False,ptlb:Texture_Indirect,ptin:_Texture_Indirect,varname:node_8143,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4152-OUT;n:type:ShaderForge.SFN_Add,id:6393,x:31641,y:33046,varname:node_6393,prsc:2|A-1277-U,B-9560-OUT;n:type:ShaderForge.SFN_Append,id:4152,x:31799,y:33046,varname:node_4152,prsc:2|A-6393-OUT,B-1277-V;n:type:ShaderForge.SFN_RemapRange,id:2708,x:31997,y:33224,varname:node_2708,prsc:2,frmn:0,frmx:1,tomn:1,tomx:-1|IN-1277-U;n:type:ShaderForge.SFN_Clamp01,id:8021,x:32164,y:33224,varname:node_8021,prsc:2|IN-2708-OUT;n:type:ShaderForge.SFN_Add,id:917,x:32204,y:33095,varname:node_917,prsc:2|A-8143-R,B-8021-OUT;n:type:ShaderForge.SFN_Clamp01,id:5055,x:32361,y:33095,varname:node_5055,prsc:2|IN-917-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4663,x:33170,y:32848,ptovrint:False,ptlb:Value_Alpha,ptin:_Value_Alpha,varname:node_4663,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ComponentMask,id:1317,x:32862,y:32693,varname:node_1317,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9767-RGB;n:type:ShaderForge.SFN_Min,id:1548,x:33415,y:32583,varname:node_1548,prsc:2|A-4801-OUT,B-4663-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2985,x:33211,y:33134,ptovrint:False,ptlb:Value_Opacity,ptin:_Value_Opacity,varname:node_2985,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Code,id:7679,x:31561,y:34478,varname:node_7679,prsc:2,code:cgBlAHQAdQByAG4AIAB3AG8AcgBsAGQAUABvAHMAIAAtACAAbQB1AGwAKAB1AG4AaQB0AHkAXwBPAGIAagBlAGMAdABUAG8AVwBvAHIAbABkACwAIABmAGwAbwBhAHQANAAoADAALAAgADAALAAgADAALAAgADEAKQApAC4AeAB5AHoAOwA=,output:2,fname:Function_node_4496,width:626,height:143,input:2,input_1_label:worldPos;proporder:7241-9767-8143-4663-2985;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_Trail" {
    Properties {
        _Color_Base ("Color_Base", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Color ("Texture_Color", 2D) = "white" {}
        _Texture_Indirect ("Texture_Indirect", 2D) = "white" {}
        _Value_Alpha ("Value_Alpha", Float ) = 1
        _Value_Opacity ("Value_Opacity", Float ) = 0.5
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan 
            #pragma target 3.0
            uniform float4 _Color_Base;
            uniform sampler2D _Texture_Color; uniform float4 _Texture_Color_ST;
            uniform sampler2D _Texture_Indirect; uniform float4 _Texture_Indirect_ST;
            uniform float _Value_Alpha;
            uniform float _Value_Opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_8607 = _Time;
                float2 node_4152 = float2((i.uv0.r+(node_8607.g*(-5.0))),i.uv0.g);
                float4 _Texture_Indirect_var = tex2D(_Texture_Indirect,TRANSFORM_TEX(node_4152, _Texture_Indirect));
                float2 node_307 = float2((i.uv0.r*(saturate((_Texture_Indirect_var.r+saturate((i.uv0.r*-2.0+1.0))))*0.6+0.9)),i.uv0.g);
                float4 _Texture_Color_var = tex2D(_Texture_Color,TRANSFORM_TEX(node_307, _Texture_Color));
                float2 node_1317 = _Texture_Color_var.rgb.rg;
                float3 emissive = min((_Color_Base.rgb*node_1317.r),_Value_Alpha);
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_1317.r*_Value_Opacity*_Value_Alpha));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
