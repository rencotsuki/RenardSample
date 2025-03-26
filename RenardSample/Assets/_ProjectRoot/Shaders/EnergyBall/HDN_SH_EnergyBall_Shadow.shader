// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33476,y:32743,varname:node_3138,prsc:2|emission-3141-OUT,alpha-5960-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32476,y:32120,ptovrint:False,ptlb:Color_Shadow,ptin:_Color_Shadow,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3065,x:32469,y:33441,ptovrint:False,ptlb:Texture_Shadow,ptin:_Texture_Shadow,varname:node_3065,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:baaa0e1a27f856541a6fabb812e9555e,ntxv:0,isnm:False|UVIN-5729-OUT;n:type:ShaderForge.SFN_Tex2d,id:3618,x:32336,y:32912,ptovrint:False,ptlb:Texture_AO,ptin:_Texture_AO,varname:node_3618,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:baaa0e1a27f856541a6fabb812e9555e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_RemapRange,id:2455,x:32284,y:33058,varname:node_2455,prsc:2,frmn:0,frmx:3,tomn:1,tomx:0.2|IN-7547-OUT;n:type:ShaderForge.SFN_Clamp01,id:6171,x:32443,y:33058,varname:node_6171,prsc:2|IN-2455-OUT;n:type:ShaderForge.SFN_Multiply,id:3173,x:32637,y:33011,varname:node_3173,prsc:2|A-3618-R,B-6171-OUT;n:type:ShaderForge.SFN_Add,id:5262,x:32869,y:33065,varname:node_5262,prsc:2|A-3173-OUT,B-8485-OUT;n:type:ShaderForge.SFN_Clamp01,id:7574,x:33062,y:33065,varname:node_7574,prsc:2|IN-5262-OUT;n:type:ShaderForge.SFN_Multiply,id:8485,x:32661,y:33299,varname:node_8485,prsc:2|A-3065-G,B-1855-OUT;n:type:ShaderForge.SFN_Vector1,id:1855,x:32661,y:33441,varname:node_1855,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:2718,x:32878,y:32781,varname:node_2718,prsc:2|A-7241-RGB,B-8485-OUT,C-1268-OUT;n:type:ShaderForge.SFN_RemapRange,id:6888,x:32003,y:33434,varname:node_6888,prsc:2,frmn:0,frmx:3,tomn:1.2,tomx:1.5|IN-7547-OUT;n:type:ShaderForge.SFN_TexCoord,id:1430,x:32003,y:33587,varname:node_1430,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5729,x:32296,y:33514,varname:node_5729,prsc:2|A-6888-OUT,B-1430-UVOUT;n:type:ShaderForge.SFN_OneMinus,id:1268,x:32679,y:32861,varname:node_1268,prsc:2|IN-3173-OUT;n:type:ShaderForge.SFN_Color,id:6350,x:32476,y:32302,ptovrint:False,ptlb:Color_Light,ptin:_Color_Light,varname:node_6350,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_RemapRange,id:2854,x:32250,y:32718,varname:node_2854,prsc:2,frmn:0,frmx:3,tomn:0,tomx:3|IN-7547-OUT;n:type:ShaderForge.SFN_Clamp01,id:3380,x:32414,y:32718,varname:node_3380,prsc:2|IN-2854-OUT;n:type:ShaderForge.SFN_Subtract,id:8599,x:32599,y:32651,varname:node_8599,prsc:2|A-8324-B,B-3380-OUT;n:type:ShaderForge.SFN_Multiply,id:8542,x:33099,y:32432,varname:node_8542,prsc:2|A-1773-OUT,B-6350-RGB,C-8760-OUT,D-1678-OUT;n:type:ShaderForge.SFN_Vector1,id:8760,x:33099,y:32549,varname:node_8760,prsc:2,v1:5;n:type:ShaderForge.SFN_Add,id:2243,x:33071,y:32781,varname:node_2243,prsc:2|A-2718-OUT,B-8542-OUT;n:type:ShaderForge.SFN_Clamp01,id:4080,x:32788,y:32605,varname:node_4080,prsc:2|IN-8599-OUT;n:type:ShaderForge.SFN_RemapRange,id:8439,x:32253,y:32459,varname:node_8439,prsc:2,frmn:0,frmx:3,tomn:1.5,tomx:-2|IN-7547-OUT;n:type:ShaderForge.SFN_Clamp01,id:7655,x:32414,y:32512,varname:node_7655,prsc:2|IN-8439-OUT;n:type:ShaderForge.SFN_Multiply,id:1850,x:32599,y:32512,varname:node_1850,prsc:2|A-7655-OUT,B-3618-R;n:type:ShaderForge.SFN_Add,id:1773,x:32842,y:32475,varname:node_1773,prsc:2|A-1850-OUT,B-4080-OUT;n:type:ShaderForge.SFN_Tex2d,id:8324,x:32039,y:32443,ptovrint:False,ptlb:Texture_Light,ptin:_Texture_Light,varname:node_8324,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:baaa0e1a27f856541a6fabb812e9555e,ntxv:0,isnm:False|UVIN-7636-OUT;n:type:ShaderForge.SFN_Multiply,id:7636,x:31883,y:32583,varname:node_7636,prsc:2|A-5545-OUT,B-1430-UVOUT;n:type:ShaderForge.SFN_RemapRange,id:5545,x:31708,y:32583,varname:node_5545,prsc:2,frmn:0,frmx:3,tomn:2,tomx:-6.4|IN-7547-OUT;n:type:ShaderForge.SFN_Time,id:6402,x:32733,y:32116,varname:node_6402,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9877,x:32899,y:32132,varname:node_9877,prsc:2|A-6402-T,B-1252-OUT;n:type:ShaderForge.SFN_Vector1,id:1252,x:32899,y:32252,varname:node_1252,prsc:2,v1:73.57;n:type:ShaderForge.SFN_Frac,id:8110,x:33062,y:32132,varname:node_8110,prsc:2|IN-9877-OUT;n:type:ShaderForge.SFN_RemapRange,id:1678,x:33224,y:32132,varname:node_1678,prsc:2,frmn:0,frmx:1,tomn:0.7,tomx:1|IN-8110-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7547,x:31647,y:33156,varname:node_7547,prsc:2,min:0,max:3|IN-1856-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1856,x:31465,y:33156,ptovrint:False,ptlb:Value_Height,ptin:_Value_Height,varname:node_1856,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4272,x:31450,y:33372,varname:node_4272,prsc:2|A-7547-OUT,B-1081-OUT;n:type:ShaderForge.SFN_Vector1,id:1081,x:31450,y:33490,varname:node_1081,prsc:2,v1:3;n:type:ShaderForge.SFN_Clamp01,id:6713,x:31615,y:33372,varname:node_6713,prsc:2|IN-4272-OUT;n:type:ShaderForge.SFN_Set,id:3710,x:31771,y:33372,varname:HightDisappear,prsc:2|IN-6713-OUT;n:type:ShaderForge.SFN_Multiply,id:5960,x:33239,y:33065,varname:node_5960,prsc:2|A-7574-OUT,B-1028-OUT,C-6108-OUT;n:type:ShaderForge.SFN_Get,id:1028,x:33218,y:32989,varname:node_1028,prsc:2|IN-3710-OUT;n:type:ShaderForge.SFN_Multiply,id:3141,x:33239,y:32781,varname:node_3141,prsc:2|A-2243-OUT,B-1028-OUT;n:type:ShaderForge.SFN_Vector1,id:6108,x:33239,y:33187,varname:node_6108,prsc:2,v1:0.5;proporder:7241-3065-3618-6350-8324-1856;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_Shadow" {
    Properties {
        _Color_Shadow ("Color_Shadow", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Shadow ("Texture_Shadow", 2D) = "white" {}
        _Texture_AO ("Texture_AO", 2D) = "white" {}
        _Color_Light ("Color_Light", Color) = (0.5,0.5,0.5,1)
        _Texture_Light ("Texture_Light", 2D) = "white" {}
        _Value_Height ("Value_Height", Float ) = 0
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
            //#define UNITY_PASS_FORWARDBASE    //TODO: Warning
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color_Shadow;
            uniform sampler2D _Texture_Shadow; uniform float4 _Texture_Shadow_ST;
            uniform sampler2D _Texture_AO; uniform float4 _Texture_AO_ST;
            uniform float4 _Color_Light;
            uniform sampler2D _Texture_Light; uniform float4 _Texture_Light_ST;
            uniform float _Value_Height;
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
                float node_7547 = clamp(_Value_Height,0,3);
                float2 node_5729 = ((node_7547*0.09999999+1.2)*i.uv0);
                float4 _Texture_Shadow_var = tex2D(_Texture_Shadow,TRANSFORM_TEX(node_5729, _Texture_Shadow));
                float node_8485 = (_Texture_Shadow_var.g*0.5);
                float4 _Texture_AO_var = tex2D(_Texture_AO,TRANSFORM_TEX(i.uv0, _Texture_AO));
                float node_3173 = (_Texture_AO_var.r*saturate((node_7547*-0.2666667+1.0)));
                float2 node_7636 = ((node_7547*-2.8+2.0)*i.uv0);
                float4 _Texture_Light_var = tex2D(_Texture_Light,TRANSFORM_TEX(node_7636, _Texture_Light));
                float4 node_6402 = _Time;
                float HightDisappear = saturate((node_7547*3.0));
                float node_1028 = HightDisappear;
                float3 emissive = (((_Color_Shadow.rgb*node_8485*(1.0 - node_3173))+(((saturate((node_7547*-1.166667+1.5))*_Texture_AO_var.r)+saturate((_Texture_Light_var.b-saturate((node_7547*1.0+0.0)))))*_Color_Light.rgb*5.0*(frac((node_6402.g*73.57))*0.3+0.7)))*node_1028);
                float3 finalColor = emissive;
                return fixed4(finalColor,(saturate((node_3173+node_8485))*node_1028*0.5));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
