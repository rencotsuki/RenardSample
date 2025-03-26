// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34411,y:32731,varname:node_3138,prsc:2|emission-3255-OUT,voffset-9968-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31913,y:32387,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Slider,id:74,x:31748,y:32932,ptovrint:False,ptlb:Appear,ptin:_Appear,varname:node_74,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:6165,x:31839,y:33154,ptovrint:False,ptlb:Damage,ptin:_Damage,varname:node_6165,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7142,x:31882,y:33464,ptovrint:False,ptlb:Break,ptin:_Break,varname:node_7142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:1797,x:32127,y:32811,varname:node_1797,prsc:2,frmn:0,frmx:0.7,tomn:0,tomx:1|IN-74-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4059,x:32301,y:32811,varname:node_4059,prsc:2,min:0,max:1|IN-1797-OUT;n:type:ShaderForge.SFN_RemapRange,id:3132,x:32127,y:32976,varname:node_3132,prsc:2,frmn:0.9,frmx:1,tomn:1,tomx:0|IN-74-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4936,x:32301,y:32976,varname:node_4936,prsc:2,min:0,max:1|IN-3132-OUT;n:type:ShaderForge.SFN_Multiply,id:8055,x:32758,y:32894,varname:node_8055,prsc:2|A-4059-OUT,B-3754-OUT,C-1913-OUT;n:type:ShaderForge.SFN_Vector3,id:7041,x:32938,y:32829,varname:node_7041,prsc:2,v1:0.4643111,v2:0.8523511,v3:0.8867924;n:type:ShaderForge.SFN_Multiply,id:6458,x:32938,y:32917,varname:node_6458,prsc:2|A-8055-OUT,B-7041-OUT,C-2011-OUT,D-6965-OUT;n:type:ShaderForge.SFN_Add,id:4894,x:33356,y:32787,varname:node_4894,prsc:2|A-6458-OUT,B-1077-OUT,C-977-OUT;n:type:ShaderForge.SFN_Multiply,id:1077,x:32964,y:33262,varname:node_1077,prsc:2|A-3119-OUT,B-2084-OUT;n:type:ShaderForge.SFN_Vector3,id:2084,x:32938,y:33075,varname:node_2084,prsc:2,v1:0.6320754,v2:0.468022,v3:0.09242612;n:type:ShaderForge.SFN_RemapRange,id:7562,x:32390,y:33702,varname:node_7562,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-7142-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:6033,x:32564,y:33702,varname:node_6033,prsc:2,min:0,max:1|IN-7562-OUT;n:type:ShaderForge.SFN_RemapRange,id:2874,x:32390,y:33867,varname:node_2874,prsc:2,frmn:0.9,frmx:1,tomn:1,tomx:0|IN-7142-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7145,x:32564,y:33867,varname:node_7145,prsc:2,min:0,max:1|IN-2874-OUT;n:type:ShaderForge.SFN_Multiply,id:7399,x:32822,y:33782,varname:node_7399,prsc:2|A-6966-OUT,B-7145-OUT;n:type:ShaderForge.SFN_Multiply,id:977,x:32977,y:33561,varname:node_977,prsc:2|A-7399-OUT,B-1105-OUT;n:type:ShaderForge.SFN_Vector3,id:1105,x:32964,y:33441,varname:node_1105,prsc:2,v1:0.4716981,v2:0.03337486,v3:0.04680096;n:type:ShaderForge.SFN_Multiply,id:1719,x:33561,y:32787,varname:node_1719,prsc:2|A-4894-OUT,B-2469-OUT;n:type:ShaderForge.SFN_VertexColor,id:6431,x:33408,y:32963,varname:node_6431,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2469,x:33598,y:32963,varname:node_2469,prsc:2|A-6431-R,B-4664-OUT;n:type:ShaderForge.SFN_Vector1,id:4664,x:33589,y:33093,varname:node_4664,prsc:2,v1:2;n:type:ShaderForge.SFN_Time,id:725,x:32034,y:32417,varname:node_725,prsc:2;n:type:ShaderForge.SFN_Frac,id:8789,x:32365,y:32422,varname:node_8789,prsc:2|IN-853-OUT;n:type:ShaderForge.SFN_RemapRange,id:3754,x:32543,y:32422,varname:node_3754,prsc:2,frmn:0,frmx:1,tomn:0.7,tomx:1.2|IN-8789-OUT;n:type:ShaderForge.SFN_Multiply,id:853,x:32199,y:32422,varname:node_853,prsc:2|A-725-T,B-4268-OUT;n:type:ShaderForge.SFN_Vector1,id:4268,x:32199,y:32541,varname:node_4268,prsc:2,v1:47.63;n:type:ShaderForge.SFN_RemapRange,id:9732,x:32365,y:32604,varname:node_9732,prsc:2,frmn:0,frmx:1,tomn:0,tomx:7|IN-8789-OUT;n:type:ShaderForge.SFN_Lerp,id:1913,x:32543,y:32622,varname:node_1913,prsc:2|A-9986-OUT,B-9732-OUT,T-4936-OUT;n:type:ShaderForge.SFN_Vector1,id:9986,x:32543,y:32579,varname:node_9986,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:102,x:32187,y:33205,varname:node_102,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-6165-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4848,x:32360,y:33205,varname:node_4848,prsc:2,min:0,max:1|IN-102-OUT;n:type:ShaderForge.SFN_OneMinus,id:2011,x:32571,y:32993,varname:node_2011,prsc:2|IN-4848-OUT;n:type:ShaderForge.SFN_Lerp,id:3119,x:32593,y:33231,varname:node_3119,prsc:2|A-9154-OUT,B-9732-OUT,T-4848-OUT;n:type:ShaderForge.SFN_Vector1,id:9154,x:32581,y:33177,varname:node_9154,prsc:2,v1:0;n:type:ShaderForge.SFN_OneMinus,id:6965,x:32495,y:33371,varname:node_6965,prsc:2|IN-7142-OUT;n:type:ShaderForge.SFN_Lerp,id:6966,x:32731,y:33549,varname:node_6966,prsc:2|A-8281-OUT,B-9732-OUT,T-6033-OUT;n:type:ShaderForge.SFN_Vector1,id:8281,x:32700,y:33486,varname:node_8281,prsc:2,v1:0;n:type:ShaderForge.SFN_ViewVector,id:3888,x:32909,y:32209,varname:node_3888,prsc:2;n:type:ShaderForge.SFN_Transform,id:8877,x:33091,y:32209,varname:node_8877,prsc:2,tffrom:0,tfto:3|IN-3888-OUT;n:type:ShaderForge.SFN_Multiply,id:9829,x:33248,y:32209,varname:node_9829,prsc:2|A-8877-XYZ,B-8800-OUT;n:type:ShaderForge.SFN_Vector3,id:8800,x:33248,y:32119,varname:node_8800,prsc:2,v1:-1,v2:-1,v3:1;n:type:ShaderForge.SFN_NormalBlend,id:6523,x:33267,y:32392,varname:node_6523,prsc:2|BSE-5064-XYZ,DTL-9829-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3904,x:33430,y:32392,varname:node_3904,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6523-OUT;n:type:ShaderForge.SFN_RemapRange,id:1787,x:33611,y:32392,varname:node_1787,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-3904-OUT;n:type:ShaderForge.SFN_Tex2d,id:6243,x:32844,y:32398,ptovrint:False,ptlb:Texture_Normal,ptin:_Texture_Normal,varname:node_1006,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Transform,id:5064,x:33001,y:32415,varname:node_5064,prsc:2,tffrom:2,tfto:3|IN-6243-RGB;n:type:ShaderForge.SFN_Tex2d,id:8344,x:33783,y:32332,ptovrint:False,ptlb:Texture_Ref_S,ptin:_Texture_Ref_S,varname:node_8344,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1787-OUT;n:type:ShaderForge.SFN_Add,id:3255,x:34034,y:32683,varname:node_3255,prsc:2|A-6046-OUT,B-4127-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4127,x:33755,y:32787,varname:node_4127,prsc:2,min:0,max:1|IN-1719-OUT;n:type:ShaderForge.SFN_Multiply,id:8242,x:34572,y:32103,varname:node_8242,prsc:2|A-4932-OUT,B-4599-RGB;n:type:ShaderForge.SFN_Color,id:4599,x:34547,y:31970,ptovrint:False,ptlb:Color_Ref,ptin:_Color_Ref,varname:node_4599,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Fresnel,id:4932,x:33911,y:31927,varname:node_4932,prsc:2;n:type:ShaderForge.SFN_ViewVector,id:968,x:35324,y:32294,varname:node_968,prsc:2;n:type:ShaderForge.SFN_Transform,id:8262,x:35506,y:32294,varname:node_8262,prsc:2,tffrom:0,tfto:3|IN-968-OUT;n:type:ShaderForge.SFN_Multiply,id:9556,x:35663,y:32294,varname:node_9556,prsc:2|A-8262-XYZ,B-8885-OUT;n:type:ShaderForge.SFN_Vector3,id:8885,x:35663,y:32204,varname:node_8885,prsc:2,v1:-1,v2:-1,v3:1;n:type:ShaderForge.SFN_NormalBlend,id:7161,x:35682,y:32477,varname:node_7161,prsc:2|BSE-9475-XYZ,DTL-9556-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2753,x:35845,y:32477,varname:node_2753,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7161-OUT;n:type:ShaderForge.SFN_RemapRange,id:4918,x:36026,y:32477,varname:node_4918,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-2753-OUT;n:type:ShaderForge.SFN_Tex2d,id:3041,x:35259,y:32483,ptovrint:False,ptlb:Texture_Normal_copy,ptin:_Texture_Normal_copy,varname:_Texture_Normal_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Transform,id:9475,x:35416,y:32500,varname:node_9475,prsc:2,tffrom:2,tfto:3|IN-3041-RGB;n:type:ShaderForge.SFN_Add,id:2789,x:34051,y:32417,varname:node_2789,prsc:2|A-8242-OUT,B-8344-R,C-3304-OUT;n:type:ShaderForge.SFN_Multiply,id:3304,x:34237,y:32020,varname:node_3304,prsc:2|A-8344-G,B-6485-RGB;n:type:ShaderForge.SFN_Color,id:6485,x:34237,y:32165,ptovrint:False,ptlb:Color_Specular,ptin:_Color_Specular,varname:node_6485,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6046,x:34249,y:32404,varname:node_6046,prsc:2|A-2789-OUT,B-6431-B;n:type:ShaderForge.SFN_Time,id:6525,x:33705,y:33180,varname:node_6525,prsc:2;n:type:ShaderForge.SFN_Sin,id:5762,x:33863,y:33167,varname:node_5762,prsc:2|IN-6525-T;n:type:ShaderForge.SFN_Multiply,id:4397,x:34044,y:33167,varname:node_4397,prsc:2|A-5762-OUT,B-9903-OUT;n:type:ShaderForge.SFN_Vector1,id:9903,x:34044,y:33286,varname:node_9903,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Append,id:9968,x:34229,y:33167,varname:node_9968,prsc:2|A-806-OUT,B-4397-OUT,C-806-OUT;n:type:ShaderForge.SFN_Vector1,id:806,x:34229,y:33286,varname:node_806,prsc:2,v1:0;proporder:7241-74-6165-7142-6243-8344-4599-6485;pass:END;sub:END;*/

Shader "Shader Forge/CustomHado/HDC_SH_CPUPanel_Base" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Appear ("Appear", Range(0, 1)) = 0
        _Damage ("Damage", Range(0, 1)) = 0
        _Break ("Break", Range(0, 1)) = 0
        _Texture_Normal ("Texture_Normal", 2D) = "bump" {}
        _Texture_Ref_S ("Texture_Ref_S", 2D) = "white" {}
        _Color_Ref ("Color_Ref", Color) = (0.5,0.5,0.5,1)
        _Color_Specular ("Color_Specular", Color) = (0.5,0.5,0.5,1)
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            uniform float _Appear;
            uniform float _Damage;
            uniform float _Break;
            uniform sampler2D _Texture_Normal; uniform float4 _Texture_Normal_ST;
            uniform sampler2D _Texture_Ref_S; uniform float4 _Texture_Ref_S_ST;
            uniform float4 _Color_Ref;
            uniform float4 _Color_Specular;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float node_806 = 0.0;
                float4 node_6525 = _Time;
                v.vertex.xyz += float3(node_806,(sin(node_6525.g)*0.05),node_806);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 _Texture_Normal_var = UnpackNormal(tex2D(_Texture_Normal,TRANSFORM_TEX(i.uv0, _Texture_Normal)));
                float3 node_6523_nrm_base = mul( UNITY_MATRIX_V, float4(mul( _Texture_Normal_var.rgb, tangentTransform ),0) ).xyz.rgb + float3(0,0,1);
                float3 node_6523_nrm_detail = (mul( UNITY_MATRIX_V, float4(viewDirection,0) ).xyz.rgb*float3(-1,-1,1)) * float3(-1,-1,1);
                float3 node_6523_nrm_combined = node_6523_nrm_base*dot(node_6523_nrm_base, node_6523_nrm_detail)/node_6523_nrm_base.z - node_6523_nrm_detail;
                float3 node_6523 = node_6523_nrm_combined;
                float2 node_1787 = (node_6523.rg*0.5+0.5);
                float4 _Texture_Ref_S_var = tex2D(_Texture_Ref_S,TRANSFORM_TEX(node_1787, _Texture_Ref_S));
                float4 node_725 = _Time;
                float node_8789 = frac((node_725.g*47.63));
                float node_9732 = (node_8789*7.0+0.0);
                float node_4848 = clamp((_Damage*10.0+0.0),0,1);
                float3 emissive = (((((1.0-max(0,dot(normalDirection, viewDirection)))*_Color_Ref.rgb)+_Texture_Ref_S_var.r+(_Texture_Ref_S_var.g*_Color_Specular.rgb))*i.vertexColor.b)+clamp(((((clamp((_Appear*1.428571+0.0),0,1)*(node_8789*0.5000001+0.7)*lerp(1.0,node_9732,clamp((_Appear*-9.999998+9.999998),0,1)))*float3(0.4643111,0.8523511,0.8867924)*(1.0 - node_4848)*(1.0 - _Break))+(lerp(0.0,node_9732,node_4848)*float3(0.6320754,0.468022,0.09242612))+((lerp(0.0,node_9732,clamp((_Break*10.0+0.0),0,1))*clamp((_Break*-9.999998+9.999998),0,1))*float3(0.4716981,0.03337486,0.04680096)))*(i.vertexColor.r*2.0)),0,1));
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
            Cull Back
            
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float node_806 = 0.0;
                float4 node_6525 = _Time;
                v.vertex.xyz += float3(node_806,(sin(node_6525.g)*0.05),node_806);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
