// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34610,y:32646,varname:node_3138,prsc:2|emission-8475-OUT,alpha-6481-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:33416,y:32052,ptovrint:False,ptlb:Color_Outline,ptin:_Color_Outline,varname:_Color_Outline,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Fresnel,id:836,x:32381,y:32830,varname:node_836,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:1196,x:32627,y:32813,varname:node_1196,prsc:2,frmn:0,frmx:1,tomn:6,tomx:-0.3|IN-836-OUT;n:type:ShaderForge.SFN_Clamp01,id:2112,x:32782,y:32813,cmnt:Outline,varname:node_2112,prsc:2|IN-1196-OUT;n:type:ShaderForge.SFN_Clamp01,id:2840,x:32782,y:32977,cmnt:Outline Inner,varname:node_2840,prsc:2|IN-9532-OUT;n:type:ShaderForge.SFN_Multiply,id:2454,x:32981,y:32922,varname:node_2454,prsc:2|A-2112-OUT,B-2840-OUT;n:type:ShaderForge.SFN_Tex2d,id:5237,x:32552,y:32228,ptovrint:False,ptlb:Texture_Dot_S,ptin:_Texture_Dot_S,varname:_Texture_Dot_S,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2771-OUT;n:type:ShaderForge.SFN_OneMinus,id:9961,x:33151,y:32922,varname:node_9961,prsc:2|IN-2454-OUT;n:type:ShaderForge.SFN_Subtract,id:6467,x:33308,y:32685,varname:node_6467,prsc:2|A-5002-OUT,B-9644-OUT;n:type:ShaderForge.SFN_Multiply,id:6808,x:33726,y:32685,cmnt:Outline Col,varname:node_6808,prsc:2|A-4576-OUT,B-4388-OUT,C-7241-RGB;n:type:ShaderForge.SFN_Vector1,id:4388,x:33726,y:32810,varname:node_4388,prsc:2,v1:6;n:type:ShaderForge.SFN_Tex2d,id:6345,x:32807,y:32596,ptovrint:False,ptlb:Texture_Ref,ptin:_Texture_Ref,varname:_Texture_Ref,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5564-OUT;n:type:ShaderForge.SFN_NormalVector,id:1775,x:32155,y:32596,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:9467,x:32314,y:32596,varname:node_9467,prsc:2,tffrom:0,tfto:3|IN-1775-OUT;n:type:ShaderForge.SFN_RemapRange,id:9485,x:32483,y:32596,varname:node_9485,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9467-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:5564,x:32649,y:32596,varname:node_5564,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9485-OUT;n:type:ShaderForge.SFN_Min,id:5940,x:33044,y:32426,varname:node_5940,prsc:2|A-6345-B,B-6820-OUT;n:type:ShaderForge.SFN_Vector1,id:6820,x:33044,y:32538,varname:node_6820,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Subtract,id:5002,x:33299,y:32401,varname:node_5002,prsc:2|A-3019-OUT,B-5940-OUT;n:type:ShaderForge.SFN_Clamp01,id:4576,x:33481,y:32685,varname:node_4576,prsc:2|IN-6467-OUT;n:type:ShaderForge.SFN_Subtract,id:4879,x:33044,y:32203,varname:node_4879,prsc:2|A-3019-OUT,B-6345-R;n:type:ShaderForge.SFN_Multiply,id:1757,x:33667,y:32221,varname:node_1757,prsc:2|A-4703-OUT,B-7564-RGB,C-8762-OUT,D-1194-OUT;n:type:ShaderForge.SFN_Color,id:7564,x:33416,y:31887,ptovrint:False,ptlb:Color_Base,ptin:_Color_Base,varname:_Color_Base,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:1141,x:33938,y:32685,varname:node_1141,prsc:2|A-1757-OUT,B-6808-OUT;n:type:ShaderForge.SFN_Clamp01,id:4703,x:33387,y:32203,varname:node_4703,prsc:2|IN-5432-OUT;n:type:ShaderForge.SFN_Vector1,id:8762,x:33649,y:32364,varname:node_8762,prsc:2,v1:4;n:type:ShaderForge.SFN_OneMinus,id:2050,x:32947,y:32723,varname:node_2050,prsc:2|IN-2112-OUT;n:type:ShaderForge.SFN_Subtract,id:5432,x:33212,y:32203,varname:node_5432,prsc:2|A-4879-OUT,B-2050-OUT;n:type:ShaderForge.SFN_Multiply,id:6481,x:33669,y:32965,varname:node_6481,prsc:2|A-4703-OUT,B-7501-OUT,C-1194-OUT,D-5813-OUT,E-2538-OUT;n:type:ShaderForge.SFN_Vector1,id:7501,x:33669,y:33084,varname:node_7501,prsc:2,v1:0.7;n:type:ShaderForge.SFN_FragmentPosition,id:9560,x:31430,y:30913,varname:node_9560,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2485,x:32604,y:31748,varname:node_2485,prsc:2,frmn:-0.02,frmx:0.02,tomn:0,tomx:1|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:1194,x:32769,y:31748,varname:node_1194,prsc:2|IN-2485-OUT;n:type:ShaderForge.SFN_RemapRange,id:9615,x:32604,y:31906,varname:node_9615,prsc:2,frmn:0.02,frmx:0.05,tomn:1,tomx:0|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:4303,x:32769,y:31906,varname:node_4303,prsc:2|IN-9615-OUT;n:type:ShaderForge.SFN_RemapRange,id:2127,x:32603,y:32065,varname:node_2127,prsc:2,frmn:0,frmx:-0.02,tomn:1,tomx:0|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:6718,x:32769,y:32065,varname:node_6718,prsc:2|IN-2127-OUT;n:type:ShaderForge.SFN_Multiply,id:9898,x:32939,y:31983,varname:node_9898,prsc:2|A-4303-OUT,B-6718-OUT;n:type:ShaderForge.SFN_Subtract,id:9644,x:33338,y:32922,varname:node_9644,prsc:2|A-9961-OUT,B-9898-OUT;n:type:ShaderForge.SFN_RemapRange,id:4410,x:32604,y:31572,varname:node_4410,prsc:2,frmn:-0.05,frmx:-0.5,tomn:1,tomx:0|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:5813,x:32807,y:31572,varname:node_5813,prsc:2|IN-4410-OUT;n:type:ShaderForge.SFN_Multiply,id:8475,x:34188,y:32685,varname:node_8475,prsc:2|A-1141-OUT,B-5813-OUT,C-2538-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2538,x:33996,y:32414,ptovrint:False,ptlb:Value_Alpha,ptin:_Value_Alpha,varname:_Value_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:9532,x:32627,y:32977,varname:node_9532,prsc:2|IN-836-OUT,IMIN-1642-OUT,IMAX-9863-OUT,OMIN-202-OUT,OMAX-9158-OUT;n:type:ShaderForge.SFN_Vector1,id:1642,x:32627,y:33096,varname:node_1642,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9863,x:32627,y:33147,varname:node_9863,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3841,x:32171,y:33280,ptovrint:False,ptlb:Outline_Width,ptin:_Outline_Width,varname:_Outline_Width,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:9158,x:32627,y:33199,varname:node_9158,prsc:2,v1:2.2;n:type:ShaderForge.SFN_TexCoord,id:3079,x:31533,y:32044,varname:node_3079,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:8526,x:31800,y:31718,varname:node_8526,prsc:2,frmn:0.7,frmx:1,tomn:1,tomx:0|IN-3079-V;n:type:ShaderForge.SFN_Clamp01,id:4567,x:31966,y:31718,varname:node_4567,prsc:2|IN-8526-OUT;n:type:ShaderForge.SFN_RemapRange,id:1649,x:31800,y:31878,varname:node_1649,prsc:2,frmn:0,frmx:0.3,tomn:0,tomx:1|IN-3079-V;n:type:ShaderForge.SFN_Clamp01,id:2629,x:31966,y:31878,varname:node_2629,prsc:2|IN-1649-OUT;n:type:ShaderForge.SFN_Multiply,id:177,x:32123,y:31793,varname:node_177,prsc:2|A-4567-OUT,B-2629-OUT;n:type:ShaderForge.SFN_Tex2d,id:815,x:31700,y:32328,ptovrint:False,ptlb:Texture_Ind,ptin:_Texture_Ind,varname:_Texture_Ind,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9089-OUT;n:type:ShaderForge.SFN_Time,id:8277,x:30878,y:32382,varname:node_8277,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8503,x:31056,y:32270,varname:node_8503,prsc:2|A-8277-T,B-3523-OUT;n:type:ShaderForge.SFN_Vector1,id:3523,x:31056,y:32382,varname:node_3523,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6963,x:31056,y:32439,varname:node_6963,prsc:2|A-8277-T,B-2527-OUT;n:type:ShaderForge.SFN_Vector1,id:2527,x:31231,y:32554,varname:node_2527,prsc:2,v1:1.33;n:type:ShaderForge.SFN_Add,id:8376,x:31231,y:32270,varname:node_8376,prsc:2|A-3079-U,B-8503-OUT;n:type:ShaderForge.SFN_Add,id:5359,x:31244,y:32439,varname:node_5359,prsc:2|A-6963-OUT,B-3079-V;n:type:ShaderForge.SFN_Append,id:9089,x:31399,y:32333,varname:node_9089,prsc:2|A-8376-OUT,B-5359-OUT;n:type:ShaderForge.SFN_RemapRange,id:796,x:31872,y:32427,varname:node_796,prsc:2,frmn:0,frmx:1,tomn:-0.1,tomx:0.1|IN-815-R;n:type:ShaderForge.SFN_Add,id:9802,x:32252,y:32221,varname:node_9802,prsc:2|A-3079-U,B-3298-OUT;n:type:ShaderForge.SFN_Append,id:2771,x:32293,y:32376,varname:node_2771,prsc:2|A-9802-OUT,B-3079-V;n:type:ShaderForge.SFN_Multiply,id:3298,x:32044,y:32274,varname:node_3298,prsc:2|A-796-OUT,B-177-OUT;n:type:ShaderForge.SFN_Vector1,id:9371,x:32171,y:33342,varname:node_9371,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:6768,x:32350,y:33280,varname:node_6768,prsc:2|A-3841-OUT,B-9371-OUT;n:type:ShaderForge.SFN_Multiply,id:4794,x:32518,y:33280,varname:node_4794,prsc:2|A-6768-OUT,B-1211-OUT;n:type:ShaderForge.SFN_Vector1,id:1211,x:32518,y:33417,varname:node_1211,prsc:2,v1:-3;n:type:ShaderForge.SFN_Subtract,id:202,x:32686,y:33280,varname:node_202,prsc:2|A-4794-OUT,B-3972-OUT;n:type:ShaderForge.SFN_Vector1,id:3972,x:32686,y:33417,varname:node_3972,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:67,x:32552,y:32424,ptovrint:False,ptlb:Texture_Dot_M,ptin:_Texture_Dot_M,varname:_Texture_Dot_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2771-OUT;n:type:ShaderForge.SFN_Lerp,id:3019,x:32775,y:32340,varname:node_3019,prsc:2|A-5237-R,B-67-R,T-7756-OUT;n:type:ShaderForge.SFN_Subtract,id:2503,x:31997,y:33023,varname:node_2503,prsc:2|A-3841-OUT,B-4786-OUT;n:type:ShaderForge.SFN_Vector1,id:4786,x:31997,y:33159,varname:node_4786,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Ceil,id:966,x:32171,y:33023,varname:node_966,prsc:2|IN-2503-OUT;n:type:ShaderForge.SFN_Clamp01,id:7756,x:32347,y:33023,varname:node_7756,prsc:2|IN-966-OUT;n:type:ShaderForge.SFN_Code,id:3512,x:31755,y:31087,varname:node_3512,prsc:2,code:cgBlAHQAdQByAG4AIABtAHUAbAAoAHcAbwByAGwAZABQAG8AcwAgAC0AIABhAG4AYwBoAG8AcgBQAG8AcwAsACAAdAApADsA,output:3,fname:Function_node_3512,width:633,height:133,input:2,input:2,input:13,input_1_label:worldPos,input_2_label:anchorPos,input_3_label:t|A-9560-XYZ,B-1451-XYZ,C-5595-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9382,x:32069,y:31388,varname:node_9382,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-3512-OUT;n:type:ShaderForge.SFN_Vector4Property,id:1451,x:31388,y:31098,ptovrint:False,ptlb:anchorPos,ptin:_anchorPos,varname:_anchorPos,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Matrix4x4Property,id:5595,x:31445,y:31296,ptovrint:False,ptlb:transformMatrix,ptin:transformMatrix,varname:node_5595,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,m00:1,m01:0,m02:0,m03:0,m10:0,m11:1,m12:0,m13:0,m20:0,m21:0,m22:1,m23:0,m30:0,m31:0,m32:0,m33:1;proporder:7241-5237-6345-7564-2538-3841-815-67-1451;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_Dot" {
    Properties {
        _Color_Outline ("Color_Outline", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Dot_S ("Texture_Dot_S", 2D) = "white" {}
        _Texture_Ref ("Texture_Ref", 2D) = "white" {}
        _Color_Base ("Color_Base", Color) = (0.5,0.5,0.5,1)
        _Value_Alpha ("Value_Alpha", Float ) = 1
        _Outline_Width ("Outline_Width", Float ) = 0.5
        _Texture_Ind ("Texture_Ind", 2D) = "white" {}
        _Texture_Dot_M ("Texture_Dot_M", 2D) = "white" {}
        _anchorPos ("anchorPos", Vector) = (0,0,0,0)
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            uniform float4 _Color_Outline;
            uniform sampler2D _Texture_Dot_S; uniform float4 _Texture_Dot_S_ST;
            uniform sampler2D _Texture_Ref; uniform float4 _Texture_Ref_ST;
            uniform float4 _Color_Base;
            uniform float _Value_Alpha;
            uniform float _Outline_Width;
            uniform sampler2D _Texture_Ind; uniform float4 _Texture_Ind_ST;
            uniform sampler2D _Texture_Dot_M; uniform float4 _Texture_Dot_M_ST;
            float4 Function_node_3512( float3 worldPos , float3 anchorPos , float4x4 t ){
            return mul(worldPos - anchorPos, t);
            }
            
            uniform float4 _anchorPos;
            uniform float4x4 transformMatrix;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_8277 = _Time;
                float2 node_9089 = float2((i.uv0.r+(node_8277.g*1.0)),((node_8277.g*1.33)+i.uv0.g));
                float4 _Texture_Ind_var = tex2D(_Texture_Ind,TRANSFORM_TEX(node_9089, _Texture_Ind));
                float2 node_2771 = float2((i.uv0.r+((_Texture_Ind_var.r*0.2+-0.1)*(saturate((i.uv0.g*-3.333333+3.333333))*saturate((i.uv0.g*3.333333+0.0))))),i.uv0.g);
                float4 _Texture_Dot_S_var = tex2D(_Texture_Dot_S,TRANSFORM_TEX(node_2771, _Texture_Dot_S));
                float4 _Texture_Dot_M_var = tex2D(_Texture_Dot_M,TRANSFORM_TEX(node_2771, _Texture_Dot_M));
                float node_3019 = lerp(_Texture_Dot_S_var.r,_Texture_Dot_M_var.r,saturate(ceil((_Outline_Width-0.4))));
                float2 node_5564 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb*0.5+0.5).rg;
                float4 _Texture_Ref_var = tex2D(_Texture_Ref,TRANSFORM_TEX(node_5564, _Texture_Ref));
                float node_836 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_2112 = saturate((node_836*-6.3+6.0)); // Outline
                float node_4703 = saturate(((node_3019-_Texture_Ref_var.r)-(1.0 - node_2112)));
                float node_9382 = Function_node_3512( i.posWorld.rgb , _anchorPos.rgb , transformMatrix ).g;
                float node_1194 = saturate((node_9382*25.0+0.5));
                float node_1642 = 0.0;
                float node_202 = (((_Outline_Width-0.5)*(-3.0))-0.5);
                float node_5813 = saturate((node_9382*2.222222+1.111111));
                float3 emissive = (((node_4703*_Color_Base.rgb*4.0*node_1194)+(saturate(((node_3019-min(_Texture_Ref_var.b,0.15))-((1.0 - (node_2112*saturate((node_202 + ( (node_836 - node_1642) * (2.2 - node_202) ) / (1.0 - node_1642)))))-(saturate((node_9382*-33.33333+1.666667))*saturate((node_9382*50.0+1.0))))))*6.0*_Color_Outline.rgb))*node_5813*_Value_Alpha);
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_4703*0.7*node_1194*node_5813*_Value_Alpha));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
